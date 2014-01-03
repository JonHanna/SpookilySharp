// SpookyEqualityComparer.cs
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
using System.Collections.Generic;
using System.Security;

namespace SpookilySharp
{
    /// <summary>
    /// An equality comparer for <see cref="string"/>s and <see cref="char"/> arrays that uses
    /// SpookyHash for its <see cref="IEqualityComparer{T}.GetHashCode()"/>.
    /// </summary>
    public class SpookyEqualityComparer : IEqualityComparer<char[]>, IEqualityComparer<string>
    {
        private int _seed;
        /// <summary>
        /// Initializes a new instance of the <see cref="SpookyEqualityComparer"/> class with a default seed.
        /// </summary>
        public SpookyEqualityComparer()
            :this(unchecked((int)SpookyHash.SpookyConst))
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SpookyEqualityComparer"/> class with a given seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <remarks>Instances with different seeds will produces different hash codes for the same item. This can be
        /// useful in preventing Hash DoS attacks.</remarks>
        public SpookyEqualityComparer(int seed)
        {
            _seed = seed;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SpookyEqualityComparer"/> class with a default or randomised
        /// seed.
        /// </summary>
        /// <param name="randomiseSeed">If set to <c>true</c> the seed is taken from the number of milliseconds the
        /// system has been up, otherwise the default seed is used.</param>
        /// <remarks>This is useful in reducing Hash DoS attacks, though different instances of the comparer will not
        /// operate together, as each will produce a different hash code.</remarks>
        public SpookyEqualityComparer(bool randomizeSeed)
            : this(randomiseSeed ? Environment.TickCount : unchecked((int)SpookyHash.SpookyConst))
        {
        }
        public int GetHashCode(char[] obj)
        {
            return obj.SpookyHash32(_seed);
        }
        public int GetHashCode(string obj)
        {
            return obj.SpookyHash32(_seed);
        }
        public unsafe bool Equals(char[] x, char[] y)
        {
            if(x == y)
                return true;
            if(x == null || y == null)
                return false;
            if(x.Length != y.Length)
                return false;
            for(int i = 0; i != x.Length; ++i)//TODO: More efficient compare
                if(x [i] != y [i])
                    return false;
            return true;
        }
        public bool Equals(string x, string y)
        {
            return x == y;
        }
        public bool Equals(string x, char[] y)
        {
            if(x.Length != y.Length)
                return false;
        }
        public bool Equals(char[] y, string x)
        {
        }
    }
}