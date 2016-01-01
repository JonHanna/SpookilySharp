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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace SpookilySharp
{
    /// <summary>Provides static extension methods for producing SpookyHashes of various types of object.</summary>
    public static partial class SpookyHasher
    {
        [SecurityCritical]
        private static unsafe HashCode128 SpookyHash128Unchecked(
            string message, int startIndex, int length, ulong seed0, ulong seed1)
        {
            fixed(char* ptr = message)
                SpookyHash.Hash128(ptr + startIndex, length << 1, ref seed0, ref seed1);
            return new HashCode128(seed0, seed1);
        }

        /// <summary>Produces an 128-bit SpookyHash of a <see cref="string"/>.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the 128-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <param name="startIndex">The index from which to hash.</param>
        /// <param name="length">The number of <see cref="char"/>s to hash.</param>
        /// <param name="seed0">The first 64-bits of the seed value.</param>
        /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater
        /// than the length of the string.</exception>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater
        /// than the length of the string.</exception>
        [SecuritySafeCritical]
        [CLSCompliant(false)]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            int startIndex,
            int length,
            ulong seed0,
            ulong seed1)
        {
            if(message == null)
                return default(HashCode128);
            ExceptionHelper.CheckBounds(message, startIndex, length);
            return unchecked(SpookyHash128Unchecked(message, startIndex, length, seed0, seed1));
        }

        /// <summary>Produces an 128-bit SpookyHash of a <see cref="string"/>.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the 128-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <param name="startIndex">The index from which to hash.</param>
        /// <param name="length">The number of <see cref="char"/>s to hash.</param>
        /// <param name="seed0">The first 64-bits of the seed value.</param>
        /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater
        /// than the length of the string.</exception>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater
        /// than the length of the string.</exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            int startIndex,
            int length,
            long seed0,
            long seed1)
        {
            return unchecked(SpookyHash128(message, startIndex, length, (ulong)seed0, (ulong)seed1));
        }

        /// <summary>Produces an 128-bit SpookyHash of a <see cref="string"/>.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the 128-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <param name="seed0">The first 64-bits of the seed value.</param>
        /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            long seed0,
            long seed1)
        {
            return message == null
                ? default(HashCode128)
                : unchecked(SpookyHash128Unchecked(message, 0, message.Length, (ulong)seed0, (ulong)seed1));
        }

        /// <summary>Produces an 128-bit SpookyHash of a <see cref="string"/>, using a default seed.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the 128-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <param name="startIndex">The index from which to hash.</param>
        /// <param name="length">The number of <see cref="char"/>s to hash.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater
        /// than the length of the string.</exception>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater
        /// than the length of the string.</exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SecuritySafeCritical]
        public static HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            int startIndex,
            int length)
        {
            if(message == null)
                return default(HashCode128);
            ExceptionHelper.CheckBounds(message, startIndex, length);
            return SpookyHash128Unchecked(message, startIndex, length, SpookyHash.SpookyConst, SpookyHash.SpookyConst);
        }

        /// <summary>Produces an 128-bit SpookyHash of a <see cref="string"/>, using a default seed.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halves of the 128-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        public static HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            string message)
        {
            return unchecked(SpookyHash128(message, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
        }

        [SecurityCritical]
        private static unsafe long SpookyHash64Unchecked(string message, int startIndex, int length, long seed)
        {
            fixed(char* ptr = message)
                return unchecked((long)SpookyHash.Hash64(ptr + startIndex, length << 1, (ulong)seed));
        }

        /// <summary>Produces a 64-bit SpookyHash of a <see cref="string"/>.</summary>
        /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <param name="startIndex">The index from which to hash.</param>
        /// <param name="length">The number of <see cref="char"/>s to hash.</param>
        /// <param name="seed">The 64-bit seed value.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater
        /// than the length of the string.</exception>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater
        /// than the length of the string.</exception>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static unsafe long SpookyHash64(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            int startIndex,
            int length,
            long seed)
        {
            if(message == null)
                return 0L;
            ExceptionHelper.CheckBounds(message, startIndex, length);
            return SpookyHash64Unchecked(message, startIndex, length, seed);
        }

        /// <summary>Produces a 64-bit SpookyHash of a <see cref="string"/>, using a default seed.</summary>
        /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <param name="startIndex">The index from which to hash.</param>
        /// <param name="length">The number of <see cref="char"/>s to hash.</param>
        /// <remarks>For a null string, the hash will be <see cref="HashCode128.Zero"/> .</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater
        /// than the length of the string.</exception>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater
        /// than the length of the string.</exception>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static unsafe long SpookyHash64(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            int startIndex,
            int length)
        {
            if(message == null)
                return 0L;
            ExceptionHelper.CheckBounds(message, startIndex, length);
            return SpookyHash64Unchecked(message, 0, message.Length, unchecked((long)SpookyHash.SpookyConst));
        }

        /// <summary>Produces a 64-bit SpookyHash of a <see cref="string"/>.</summary>
        /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <param name="seed">The 64-bit seed value.</param>
        /// <remarks>For a null string, the hash will be zero.</remarks>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static unsafe long SpookyHash64(
#if !NET_20 && !NET_30
            this
#endif
            string message, long seed)
        {
            return message == null ? 0L : SpookyHash64Unchecked(message, 0, message.Length, seed);
        }

        /// <summary>Produces a 64-bit SpookyHash of a <see cref="string"/>, using a default seed.</summary>
        /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
        /// <param name="message">The <see cref="string"/> to hash.</param>
        /// <remarks>For a null string, the hash will be zero.</remarks>
        public static long SpookyHash64(
#if !NET_20 && !NET_30
            this
#endif
            string message)
        {
            return SpookyHash64(message, unchecked((long)SpookyHash.SpookyConst));
        }

        [SecurityCritical]
        private static int SpookyHash32Unchecked(string message, int startIndex, int length, uint seed)
        {
            return unchecked((int)SpookyHash64Unchecked(message, startIndex, length, seed));
        }

        /// <summary>Produces a 32-bit SpookyHash of a <see cref="string"/>s.</summary>
        /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
        /// <param name="message">The string to hash.</param>
        /// <param name="startIndex">The index from which to hash.</param>
        /// <param name="length">The number of characters to hash.</param>
        /// <param name="seed">The 32-bit seed value.</param>
        /// <remarks>For a null string, the hash will be zero.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater
        /// than the length of the string.</exception>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater
        /// than the length of the string.</exception>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static int SpookyHash32(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            int startIndex,
            int length,
            int seed)
        {
            if(message == null)
                return 0;
            ExceptionHelper.CheckBounds(message, startIndex, length);
            return unchecked(SpookyHash32Unchecked(message, startIndex, length, (uint)seed));
        }

        /// <summary>Produces a 32-bit SpookyHash of a <see cref="string"/>, using a default seed.</summary>
        /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
        /// <param name="message">The string to hash.</param>
        /// <param name="startIndex">The index from which to hash.</param>
        /// <param name="length">The number of characters to hash.</param>
        /// <remarks>For a null string, the hash will be zero.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> was less than zero, or greater
        /// than the length of the string.</exception>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> plus <paramref name="length"/> is greater
        /// than the length of the string.</exception>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static int SpookyHash32(
#if !NET_20 && !NET_30
            this
#endif
            string message,
            int startIndex,
            int length)
        {
            if(message == null)
                return 0;
            ExceptionHelper.CheckBounds(message, startIndex, length);
            return unchecked(SpookyHash32Unchecked(message, startIndex, length, (uint)SpookyHash.SpookyConst));
        }

        /// <summary>Produces a 32-bit SpookyHash of a <see cref="string"/>.</summary>
        /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
        /// <param name="message">The string to hash.</param>
        /// <param name="seed">The 32-bit seed value.</param>
        /// <remarks>For a null string, the hash will be zero.</remarks>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static int SpookyHash32(
#if !NET_20 && !NET_30
            this
#endif
            string message, int seed)
        {
            return message == null ? 0 : unchecked(SpookyHash32Unchecked(message, 0, message.Length, (uint)seed));
        }

        /// <summary>Produces a 32-bit SpookyHash of a <see cref="string"/>, using a default seed.</summary>
        /// <returns>An <see cref="int"/> containing the two 32-bit hash.</returns>
        /// <param name="message">The string to hash.</param>
        /// <remarks>For a null string, the hash will be zero.</remarks>
        [SecuritySafeCritical]
        public static int SpookyHash32(
#if !NET_20 && !NET_30
            this
#endif
            string message)
        {
            return message == null ? 0 : unchecked(SpookyHash32Unchecked(
                message,
                0,
                message.Length,
                (uint)SpookyHash.SpookyConst));
        }

        /// <summary>Produces an 128-bit SpookyHash of a stream.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halves of the 128-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <param name="seed0">The first 64-bits of the seed value.</param>
        /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        [SecuritySafeCritical]
        [CLSCompliant(false)]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static unsafe HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream,
            ulong seed0,
            ulong seed1)
        {
            ExceptionHelper.CheckNotNull(stream);
            var hash = new SpookyHash(seed0, seed1);
            var buffer = new byte[4096];
            fixed(void* ptr = buffer)
                for(int len = stream.Read(buffer, 0, 4096); len != 0; len = stream.Read(buffer, 0, 4096))
                    hash.Update(ptr, len);
            return hash.Final();
        }

#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        [CLSCompliant(false)]
        public async static Task<HashCode128> SpookyHash128Async(this Stream stream, ulong seed0, ulong seed1, CancellationToken cancellationToken)
        {
            ExceptionHelper.CheckNotNull(stream);
            cancellationToken.ThrowIfCancellationRequested();
            var hash = new SpookyHash(seed0, seed1);
            var buffer = new byte[4096];
            for (int len = await stream.ReadAsync(buffer, 0, 4096, cancellationToken); len != 0; len = await stream.ReadAsync(buffer, 0, 4096, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                hash.Update(buffer, 0, len);
            }
            return hash.Final();
        }
        [CLSCompliant(false)]
        public static Task<HashCode128> SpookyHash128Async(this Stream stream, ulong seed0, ulong seed1)
        {
            return SpookyHash128Async(stream, seed0, seed1, CancellationToken.None);
        }
#endif

        /// <summary>Produces an 128-bit SpookyHash of a stream.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halves of the 128-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <param name="seed0">The first 64-bits of the seed value.</param>
        /// <param name="seed1">The second 64-bits of the seed value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream,
            long seed0,
            long seed1)
        {
            return unchecked(SpookyHash128(stream, (ulong)seed0, (ulong)seed1));
        }

#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        public static Task<HashCode128> SpookyHash128Async(this Stream stream, long seed0, long seed1, CancellationToken cancellationToken)
        {
            return unchecked(SpookyHash128Async(stream, (ulong)seed0, (ulong)seed1, cancellationToken));
        }
        public static Task<HashCode128> SpookyHash128Async(this Stream stream, long seed0, long seed1)
        {
            return SpookyHash128Async(stream, seed0, seed1, CancellationToken.None);
        }
#endif

        /// <summary>Produces an 128-bit SpookyHash of a stream, with a default seed.</summary>
        /// <returns>A <see cref="HashCode128"/> containing the two 64-bit halves of the 128-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        public static HashCode128 SpookyHash128(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream)
        {
            return SpookyHash128(stream, SpookyHash.SpookyConst, SpookyHash.SpookyConst);
        }

#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        public static Task<HashCode128> SpookyHash128Async(this Stream stream, CancellationToken cancellationToken)
        {
            return SpookyHash128Async(stream, SpookyHash.SpookyConst, SpookyHash.SpookyConst, cancellationToken);
        }
        public static Task<HashCode128> SpookyHash128Async(this Stream stream)
        {
            return SpookyHash128Async(stream, CancellationToken.None);
        }
#endif

        /// <summary>Produces a 64-bit SpookyHash of a stream.</summary>
        /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <param name="seed">The 64-bit seed value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        [CLSCompliant(false)]
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static unsafe ulong SpookyHash64(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream, ulong seed)
        {
            ExceptionHelper.CheckNotNull(stream);
            var hash = new SpookyHash(seed, seed);
            var buffer = new byte[4096];
            fixed(void* ptr = buffer)
                for(int len = stream.Read(buffer, 0, 4096); len != 0; len = stream.Read(buffer, 0, 4096))
                    hash.Update(ptr, len);
            hash.Final(out seed, out seed);
            return seed;
        }

#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        [CLSCompliant(false)]
        public async static Task<ulong> SpookyHash64Async(this Stream stream, ulong seed, CancellationToken cancellationToken)
        {
            ExceptionHelper.CheckNotNull(stream);
            cancellationToken.ThrowIfCancellationRequested();
            var hash = new SpookyHash(seed, seed);
            var buffer = new byte[4096];
            for (int len = await stream.ReadAsync(buffer, 0, 4096, cancellationToken); len != 0; len = await stream.ReadAsync(buffer, 0, 4096, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                hash.Update(buffer, 0, len);
            }
            hash.Final(out seed, out seed);
            return seed;
        }
        [CLSCompliant(false)]
        public static Task<ulong> SpookyHash64Async(this Stream stream, ulong seed)
        {
            return SpookyHash64Async(stream, seed, CancellationToken.None);
        }
#endif

        /// <summary>Produces a 64-bit SpookyHash of a stream.</summary>
        /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <param name="seed">The 64-bit seed value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static long SpookyHash64(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream, long seed)
        {
            return unchecked((long)SpookyHash64(stream, (ulong)seed));
        }

#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        public static Task<long> SpookyHash64Async(this Stream stream, long seed, CancellationToken cancellationToken)
        {
            return SpookyHash64Async(stream, unchecked((ulong)seed)).ContinueWith(t => unchecked((long)t.Result), cancellationToken, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }
        public static Task<long> SpookyHash64Async(this Stream stream, long seed)
        {
            return SpookyHash64Async(stream, seed, CancellationToken.None);
        }
#endif

        /// <summary>Produces a 64-bit SpookyHash of a stream, with a default seed.</summary>
        /// <returns>A <see cref="long"/> containing the 64-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        public static long SpookyHash64(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream)
        {
            return unchecked(SpookyHash64(stream, (long)SpookyHash.SpookyConst));
        }

#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        public static Task<long> SpookyHash64Async(Stream stream, CancellationToken cancellationToken)
        {
            return unchecked(SpookyHash64Async(stream, (long)SpookyHash.SpookyConst, cancellationToken));
        }
        public static Task<long> SpookyHash64Async(Stream stream)
        {
            return SpookyHash64Async(stream, CancellationToken.None);
        }
#endif

        /// <summary>Produces a 32-bit SpookyHash of a stream.</summary>
        /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <param name="seed">The 32-bit seed value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        [CLSCompliant(false)]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static uint SpookyHash32(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream, uint seed)
        {
            return unchecked((uint)SpookyHash64(stream, (ulong)seed));
        }

#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        [CLSCompliant(false)]
        public static Task<uint> SpookyHash32Async(this Stream stream, uint seed, CancellationToken cancellationToken)
        {
            return unchecked(SpookyHash64Async(stream, (ulong)seed).ContinueWith(t => (uint)t.Result, cancellationToken, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default));
        }
        [CLSCompliant(false)]
        public static Task<uint> SpookyHash32Async(this Stream stream, uint seed)
        {
            return SpookyHash32Async(stream, seed, CancellationToken.None);
        }
#endif

        /// <summary>Produces a 32-bit SpookyHash of a stream.</summary>
        /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <param name="seed">The 32-bit seed value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1115:ParameterMustFollowComma",
            Justification = "Necessary to allow conditional compilation of extension methods.")]
        public static int SpookyHash32(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream, int seed)
        {
            return unchecked((int)SpookyHash64(stream, (ulong)(uint)seed));
        }
#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        public static Task<int> SpookyHash32Async(this Stream stream, int seed, CancellationToken cancellationToken)
        {
            return unchecked(SpookyHash64Async(stream, (ulong)(uint)seed, cancellationToken).ContinueWith(t => (int)t.Result, cancellationToken, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default));
        }
        public static Task<int> SpookyHash32Async(this Stream stream, int seed)
        {
            return SpookyHash32Async(stream, seed, CancellationToken.None);
        }
#endif

        /// <summary>Produces a 32-bit SpookyHash of a stream.</summary>
        /// <returns>An <see cref="int"/> containing the 32-bit hash.</returns>
        /// <param name="stream">The stream to hash.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        public static int SpookyHash32(
#if !NET_20 && !NET_30
            this
#endif
            Stream stream)
        {
            return unchecked(SpookyHash32(stream, (int)(uint)SpookyHash.SpookyConst));
        }
#if !NET_20 && !NET_30 && !NET_35 && !NET_40
        public static Task<int> SpookyHash32Async(this Stream stream, CancellationToken cancellationToken)
        {
            return unchecked(SpookyHash32Async(stream, (int)(uint)SpookyHash.SpookyConst, cancellationToken));
        }
        public static Task<int> SpookyHash32Async(this Stream stream)
        {
            return SpookyHash32Async(stream, CancellationToken.None);
        }
#endif
    }
}