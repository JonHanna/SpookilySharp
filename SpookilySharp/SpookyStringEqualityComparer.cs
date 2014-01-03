// SpookyStringEqualityComparer.cs
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
using System.Runtime.CompilerServices;
using System.Security;

namespace SpookilySharp
{
    /// <summary>
    /// An equality comparer for <see cref="string"/>s and <see cref="char"/> arrays that uses
    /// SpookyHash for its hash codes./>.
    /// </summary>
    public class SpookyStringEqualityComparer
        : IWellDistributedEqualityComparer<char[]>, IWellDistributedEqualityComparer<string>,
        IEquatable<SpookyStringEqualityComparer>
    {
        private readonly int _seed;
        /// <summary>
        /// Initializes a new instance of the <see cref="SpookyStringEqualityComparer"/> class with a default seed.
        /// </summary>
        public SpookyStringEqualityComparer()
            :this(unchecked((int)SpookyHash.SpookyConst))
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SpookyStringEqualityComparer"/> class with a given seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <remarks>Instances with different seeds will produces different hash codes for the same item. This can be
        /// useful in preventing Hash DoS attacks.</remarks>
        public SpookyStringEqualityComparer(int seed)
        {
            _seed = seed;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SpookyStringEqualityComparer"/> class with a default or randomised
        /// seed.
        /// </summary>
        /// <param name="randomizeSeed">If set to <c>true</c> the seed is taken from the number of milliseconds the
        /// system has been up, otherwise the default seed is used.</param>
        /// <remarks>This is useful in reducing Hash DoS attacks, though different instances of the comparer will not
        /// operate together, as each will produce a different hash code.</remarks>
        public SpookyStringEqualityComparer(bool randomizeSeed)
            : this(randomizeSeed ? Environment.TickCount : unchecked((int)SpookyHash.SpookyConst))
        {
        }
        /// <returns>The 32-bit signed SpookyHash hash code, or zero if the array is null.</returns>
        /// <summary>Returns a 32-bit signed SpookyHash hash code for the specified array.</summary>
        /// <param name="obj">The array of <see cref="char"/> to hash.</param>
        public int GetHashCode(char[] obj)
        {
            return obj == null ? 0 : obj.SpookyHash32(_seed);
        }
        /// <returns>The 32-bit signed SpookyHash hash code, or zero if the string is null.</returns>
        /// <summary>Returns a 32-bit signed SpookyHash hash code for the specified string.</summary>
        /// <param name="obj">The <see cref="string"/> to hash.</param>
        public int GetHashCode(string obj)
        {
            return obj == null ? 0 : obj.SpookyHash32(_seed);
        }
        /// <summary>Returns true if the two arrays are identical</summary>
        /// <param name="x">The first array to compare.</param>
        /// <param name="y">The second array to compare.</param>
        /// <returns>True if the two arrays are identical, false otherwise.</returns>
        [SecuritySafeCritical]
        public unsafe bool Equals(char[] x, char[] y)
        {
            if(x == y)
                return true;
            if(x == null || y == null)
                return false;
            int len = x.Length;
            if(y.Length != len)
                return false;
            if(len == 0)
                return true;
            fixed(char* px = x)
                fixed(char* py = y)
                    return Equals(px, py, len);
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
        [SecuritySafeCritical]
        public unsafe bool Equals(string x, char[] y)
        {
            if(x == null)
                return y == null;
            if(y == null)
                return false;
            int len = x.Length;
            if(y.Length != len)
                return false;
            if(len == 0)
                return true;
            fixed(char* px = x + RuntimeHelpers.OffsetToStringData)
                fixed(char* py = y)
                    return Equals(px, py, len);
        }
        /// <summary>Returns true if the string and array of characters contain the same sequence of characters,
        /// or both are null.</summary>
        /// <param name="x">The array to cmpare.</param>
        /// <param name="y">The string to compare.</param>
        /// <returns>True if the two sequences of characters are identical, or both are null, false otherwise.</returns>
        [SecuritySafeCritical]
        public unsafe bool Equals(char[] x, string y)
        {
            if(x == null)
                return y == null;
            if(y == null)
                return false;
            int len = x.Length;
            if(y.Length != len)
                return false;
            if(len == 0)
                return true;
            fixed(char* px = x)
                fixed(char* py = y + RuntimeHelpers.OffsetToStringData)
                    return Equals(px, py, len);
        }
        [SecurityCritical]
        private static unsafe bool Equals(char* pX, char* pY, int length)
        {
            var ipX = (int*)pX;
            var ipY = (int*)pY;
            if((length & 1) != 0)
            {
                int move = length & ~1;
                if(*(pX + move) != *(pY + move))
                    return false;
            }
            int intLen = length >> 1;
            if(intLen == 0)
                return true;
            switch(intLen & 15)
            {
                case 0:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 15;
                case 15:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 14;
                case 14:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 13;
                case 13:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 12;
                case 12:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 11;
                case 11:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 10;
                case 10:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 9;
                case 9:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 8;
                case 8:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 7;
                case 7:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 6;
                case 6:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 5;
                case 5:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 4;
                case 4:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 3;
                case 3:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 2;
                case 2:
                    if(*ipX++ != *ipY++)
                        return false;
                    goto case 1;
                case 1:
                    if(*ipX++ != *ipY++)
                        return false;
                    if((intLen -= 16) > 0)
                        goto case 0;
                    break;
            }
            return true;
        }
        /// <summary>
        /// Determines whether the specified <see cref="SpookyStringEqualityComparer"/> is equal to the current <see cref="SpookyStringEqualityComparer"/>.
        /// </summary>
        /// <param name="other">The <see cref="SpookyStringEqualityComparer"/> to compare with the current <see cref="SpookilySharp.SpookyStringEqualityComparer"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="SpookyStringEqualityComparer"/> is equal to the current
        /// <see cref="SpookyStringEqualityComparer"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>Two instances of <see cref="SpookyStringEqualityComparer"/> are considered equal if they have the same seed,
        /// and as such can be depended upon to produce the same hash codes for the same input.</remarks>
        public bool Equals(SpookyStringEqualityComparer other)
        {
            return other != null && other._seed == _seed;
        }
        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="SpookyStringEqualityComparer"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="SpookyStringEqualityComparer"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is a <see cref="SpookyStringEqualityComparer"/> equal to the current
        /// <see cref="SpookyStringEqualityComparer"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SpookyStringEqualityComparer);
        }
        /// <summary>
        /// Serves as a hash function for a <see cref="SpookyStringEqualityComparer"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return _seed;
        }
    }
}