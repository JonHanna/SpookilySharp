// WellDistributedHashAttribute.cs
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

// Analysis disable once RedundantUsingDirective used by xml doc.
// Analysis disable EmptyConstructor otherwise inherit en-US documentation.
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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class WellDistributedHashAttribute : Attribute
    {
        /// <summary>Initialises a new instance of the <see cref="WellDistributedHashAttribute"/> class.</summary>
        public WellDistributedHashAttribute()
        {
        }
    }
}