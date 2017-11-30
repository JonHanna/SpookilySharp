// SpookierEqualityComparer.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014–2017 Jon Hanna
//
// Licensed under the MIT license. See the LICENSE file in the repository root for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SpookilySharp
{
    /// <summary>Improves the bit distribution of equality comparers.</summary>
    public static class SpookierEqualityComparers
    {
        // Note that this store is segregated by the type of the comparer interface
        // as well as looking-up by the type of the implementing class: It is possible
        // for a class to implement IEqualityComparer<T> for one T with [WellDistributedHash]
        // and for another T without.
        private static class HashQualityStore<T>
        {
            // Not worth using a concurrent dictionary or (importing Ariadne or similar for < 4.0) here
            // as we should expect very few concurrent writes.
            // Note: Hashtable is documented as thread-safe for single-writer, multiple-reader.
            // ReSharper disable once StaticMemberInGenericType
            private static readonly Hashtable Store = new Hashtable();

            public static bool? KnownQuality(Type type) => (bool?)Store[type];

            public static void Record(Type type, bool good)
            {
                lock (Store)
                {
                    Store[type] = good;
                }
            }
        }

        [Serializable]
        private sealed class WellDistributedEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly IEqualityComparer<T> _cmp;

            public WellDistributedEqualityComparer(IEqualityComparer<T> comparer) => _cmp = comparer;

            public bool Equals(T x, T y) => _cmp.Equals(x, y);

            [WellDistributedHash]
            public int GetHashCode(T obj) => obj == null ? 0 : _cmp.GetHashCode(obj).Rehash();

            public override bool Equals(object obj) => obj is WellDistributedEqualityComparer<T> other && _cmp.Equals(other._cmp);

            public override int GetHashCode() => _cmp.GetHashCode().Rehash();
        }

        /// <summary>
        /// Returns a version of <paramref name="comparer"/> that provides strong distribution of bits in its hash
        /// codes.
        /// </summary>
        /// <returns>An <see cref="IEqualityComparer{T}"/> based on <paramref name="comparer"/>, or
        /// <paramref name="comparer"/> if its implementation of
        /// <see cref="M:System.Collections.Generic.IEqualityComparer`1.GetHashCode(`0)"/> is marked with
        /// <see cref="WellDistributedHashAttribute"/>, indicating it already provides a strong distribution.</returns>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to improve.</param>
        /// <typeparam name="T">The type of objects compared by <paramref name="comparer"/>.</typeparam>
        /// <remarks>This cannot improve the overall risk of collision (indeed, will make it slightly worse), it can
        /// help when uses of hash codes are particularly sensitive to collisions in the one section of bits, e.g. with
        /// power-of-two hash tables.</remarks>
        public static IEqualityComparer<T> WellDistributed<T>(this IEqualityComparer<T> comparer)
        {
            if (IntPtr.Size == 8 && typeof(T) == typeof(string) && EqualityComparer<string>.Default.Equals(comparer))
            {
                return (IEqualityComparer<T>)(object)new SpookyStringEqualityComparer();
            }

            return IsGood(comparer) ? comparer : new WellDistributedEqualityComparer<T>(comparer);
        }

        private static bool IsGood<T>(IEqualityComparer<T> comparer)
        {
            Type type = comparer.GetType();
            bool? knownGood = HashQualityStore<T>.KnownQuality(type);
            if (knownGood.HasValue)
            {
                return knownGood.Value;
            }

            bool good = DetermineGood(comparer, type);
            HashQualityStore<T>.Record(type, good);
            return good;
        }

        private static bool DetermineGood<T>(IEqualityComparer<T> comparer, Type type)
        {
            if (EqualityComparer<T>.Default.Equals(comparer))
            {
                return typeof(T).GetMethod("GetHashCode", new Type[0])
                           .GetCustomAttributes(typeof(WellDistributedHashAttribute), false)
                           .Length == 1;
            }

            InterfaceMapping imap = type.GetInterfaceMap(typeof(IEqualityComparer<T>));
            int idx = imap.InterfaceMethods[0].ReturnType == typeof(int) ? 0 : 1;
            return imap.TargetMethods[idx].GetCustomAttributes(typeof(WellDistributedHashAttribute), false).Length != 0;
        }
    }
}
