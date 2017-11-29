// Hash128Tests.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014 Jon Hanna
//
// This source code is licensed under the EUPL, Version 1.1 only (the “Licence”).
// You may not use, modify or distribute this work except in compliance with the Licence.
// You may obtain a copy of the Licence at:
// <http://joinup.ec.europa.eu/software/page/eupl/licence-eupl>
// A copy is also distributed with this source code.
// Unless required by applicable law or agreed to in writing, software distributed under the
// Licence is distributed on an “AS IS” basis, without warranties or conditions of any kind.

using System;
using SpookilySharp;
using Xunit;

namespace SpookyHashTesting
{
    public class Hash128Tests
    {
        [Fact]
        public void Parsing()
        {
            Assert.False(HashCode128.TryParse(null, out HashCode128 hash));
            Assert.False(HashCode128.TryParse("", out hash));
            Assert.False(HashCode128.TryParse("123456789ABCDE", out hash));
            Assert.False(HashCode128.TryParse("Well, this isn't likely to work, is it?", out hash));
            Assert.False(HashCode128.TryParse("123456789abcdef01", out hash));
            Assert.Equal(hash, HashCode128.Zero);
            Assert.Equal(hash, default(HashCode128));
            Assert.True(HashCode128.TryParse("123456789abcdef00fedcba987654321", out hash));
            Assert.Equal(hash, HashCode128.Parse("  123456789ABCD EF0  0fe DCB a98 765 4321   "));
            Assert.Equal(HashCode128.Parse("00000000000000000000000000000000"), HashCode128.Zero);
            Assert.Equal(hash.GetHashCode(), HashCode128.Parse("123456789abcdef0123456789abcdef0").GetHashCode());
            Assert.NotEqual(hash.GetHashCode(), HashCode128.Zero.GetHashCode());
            Assert.Equal(HashCode128.Zero.GetHashCode(), 0);
            Assert.Equal<ulong>(hash.UHash1, 0x123456789abcdef0);
            Assert.Equal<ulong>(hash.UHash2, 0x0fedcba987654321);
            Assert.Equal(hash.Hash1, (long)0x123456789abcdef0);
            Assert.Equal(hash.Hash2, (long)0x0fedcba987654321);
            Assert.Equal(hash, new HashCode128(0x123456789abcdef0u, 0x0fedcba987654321));
            Assert.Equal(hash, new HashCode128((long)0x123456789abcdef0, (long)0x0fedcba987654321));
            Assert.Equal(hash, HashCode128.Parse("0x123456789abcdef00fedcba987654321"));
            Assert.Equal(hash, HashCode128.Parse("0x123456789abcdef00fedcba987654321"));
        }

        [Fact]
        public void ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>("s", () => HashCode128.Parse(null));
        }

        [Fact]
        public void BadFormat()
        {
            Assert.Throws<FormatException>(() => HashCode128.Parse("0123456780123457833943"));
        }

        [Fact]
        public void BadFormatHigh()
        {
            Assert.Throws<FormatException>(
                () => HashCode128.Parse(
                    "76544561234565434567654456012sdfafasjkl;fdsafdk1234565434561234565434567654456"));
        }

        [Fact]
        public void BadFormatLow()
        {
            Assert.Throws<FormatException>(
                () => HashCode128.Parse("0123456789f12323432343234324324323433232sdrtyrtyttytrty"));
        }

        [Fact]
        public void BadFormatDouble0X()
        {
            Assert.Throws<FormatException>(
                () => HashCode128.Parse("0x0x123456543456765445612345654345676544561234565434567654456"));
        }

        [Fact]
        public void BadFormatTooShort()
        {
            Assert.Throws<FormatException>(() => HashCode128.Parse(""));
        }

        [Fact]
        public void BadFormatTooShortPadded()
        {
            Assert.Throws<FormatException>(
                () => HashCode128.Parse("1234                                                            "));
        }

        [Fact]
        public void BadFormatTooShortHigh()
        {
            Assert.Throws<FormatException>(() => HashCode128.Parse(new string(' ', 32)));
        }

        [Fact]
        public void BadFormatTooShortLow()
        {
            Assert.Throws<FormatException>(
                () => HashCode128.Parse("0123456789abcdef01234                                        "));
        }

        [Fact]
        public void ToStringTests()
        {
            Assert.Equal(
                HashCode128.Parse("0123456789abcdef0123456789ABCDEF").ToString(), "0123456789ABCDEF0123456789ABCDEF");
        }

        [Fact]
        public void EqualsObj()
        {
            Assert.NotEqual<object>(HashCode128.Zero, null);
            Assert.Equal(HashCode128.Zero, (object)HashCode128.Zero);
            object boxed = HashCode128.Parse("0123456789abcdef0123456789ABCDEF");
            Assert.True(boxed.Equals(HashCode128.Parse("0123456789ABCDEF0123456789ABCDEF")));
            Assert.False(boxed.Equals(HashCode128.Zero));
            Assert.False(boxed.Equals("not a hash code"));
            Assert.True(
                Equals(
                    HashCode128.Parse("fed c b a9876543210 0123456789ABCDEF"),
                    HashCode128.Parse("FE DCBA 98765 432 10 0123456789ABCD EF     ")));
        }

        [Fact]
#pragma warning disable 1718 //Yes, I'm testing the obvious!
        public void EqualsOps()
        {
            Assert.True(HashCode128.Zero == HashCode128.Zero);
            Assert.True(
                HashCode128.Parse("0123456789abcdef0123456789abcdef")
                == HashCode128.Parse("0123456789ABCDEF0123456789ABCDEF"));
            Assert.False(HashCode128.Zero != HashCode128.Zero);
            Assert.False(
                HashCode128.Parse("0123456789abcdef0123456789abcdef")
                != HashCode128.Parse("0123456789ABCDEF0123456789ABCDEF"));
            Assert.True(HashCode128.Parse("0123456789abcdef0123456789abcdef") != HashCode128.Zero);
            Assert.False(HashCode128.Parse("0123456789abcdef0123456789abcdef") == HashCode128.Zero);
        }
    }
}