// SpookyHasher.cs
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
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
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
            return _hash1.ToString("X8", CultureInfo.InvariantCulture) + _hash2.ToString("X8", CultureInfo.InvariantCulture);
        }
    }
	/// <summary>
	/// Provides static extension methods for producing SpookyHashes of various types of object.
	/// <threadsafety static="true"/>
	/// </summary>
	public static class SpookyHasher
	{
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of a <see cref="string"/>.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the 128-bit hash.</returns>
	    /// <param name="str">The <see cref="string"/> to hash.</param>
	    /// <param name="seed0">The first 64-bits of the seed value.</param>
	    /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
	    [SecuritySafeCritical]
        public unsafe static HashCode128 SpookyHash128(this string str, long seed0, long seed1)
	    {
            if(str == null)
                return default(HashCode128);
	        ulong hash1 = (ulong)seed0;
	        ulong hash2 = (ulong)seed1;
	        fixed(char* ptr = str + RuntimeHelpers.OffsetToStringData)
	            SpookyHash.Hash128(ptr, str.Length, ref hash1, ref hash2);
            return new HashCode128(hash1, hash2);
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of a <see cref="string"/>, using a default seed.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="str">The <see cref="string"/> to hash.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        public static HashCode128 SpookyHash128(this string str)
	    {
	        return unchecked(SpookyHash128(str, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of a <see cref="string"/>.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="str">The <see cref="string"/> to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <remarks>For a null string, the hash will be zero.</remarks>
	    [SecuritySafeCritical]
	    public unsafe static long SpookyHash64(this string str, long seed)
	    {
	        if(str == null)
	            return 0L;
	        ulong hash = (ulong)seed;
	        fixed(char* ptr = str + RuntimeHelpers.OffsetToStringData)
	            return (long)SpookyHash.Hash64(ptr, str.Length, hash);
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of a <see cref="string"/>, using a default seed.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="str">The <see cref="string"/> to hash.</param>
	    /// <remarks>For a null string, the hash will be zero.</remarks>
	    public static long SpookyHash64(this string str)
	    {
	        return unchecked(SpookyHash64(str, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of a <see cref="string"/>.
	    /// </summary>
	    /// <returns>A <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="str">The <see cref="string"/> to hash.</param>
	    /// <param name="seed">The 32-bit seed value.</param>
	    /// <remarks>For a null string, the hash will be zero.</remarks>
	    [SecuritySafeCritical]
	    public unsafe static int SpookyHash32(this string str, int seed)
	    {
	        if(str == null)
	            return 0;
	        uint hash = (uint)seed;
	        fixed(char* ptr = str + RuntimeHelpers.OffsetToStringData)
	            return (int)SpookyHash.Hash32(ptr, str.Length, hash);
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of a <see cref="string"/>, using a default seed.
	    /// </summary>
	    /// <returns>A <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="str">The <see cref="string"/> to hash.</param>
	    /// <remarks>For a null string, the hash will be zero.</remarks>
	    public static int SpookyHash32(this string str)
	    {
	        return unchecked(SpookyHash32(str, (int)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of characters.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of characters to hash.</param>
	    /// <param name="seed0">The first 64-bits of the seed value.</param>
	    /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    [SecuritySafeCritical]
        public unsafe static HashCode128 SpookyHash128(this char[] message, int startIndex, int length, long seed0, long seed1)
	    {
	        if(message == null)
                return default(HashCode128);
	        if(startIndex < 0 || startIndex > message.Length)
	            throw new ArgumentOutOfRangeException("startIndex");
	        if(startIndex + length > message.Length)
	            throw new ArgumentException();
	        ulong hash1 = (ulong)seed0;
	        ulong hash2 = (ulong)seed1;
	        fixed(char* ptr = message)
	            SpookyHash.Hash128(ptr + startIndex, length * 2, ref hash1, ref hash2);
            return new HashCode128(hash1, hash2);
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of characters, using a default seed.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of characters to hash.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
        public static HashCode128 SpookyHash128(this char[] message, int startIndex, int length)
	    {
	        return unchecked(SpookyHash128(message, startIndex, length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of characters.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of characters to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    [SecuritySafeCritical]
	    public unsafe static long SpookyHash64(this char[] message, int startIndex, int length, long seed)
	    {
	        if(message == null)
	            return 0;
	        if(startIndex < 0 || startIndex > message.Length)
	            throw new ArgumentOutOfRangeException("startIndex");
	        if(startIndex + length > message.Length)
	            throw new ArgumentException();
	        ulong hash = (ulong)seed;
	        fixed(char* ptr = message)
	            return (long)SpookyHash.Hash64(ptr + startIndex, length * 2, hash);
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of characters, with a default seed.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of characters to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    public static long SpookyHash64(this char[] message, int startIndex, int length)
	    {
	        return unchecked(SpookyHash64(message, startIndex, length, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of characters.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of characters to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    [SecuritySafeCritical]
	    public unsafe static int SpookyHash32(this char[] message, int startIndex, int length, int seed)
	    {
	        if(message == null)
	            return 0;
	        if(startIndex < 0 || startIndex > message.Length)
	            throw new ArgumentOutOfRangeException("startIndex");
	        if(startIndex + length > message.Length)
	            throw new ArgumentException();
	        uint hash = (uint)seed;
	        fixed(char* ptr = message)
	            return (int)SpookyHash.Hash32(ptr + startIndex, length * 2, hash);
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of characters, with a default seed.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of characters to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    public static int SpookyHash32(this char[] message, int startIndex, int length)
	    {
	        return unchecked(SpookyHash32(message, startIndex, length, (int)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of characters.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="seed0">The first 64-bits of the seed value.</param>
	    /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        public static HashCode128 SpookyHash128(this char[] message, long seed0, long seed1)
	    {
	        return SpookyHash128(message, 0, message.Length, seed0, seed1);
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of characters, with a default seed.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        public static HashCode128 SpookyHash128(this char[] message)
	    {
	        return unchecked(SpookyHash128(message, 0, message.Length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of characters.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static long SpookyHash64(this char[] message, long seed)
	    {
	        return SpookyHash64(message, 0, 0, seed);
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of characters, with a default seed.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static long SpookyHash64(this char[] message)
	    {
	        return unchecked(SpookyHash64(message, 0, message.Length, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of characters.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static int SpookyHash32(this char[] message, int seed)
	    {
	        return SpookyHash32(message, 0, message.Length, seed);
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of characters, with a default seed.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static int SpookyHash32(this char[] message)
	    {
	        return unchecked(SpookyHash32(message, 0, message.Length, (int)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of bytes
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of bytes to hash.</param>
	    /// <param name="seed0">The first 64-bits of the seed value.</param>
	    /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    [SecuritySafeCritical]
        public unsafe static HashCode128 SpookyHash128(this byte[] message, int startIndex, int length, long seed0, long seed1)
	    {
            if(message == null)
                return default(HashCode128);
	        if(startIndex < 0 || startIndex > message.Length)
	            throw new ArgumentOutOfRangeException("startIndex");
	        if(startIndex + length > message.Length)
	            throw new ArgumentException();
	        ulong hash1 = (ulong)seed0;
	        ulong hash2 = (ulong)seed1;
	        fixed(byte* ptr = message)
	            SpookyHash.Hash128(ptr + startIndex, length, ref hash1, ref hash2);
            return new HashCode128(hash1, hash2);
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of bytes, with a default seed.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of bytes to hash.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
        public static HashCode128 SpookyHash128(this byte[] message, int startIndex, int length)
	    {
	        return unchecked(SpookyHash128(message, startIndex, length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of bytes
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the two 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of bytes to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    [SecuritySafeCritical]
	    public unsafe static long SpookyHash64(this byte[] message, int startIndex, int length, long seed)
	    {
	        if(message == null)
	            return 0;
	        if(startIndex < 0 || startIndex > message.Length)
	            throw new ArgumentOutOfRangeException("startIndex");
	        if(startIndex + length > message.Length)
	            throw new ArgumentException();
	        ulong hash = (ulong)seed;
	        fixed(byte* ptr = message)
	            return (long)SpookyHash.Hash64(ptr + startIndex, length, hash);
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of bytes, with a default seed.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of bytes to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    public static long SpookyHash64(this byte[] message, int startIndex, int length)
	    {
	        return unchecked(SpookyHash64(message, startIndex, length, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of bytes.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of bytes to hash.</param>
	    /// <param name="seed">The 32-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    [SecuritySafeCritical]
	    public unsafe static int SpookyHash32(this byte[] message, int startIndex, int length, int seed)
	    {
	        if(message == null)
	            return 0;
	        if(startIndex < 0 || startIndex > message.Length)
	            throw new ArgumentOutOfRangeException("startIndex");
	        if(startIndex + length > message.Length)
	            throw new ArgumentException();
	        uint hash = (uint)seed;
	        fixed(byte* ptr = message)
	            return (int)SpookyHash.Hash32(ptr + startIndex, length, hash);
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of bytes, with a default seed.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="startIndex">The index from which to hash.</param>
	    /// <param name="length">The number of bytes to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater than the length of the array.</exception>
	    /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of the array.</exception>
	    public static int SpookyHash32(this byte[] message, int startIndex, int length)
	    {
	        return unchecked(SpookyHash32(message, startIndex, length, (int)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of bytes
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="seed0">The first 64-bits of the seed value.</param>
	    /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        public static HashCode128 SpookyHash128(this byte[] message, long seed0, long seed1)
	    {
	        return SpookyHash128(message, 0, message.Length, seed0, seed1);
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of an array of bytes, with a default seed.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
        /// <remarks>For a null array, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        public static HashCode128 SpookyHash128(this byte[] message)
	    {
	        return unchecked(SpookyHash128(message, 0, message.Length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of bytes.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static long SpookyHash64(this byte[] message, long seed)
	    {
	        return SpookyHash64(message, 0, 0, seed);
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of an array of bytes, with a default seed.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static long SpookyHash64(this byte[] message)
	    {
	        return unchecked(SpookyHash64(message, 0, message.Length, (int)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of bytes.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <param name="seed">The 32-bit seed value.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static int SpookyHash32(this byte[] message, int seed)
	    {
	        return SpookyHash32(message, 0, message.Length, seed);
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of an array of bytes, with a default seed.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
	    /// <param name="message">The array to hash.</param>
	    /// <remarks>For a null array, the hash will be zero.</remarks>
	    public static int SpookyHash32(this byte[] message)
	    {
	        return unchecked(SpookyHash32(message, 0, message.Length, (int)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of a stream
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="stream">The stream to hash.</param>
	    /// <param name="seed0">The first 64-bits of the seed value.</param>
	    /// <param name="seed1">The second 64-bits of the seed value.</param>
	    /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
	    [SecuritySafeCritical]
        public unsafe static HashCode128 SpookyHash128(this Stream stream, long seed0, long seed1)
	    {
	        if(stream == null)
	            throw new ArgumentNullException("stream");
	        var hash = new SpookyHash(seed0, seed1);
	        var buffer = new byte[4096];
	        fixed(void* ptr = buffer)
	            for(int len = stream.Read(buffer, 0, 4096); len != 0; len = stream.Read(buffer, 0, 4096))
	                hash.Update(ptr, len);
            return hash.Final();
	    }
	    /// <summary>
	    /// Produces an 128-bit SpookyHash of a stream, with a default seed.
	    /// </summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halfs of the 128-bit hash.</returns>
	    /// <param name="stream">The stream to hash.</param>
	    /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        public static HashCode128 SpookyHash128(this Stream stream)
	    {
	        return unchecked(SpookyHash128(stream, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of a stream.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="stream">The stream to hash.</param>
	    /// <param name="seed">The 64-bit seed value.</param>
	    /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
	    public static long SpookyHash64(this Stream stream, long seed)
	    {
            return SpookyHash128(stream, seed, seed).Hash1;
	    }
	    /// <summary>
	    /// Produces a 64-bit SpookyHash of a stream, with a default seed.
	    /// </summary>
	    /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
	    /// <param name="stream">The stream to hash.</param>
	    /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
	    public static long SpookyHash64(this Stream stream)
	    {
	        return unchecked(SpookyHash64(stream, (long)SpookyHash.SpookyConst));
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of a stream.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="stream">The stream to hash.</param>
	    /// <param name="seed">The 32-bit seed value.</param>
	    /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
	    public static int SpookyHash32(this Stream stream, int seed)
	    {
            return unchecked((int)SpookyHash128(stream, seed, seed).Hash1);
	    }
	    /// <summary>
	    /// Produces a 32-bit SpookyHash of a stream.
	    /// </summary>
	    /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
	    /// <param name="stream">The stream to hash.</param>
	    /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
	    public static int SpookyHash32(this Stream stream)
	    {
	        return unchecked(SpookyHash32(stream, (int)SpookyHash.SpookyConst));
	    }
	}
}
