// WellDistributedHashAttribute.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014–2017 Jon Hanna
//
// Licensed under the MIT license. See the LICENSE file in the repository root for more details.

using System;
using System.Collections.Generic;

namespace SpookilySharp
{
    /// <summary>Marks an implementation of
    /// <see cref="M:System.Collections.Generic.IEqualityComparer`1.GetHashCode(`0)"/> or an override of
    /// <see cref="object.GetHashCode()"/> as being known to distribute bits well in its implementation of hash
    /// codes.</summary>
    /// <remarks>When such a method is used by an <see cref="IEqualityComparer{T}"/>, whether as the implementation of
    /// that interface, of because it is used by <see cref="EqualityComparer{T}.Default"/>, then passing it to
    /// <see cref="SpookierEqualityComparers.WellDistributed{T}(IEqualityComparer{T})"/> will return it again, as it can
    /// add nothing more in the way of distribution, and will just be wasteful and if anything cause more
    /// collisions.</remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class WellDistributedHashAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:SpookilySharp.WellDistributedHashAttribute" /> class.</summary>
        public WellDistributedHashAttribute()
        {
        }
    }
}