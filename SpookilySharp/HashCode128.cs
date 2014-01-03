// HashCode128.cs
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
using System.Globalization;

namespace SpookilySharp
{
    /// <summary>
    /// Represents a 128-bit hash code.
    /// </summary>
    public struct HashCode128 : IEquatable<HashCode128>
    {
        /// <summary>
        /// A <see cref="HashCode128"/> that is all-zero. This is the same as the default value.
        /// </summary>
        public static readonly HashCode128 Zero = new HashCode128(0, 0);
        /// <summary>
        /// Tries to parse a <see cref="HashCode128"/> from a string.
        /// </summary>
        /// <returns><c>true</c>, if <paramref name="s"/> was converted successfully; otherwise <c>false</c>.</returns>
        /// <param name="s">A <see cref="string"/> containing the hash code to convert.</param>
        /// <param name="result">The 128-bit has code parsed from the string, or <see cref="HashCode128.Zero"/> if
        /// the parsing was unsuccessful.</param>
        /// <remarks>The value passed to <paramref name="s"/> must be a 16-digit hexadecimal number for this to succeed.
        /// Leading and trailing whitespace is allowed. Leading zeros must not be omitted.</remarks>
        public static bool TryParse(string s, out HashCode128 result)
        {
            if(s != null)
            {
                s = s.Trim();
                if(s.Length == 16)
                {
                    ulong hash1;
                    if(ulong.TryParse(s.Substring(0, 16), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hash1))
                    {
                        ulong hash2;
                        if(ulong.TryParse(s.Substring(16, 16), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hash2))
                        {
                            result = new HashCode128(hash1, hash2);
                            return true;
                        }
                    }
                }
            }
            result = default(HashCode128);
            return false;
        }
        /// <summary>
        /// Produces a <see cref="HashCode128"/> from a string containing a 16-digit hexadecimal number. Leading and
        /// trailing whitespace is allowed.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="HashCode128"/> represented by <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> was null.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> did not contain a 16-digit hexadecimal number.</exception> 
        public static HashCode128 Parse(string s)
        {
            if(s == null)
                throw new ArgumentNullException("s");
            HashCode128 ret;
            if(!TryParse(s, out ret))
                throw new FormatException("The string did not contain a 16-digit hexadecimal number.");
            return ret;
        }
        private readonly ulong _hash1;
        private readonly ulong _hash2;
        /// <summary>
        /// Initializes a new instance of the <see cref="HashCode128"/> struct.
        /// </summary>
        /// <param name="hash1">The first 64 bits of the hash code.</param>
        /// <param name="hash2">The second 64 bits of the hash code.</param>
        [CLSCompliant(false)]
        public HashCode128(ulong hash1, ulong hash2)
        {
            _hash1 = hash1;
            _hash2 = hash2;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HashCode128"/> struct.
        /// </summary>
        /// <param name="hash1">The first 64 bits of the hash code.</param>
        /// <param name="hash2">The second 64 bits of the hash code.</param>
        public HashCode128(long hash1, long hash2)
            :this(unchecked((ulong)hash1), unchecked((ulong)hash2))
        {
        }
        /// <summary>
        /// The first 64 bits of the hash code, as a <see cref="long"/>.
        /// </summary>
        public long Hash1
        {
            get { return unchecked((long)_hash1); }
        }
        /// <summary>
        /// The second 64 bits of the hash code, as a <see cref="long"/>.
        /// </summary>
        public long Hash2
        {
            get { return unchecked((long)_hash2); }
        }
        /// <summary>
        /// The first 64 bits of the hash code, as a <see cref="ulong"/>.
        /// </summary>
        [CLSCompliant(false)]
        public ulong UHash1
        {
            get { return _hash1; }
        }
        /// <summary>
        /// The second 64 bits of the hash code, as a <see cref="ulong"/>.
        /// </summary>
        [CLSCompliant(false)]
        public ulong UHash2
        {
            get { return _hash2; }
        }
        /// <summary>
        /// Determines whether the specified <see cref="HashCode128"/> is equal to the current <see cref="HashCode128"/>.
        /// </summary>
        /// <param name="other">The <see cref="SpookilySharp.HashCode128"/> to compare with the current <see cref="SpookilySharp.HashCode128"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="SpookilySharp.HashCode128"/> is equal to the current
        /// <see cref="HashCode128"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(HashCode128 other)
        {
            return _hash1 == other._hash1 && _hash2 == other._hash2;
        }
        /// <summary>
        /// Serves as a hash function for a <see cref="SpookilySharp.HashCode128"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        /// <remarks>This code is the same as that which would have been returned by the 32-bit hashing methods.</remarks>
        public override int GetHashCode()
        {
            return unchecked((int)_hash1);
        }
        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="HashCode128"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="HashCode128"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is a boxed <see cref="HashCode128"/> with the
        /// same value as the current; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if(obj is HashCode128)
                return Equals((HashCode128)obj);
            return false;
        }
        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="HashCode128"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the hash code as a 16-digit hexadecimal number.</returns>
        public override string ToString()
        {
            // Analysis disable once FormatStringProblem
            return _hash1.ToString("X8") + _hash2.ToString("X8");
        }
    }	
}