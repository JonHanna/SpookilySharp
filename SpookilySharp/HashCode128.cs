﻿// HashCode128.cs
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
using System.Diagnostics.CodeAnalysis;

namespace SpookilySharp
{
    /// <summary>Represents a 128-bit hash code.</summary>
    [Serializable]
    public struct HashCode128 : IEquatable<HashCode128>
    {
        /// <summary>A <see cref="HashCode128"/> that is all-zero. This is the same as the default value.</summary>
        public static readonly HashCode128 Zero = default(HashCode128);

        private readonly ulong _hash1;
        private readonly ulong _hash2;

        /// <summary>Initialises a new instance of the <see cref="HashCode128"/> struct.</summary>
        /// <param name="hash1">The first 64 bits of the hash code.</param>
        /// <param name="hash2">The second 64 bits of the hash code.</param>
        [CLSCompliant(false)]
        public HashCode128(ulong hash1, ulong hash2)
        {
            _hash1 = hash1;
            _hash2 = hash2;
        }

        /// <summary>Initialises a new instance of the <see cref="HashCode128"/> struct.</summary>
        /// <param name="hash1">The first 64 bits of the hash code.</param>
        /// <param name="hash2">The second 64 bits of the hash code.</param>
        public HashCode128(long hash1, long hash2)
            : this(unchecked((ulong)hash1), unchecked((ulong)hash2))
        {
        }

        /// <summary>Gets the first 64 bits of the hash code, as a <see cref="long"/>.</summary>
        /// <value>The first 64 bits of the hash code.</value>
        public long Hash1
        {
            get { return unchecked((long)_hash1); }
        }

        /// <summary>Gets the second 64 bits of the hash code, as a <see cref="long"/>.</summary>
        /// <value>The second 64 bits of the hash code.</value>
        public long Hash2
        {
            get { return unchecked((long)_hash2); }
        }

        /// <summary>Gets the first 64 bits of the hash code, as a <see cref="ulong"/>.</summary>
        /// <value>The first 64 bits of the hash code.</value>
        [CLSCompliant(false)]
        public ulong UHash1
        {
            get { return _hash1; }
        }

        /// <summary>Gets the second 64 bits of the hash code, as a <see cref="ulong"/>.</summary>
        /// <value>The second 64 bits of the hash code.</value>
        [CLSCompliant(false)]
        public ulong UHash2
        {
            get { return _hash2; }
        }

        /// <summary>Tries to parse a <see cref="HashCode128"/> from a string.</summary>
        /// <returns><see langword="true"/>, if <paramref name="s"/> was converted successfully; otherwise
        /// <see langword="false"/>.</returns>
        /// <param name="s">A <see cref="string"/> containing the hash code to convert.</param>
        /// <param name="result">The 128-bit has code parsed from the string, or <see cref="HashCode128.Zero"/> if
        /// the parsing was unsuccessful.</param>
        /// <remarks>The value passed to <paramref name="s"/> must be a 32-digit hexadecimal number for this to succeed.
        /// Leading, trailing and contained whitespace is allowed. A leading <c>0x</c> is permitted, but not required.
        /// Leading zeros must not be omitted.</remarks>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1107:CodeMustNotContainMultipleStatementsOnOneLine",
            Justification = "More readable with multiple case statements.")]
        public static bool TryParse(string s, out HashCode128 result)
        {
            if(s != null)
            {
                int idx = 0;
                bool wasXSeen = false;
                int len = s.Length;

                // parse as we go rather than trimming and using ulong.Parse() so we don't spend forever
                // trimming a long string.
                if(len >= 32)
                {
                    ulong first = 0;
                    int stringIndex;
                    for(stringIndex = 0; stringIndex != len; ++stringIndex)
                    {
                        char c = s[stringIndex];
                        if(!char.IsWhiteSpace(c))
                        {
                            switch(c)
                            {
                                case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7':
                                case '8': case '9':
                                    first = (first << 4) + (ulong)c - (ulong)'0';
                                    break;
                                case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                                    first = (first << 4) + (ulong)c - ((ulong)'a' - 0xA);
                                    break;
                                case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                                    first = (first << 4) + (ulong)c - ((ulong)'A' - 0xA);
                                    break;
                                case 'X': case 'x':
                                    if(idx == 1 && first == 0 && !wasXSeen)
                                    {
                                        idx = 0;
                                        wasXSeen = true;
                                        continue;
                                    }
                                    goto fail;
                                default:
                                    goto fail;
                            }
                            if(++idx == 16)
                                break;
                        }
                    }
                    if(idx == 16)
                    {
                        ulong second = 0;
                        for(++stringIndex; stringIndex != len; ++stringIndex)
                        {
                            char c = s[stringIndex];
                            if(!char.IsWhiteSpace(c))
                            {
                                switch(c)
                                {
                                    case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7':
                                    case '8': case '9':
                                        second = (second << 4) + (ulong)c - (ulong)'0';
                                        break;
                                    case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                                        second = (second << 4) + (ulong)c - ((ulong)'a' - 0xA);
                                        break;
                                    case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                                        second = (second << 4) + (ulong)c - ((ulong)'A' - 0xA);
                                        break;
                                    default:
                                        goto fail;
                                }
                                if(++idx == 32)
                                {
                                    for(++stringIndex; stringIndex != len; ++stringIndex)
                                        if(!char.IsWhiteSpace(s[stringIndex]))
                                            goto fail;
                                    result = new HashCode128(first, second);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        fail:
            result = default(HashCode128);
            return false;
        }

        /// <summary>Produces a <see cref="HashCode128"/> from a string containing a 32-digit hexadecimal number.
        /// Leading and trailing whitespace is allowed.</summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="HashCode128"/> represented by <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> was null.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> did not contain a 32-digit hexadecimal
        /// number.</exception>
        /// <remarks>The value passed to <paramref name="s"/> must be a 32-digit hexadecimal number for this to succeed.
        /// Leading, trailing and contained whitespace is allowed. A leading <c>0x</c> is permitted, but not required.
        /// Leading zeros must not be omitted.</remarks>
        public static HashCode128 Parse(string s)
        {
            ExceptionHelper.CheckNotNullString(s);
            HashCode128 ret;
            if(!TryParse(s, out ret))
                ExceptionHelper.BadHashCode128Format();
            return ret;
        }

        /// <summary>Determines whether two <see cref="HashCode128"/> instances are equal.</summary>
        /// <param name="x">The first <see cref="HashCode128"/> instance to compare.</param>
        /// <param name="y">The second <see cref="HashCode128"/> instance to compare.</param>
        /// <returns><see langword="true"/> if the two <see cref="HashCode128"/> instances are equal; otherwise,
        /// <see langword="false"/>.</returns>
        public static bool operator ==(HashCode128 x, HashCode128 y)
        {
            return x.Equals(y);
        }

        /// <summary>Determines whether two <see cref="HashCode128"/> instances are different.</summary>
        /// <param name="x">The first <see cref="HashCode128"/> instance to compare.</param>
        /// <param name="y">The second <see cref="HashCode128"/> instance to compare.</param>
        /// <returns><see langword="true"/> if the two <see cref="HashCode128"/> instances are different; otherwise,
        /// <see langword="false"/>.</returns>
        public static bool operator !=(HashCode128 x, HashCode128 y)
        {
            return !x.Equals(y);
        }

        /// <inheritdoc/>
        /// <remarks>Considers itself equal to an equal <see cref="HashCode128"/> instance, and to nothing
        /// else.</remarks>
        public bool Equals(HashCode128 other)
        {
            return _hash1 == other._hash1 && _hash2 == other._hash2;
        }

        /// <inheritdoc/>
        [WellDistributedHash] // ironically not really true if you create this struct any way other than as the result
                              // of a good hash operation in the first place.
        public override int GetHashCode()
        {
            return unchecked((int)_hash1);
        }

        /// <summary>Determines whether the specified <see cref="object"/> is equal to the current
        /// <see cref="HashCode128"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="HashCode128"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="object"/> is a boxed <see cref="HashCode128"/>
        /// with the same value as the current; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if(obj is HashCode128)
                return Equals((HashCode128)obj);
            return false;
        }

        /// <summary>Returns a <see cref="string"/> that represents the current <see cref="HashCode128"/>.</summary>
        /// <returns>A <see cref="string"/> that represents the hash code as a 32-digit hexadecimal number.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            Justification = "Irrelevant to formatstring in question.")]
        public override string ToString()
        {
            return _hash1.ToString("X16") + _hash2.ToString("X16");
        }
    }
}