// Redistributor.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014–2017 Jon Hanna
//
// Licensed under the MIT license. See the LICENSE file in the repository root for more details.

using System;

namespace SpookilySharp
{
    /// <summary> Redistributes bits of integers of different sizes. Among other possible uses, this can be useful when
    /// hash codes from other sources have poor distribution. While this cannot improve the overall risk of collision
    /// (indeed, will make it slightly worse), it can help when uses of hash codes are particularly sensitive to
    /// collisions in the one section of bits, e.g. with power-of-two hash tables.</summary>
    public static class Redistributor
    {
        /// <summary>Mixes the bits of an unsigned 32-bit integer.</summary>
        /// <returns>A <see cref="uint"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="uint"/> to re-hash.</param>
        /// <param name="seed">A 32-bit seed.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        [CLSCompliant(false)]
        public static uint Rehash(this uint message, uint seed)
        {
            // Fast-path equivalent of calling SpookyHash.Hash32 with a uint, with all paths not taken by such values
            // removed, all paths that don't affect the output removed, and the remaining code folded into constants or
            // shorter expressions, where possible.
            ulong c = SpookyHash.SpookyConst + message;
            ulong d = 0xE2ADBEEFDEADBEEF ^ c;
            c = c << 15 | c >> -15;
            d += c;
            ulong a = seed ^ d;
            d = d << 52 | d >> -52;
            ulong b = seed ^ (a += d);
            a = a << 26 | a >> -26;
            b += a;
            c ^= b;
            b = b << 51 | b >> -51;
            c += b;
            d ^= c;
            c = c << 28 | c >> -28;
            d += c;
            a ^= d;
            d = d << 9 | d >> -9;
            a += d;
            b ^= a;
            a = a << 47 | a >> -47;
            b += a;
            c ^= b;
            b = b << 54 | b >> -54;
            c += b;
            d ^= c;
            d += c << 32 | c >> -32;
            return (uint)(((a ^ d) + (d << 25 | d >> -25)) >> 1);
        }

        /// <summary>Mixes the bits of an unsigned 32-bit integer.</summary>
        /// <returns>A <see cref="uint"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="uint"/> to re-hash.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        [CLSCompliant(false)]
        public static uint Rehash(this uint message) => Rehash(message, 0xDEADBEEF);

        /// <summary>Mixes the bits of a signed 32-bit integer.</summary>
        /// <returns>An <see cref="int"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="ulong"/> to re-hash.</param>
        /// <param name="seed">A 32-bit seed.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        public static int Rehash(this int message, int seed) => unchecked((int)Rehash((uint)message, (uint)seed));

        /// <summary>Mixes the bits of a signed 32-bit integer.</summary>
        /// <returns>An <see cref="int"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="ulong"/> to re-hash.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        public static int Rehash(this int message) => unchecked(Rehash(message, (int)0xDEADBEEF));

        /// <summary>Mixes the bits of an unsigned 64-bit integer.</summary>
        /// <returns>A <see cref="ulong"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="uint"/> to re-hash.</param>
        /// <param name="seed">A 64-bit seed.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        [CLSCompliant(false)]
        public static ulong Rehash(this ulong message, ulong seed)
        {
            // Fast-path equivalent of calling SpookyHash.Hash64 with a ulong, with all paths not taken by such values
            // removed, all paths that don't affect the output removed, and the remaining code folded into constants or
            // shorter expressions, where possible.
            ulong c = SpookyHash.SpookyConst + message;
            ulong d = 0xE6ADBEEFDEADBEEF ^ c;
            c = c << 15 | c >> -15;
            d += c;
            ulong a = seed ^ d;
            d = d << 52 | d >> -52;
            a += d;
            ulong b = seed ^ a;
            a = a << 26 | a >> -26;
            b += a;
            c ^= b;
            b = b << 51 | b >> -51;
            c += b;
            d ^= c;
            c = c << 28 | c >> -28;
            d += c;
            a ^= d;
            d = d << 9 | d >> -9;
            a += d;
            b ^= a;
            a = a << 47 | a >> -47;
            b += a;
            c ^= b;
            b = b << 54 | b >> -54;
            c += b;
            d ^= c;
            c = c << 32 | c >> -32;
            d += c;
            a ^= d;
            d = d << 25 | d >> -25;
            a += d;
            return a << 63 | a >> -63;
        }

        /// <summary>Mixes the bits of an unsigned 64-bit integer.</summary>
        /// <returns>A <see cref="ulong"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="uint"/> to re-hash.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        [CLSCompliant(false)]
        public static ulong Rehash(this ulong message) => Rehash(message, SpookyHash.SpookyConst);

        /// <summary>Mixes the bits of a signed 64-bit integer.</summary>
        /// <returns>A <see cref="long"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="uint"/> to re-hash.</param>
        /// <param name="seed">A 64-bit seed.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        public static long Rehash(this long message, long seed) => unchecked((long)Rehash((ulong)message, (ulong)seed));

        /// <summary>Mixes the bits of a signed 64-bit integer.</summary>
        /// <returns>A <see cref="long"/> that could serve as a hash for the input.</returns>
        /// <param name="message">A <see cref="uint"/> to re-hash.</param>
        /// <remarks>This cannot improve the overall collision-risk of a poor hash, but does improve poor hashes that
        /// suffer particularly in the lower bits, which includes a great many that are to be found in .NET and
        /// Mono.</remarks>
        public static long Rehash(this long message) => unchecked(Rehash(message, (long)SpookyHash.SpookyConst));
    }
}
