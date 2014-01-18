// SpookierEqualityComparer.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014 Jon Hanna
//
// Licensed under the EUPL, Version 1.1 only (the “Licence”).
// You may not use, modify or distribute this work except in compliance with the Licence.
// You may obtain a copy of the Licence at:
// <http://joinup.ec.europa.eu/software/page/eupl/licence-eupl>
// A copy is also distributed with this source code.
// Unless required by applicable law or agreed to in writing, software distributed under the
// Licence is distributed on an “AS IS” basis, without warranties or conditions of any kind.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

// Analysis disable MemberHidesStaticFromOuterClass
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
            // Dictionary<TKey, TValue> as implemented in both .NET and Mono is safe for that when
            // both key and value types are capable of atomic read or write , but not documented as such,
            // so we play it safe.            
            // Analysis disable once StaticFieldInGenericType
            private static readonly Hashtable Store = new Hashtable();
            public static bool? KnownQuality(Type type)
            {
                return (bool?)Store[type];
            }
            public static void Record(Type type, bool good)
            {
                lock(Store)
                    Store[type] = good;
            }
        }

        private sealed class WellDistributedEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly IEqualityComparer<T> _cmp;
            public WellDistributedEqualityComparer(IEqualityComparer<T> comparer)
            {
                _cmp = comparer;
            }
            public bool Equals(T x, T y)
            {
                return _cmp.Equals(x, y);
            }
            [WellDistributedHash]
            public int GetHashCode(T obj)
            {
                // Analysis disable once CompareNonConstrainedGenericWithNull
                return obj == null ? 0 : _cmp.GetHashCode(obj).Rehash();
            }
        }

        /// <summary>
        /// Returns a version of <paramref name="comparer"/> that provides strong distribution of bits in its hash codes.
        /// </summary>
        /// <returns>An <see cref="IEqualityComparer{T}"/> based on <paramref name="comparer"/>, or
        /// <paramref name="comparer"/> if its implementation of <see cref="M:System.Collections.Generic.IEqualityComparer`1.GetHashCode(`0)"/> is
        /// marked with <see cref="WellDistributedHashAttribute"/>, indicating it already provides a strong distribution.</returns>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to improve.</param>
        /// <typeparam name="T">The type of objects compared by <paramref name="comparer"/>.</typeparam>
        /// <remarks>This cannot improve the overall risk of collision (indeed, will
        /// make it slightly worse), it can help when uses of hash codes are particularly sensitive to collisions in the one
        /// section of bits, e.g. with power-of-two hash tables.</remarks>
        public static IEqualityComparer<T> WellDistributed<T>(this IEqualityComparer<T> comparer)
        {
            if(IntPtr.Size == 8 && typeof(T) == typeof(string) && EqualityComparer<string>.Default.Equals(comparer))
                return (IEqualityComparer<T>)(object)new SpookyStringEqualityComparer();
            return IsGood<T>(comparer) ? comparer : new WellDistributedEqualityComparer<T>(comparer);
        }

        private static bool IsGood<T>(IEqualityComparer<T> comparer)
        {
            Type type = comparer.GetType();
            bool? knownGood = HashQualityStore<T>.KnownQuality(type);
            if(knownGood.HasValue)
                return knownGood.Value;
            bool good = DetermineGood(comparer, type);
            HashQualityStore<T>.Record(type, good);
            return good;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Allows for type inference.")]
        private static bool DetermineGood<T>(IEqualityComparer<T> comparer, Type type)
        {
            if(EqualityComparer<T>.Default.Equals(comparer))
                return typeof(T).GetMethod("GetHashCode", new Type[0])
                    .GetCustomAttributes(typeof(WellDistributedHashAttribute), false).Length == 1;
            var imap = type.GetInterfaceMap(typeof(IEqualityComparer<T>));
            int idx = imap.InterfaceMethods[0].ReturnType == typeof(int) ? 0 : 1;
            return imap.TargetMethods[idx].GetCustomAttributes(typeof(WellDistributedHashAttribute), false).Length != 0;
        }
    }
}
