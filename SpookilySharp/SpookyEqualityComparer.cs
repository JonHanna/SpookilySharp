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
    /// SpookyHash for its hash codes./>.
    /// </summary>
    public class SpookyEqualityComparer : IEqualityComparer<char[]>, IEqualityComparer<string>
    {
        private readonly int _seed;
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
        /// <param name="randomizeSeed">If set to <c>true</c> the seed is taken from the number of milliseconds the
        /// system has been up, otherwise the default seed is used.</param>
        /// <remarks>This is useful in reducing Hash DoS attacks, though different instances of the comparer will not
        /// operate together, as each will produce a different hash code.</remarks>
        public SpookyEqualityComparer(bool randomizeSeed)
            : this(randomizeSeed ? Environment.TickCount : unchecked((int)SpookyHash.SpookyConst))
        {
        }
        /// <returns>The 32-bit signed SpookyHash hash code, or zero if the array is null.</returns>
        /// <summary>Returns a 32-bit signed SpookyHash hash code for the specified array.</summary>
        /// <param name="obj">The array of <see cref="char"/> to hash.</param>
        public int GetHashCode(char[] obj)
        {
            return obj.SpookyHash32(_seed);
        }
        /// <returns>The 32-bit signed SpookyHash hash code, or zero if the string is null.</returns>
        /// <summary>Returns a 32-bit signed SpookyHash hash code for the specified string.</summary>
        /// <param name="obj">The <see cref="string"/> to hash.</param>
        public int GetHashCode(string obj)
        {
            return obj.SpookyHash32(_seed);
        }
        /// <summary>Returns true if the two arrays are identical</summary>
        /// <param name="x">The first array to compare.</param>
        /// <param name="y">The second array to compare.</param>
        /// <returns>True if the two arrays are identical, false otherwise.</returns>
        public bool Equals(char[] x, char[] y)
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
        /// <summary>Returns true if the two <see cref="string"/>s are identical</summary>
        /// <param name="x">The first <see cref="string"/> to compare.</param>
        /// <param name="y">The second <see cref="string"/> to compare.</param>
        /// <returns>True if the two <see cref="string"/>s are identical, false otherwise.</returns>
        public bool Equals(string x, string y)
        {
            return x == y;
        }
        /// <summary>Returns true if the string and array of characters contain the same sequence of characters,
        /// or both are null.</summary>
        /// <param name="x">The string to compare.</param>
        /// <param name="y">The array to cmpare.</param>
        /// <returns>True if the two sequences of characters are identical, or both are null, false otherwise.</returns>
        public bool Equals(string x, char[] y)
        {
            if(x == null)
                return y == null;
            if(y == null)
                return false;
            if(x.Length != y.Length)
                return false;
            for(int i = 0; i != x.Length; ++i)
                if(x[i] != y[i])
                    return false;
            return true;
        }
        /// <summary>Returns true if the string and array of characters contain the same sequence of characters,
        /// or both are null.</summary>
        /// <param name="x">The array to cmpare.</param>
        /// <param name="y">The string to compare.</param>
        /// <returns>True if the two sequences of characters are identical, or both are null, false otherwise.</returns>
        public bool Equals(char[] y, string x)
        {
            if(x == null)
                return y == null;
            if(y == null)
                return false;
            if(x.Length != y.Length)
                return false;
            for(int i = 0; i != x.Length; ++i)
                if(x[i] != y[i])
                    return false;
            return true;
        }
    }
}