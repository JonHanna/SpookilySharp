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

using System.Collections.Generic;

namespace SpookilySharp
{
    /// <summary>
    /// Marks an implementaiton of <see cref="IEqualityComparer{T}"/> as being known to distribute bits well in its
    /// implementation of hash codes. As such, passing it to
    /// <see cref="SpookierEqualityComparers.WellDistributed{T}(IEqualityComparer{T})"/> will just return it again, as
    /// it can add nothing more in the way of distribution, and will just be wasteful and if anything cause more
    /// collisions.
    /// </summary>
    public interface IWellDistributedEqualityComparer<T> : IEqualityComparer<T>
    {
    }
    internal class WellDistributedEqualityComparer<T> : IWellDistributedEqualityComparer<T>
    {
        private readonly IEqualityComparer<T> _cmp;
        internal WellDistributedEqualityComparer(IEqualityComparer<T> cmp)
        {
            _cmp = cmp;
        }
        public bool Equals(T x, T y)
        {
            return _cmp.Equals(x, y);
        }
        public int GetHashCode(T obj)
        {
            // Analysis disable once CompareNonConstrainedGenericWithNull
            return obj == null ? 0 : _cmp.GetHashCode(obj).ReHash();
        }
    }
    /// <summary>
    /// Improves the bit distribution of equality comparers.
    /// </summary>
    public static class SpookierEqualityComparers
    {
        /// <summary>
        /// Returns a version of <paramref name="cmp"/> that provides strong distribution of bits in its hash codes.
        /// </summary>
        /// <returns>An <see cref="IWellDistributedEqualityComparer{T}"/> based on <paramref name="cmp"/>, or
        /// <paramref name="cmp"/> if it already implements <see cref="IWellDistributedEqualityComparer{T}"/>.</returns>
        /// <param name="cmp">The <see cref="IEqualityComparer{T}"/> to improve.</param>
        /// <remarks>This cannot improve the overall risk of collision (indeed, will
        /// make it slightly worse), it can help when uses of hash codes are particularly sensitive to collisions in the one
        /// section of bits, e.g. with power-of-two hash tables</remarks>
        public static IWellDistributedEqualityComparer<T> WellDistributed<T>(this IEqualityComparer<T> cmp)
        {
            return cmp as IWellDistributedEqualityComparer<T> ?? new WellDistributedEqualityComparer<T>(cmp);
        }
    }
}