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
using NUnit.Framework;
using SpookilySharp;

namespace SpookyHashTesting
{
    [TestFixture]
    public class Hash128Tests
    {
        [Test]
        public void Parsing()
        {
            HashCode128 hash;
            Assert.IsFalse(HashCode128.TryParse(null, out hash));
            Assert.IsFalse(HashCode128.TryParse("", out hash));
            Assert.IsFalse(HashCode128.TryParse("123456789ABCDE", out hash));
            Assert.IsFalse(HashCode128.TryParse("Well, this isn't likely to work, is it?", out hash));
            Assert.IsFalse(HashCode128.TryParse("123456789abcdef01", out hash));
            Assert.AreEqual(hash, HashCode128.Zero);
            Assert.AreEqual(hash, default(HashCode128));
            Assert.IsTrue(HashCode128.TryParse("123456789abcdef0", out hash));
            Assert.AreEqual(hash, HashCode128.Parse("  123456789ABCDEF0     "));
            Assert.AreEqual(HashCode128.Parse("0000000000000000"), HashCode128.Zero);
            Assert.AreEqual(hash.GetHashCode(), HashCode128.Parse("123456789abcdef0").GetHashCode());
            Assert.AreNotEqual(hash.GetHashCode(), HashCode128.Zero.GetHashCode());
            Assert.AreEqual(HashCode128.Zero.GetHashCode(), 0);
            Assert.AreEqual(hash.UHash1, 0x12345678);
            Assert.AreEqual(hash.UHash2, 0x9abcdef0);
            Assert.AreEqual(hash.Hash1, (long)0x12345678);
            Assert.AreEqual(hash.Hash2, (long)0x9abcdef0);
            Assert.AreEqual(hash, new HashCode128(0x12345678u, 0x9abcdef0));
            Assert.AreEqual(hash, new HashCode128((long)0x12345678, (long)0x9abcdef0));
            Assert.AreEqual(hash, HashCode128.Parse("0x123456789abcdef0"));
            Assert.AreEqual(hash, HashCode128.Parse("0x123456789ABCDEF0"));
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNull()
        {
            HashCode128.Parse(null);
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void BadFormat()
        {
            HashCode128.Parse("0123456780123457833943");
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void BadFormatHigh()
        {
            HashCode128.Parse("012sdfafasjkl;fdsafdk");
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void BadFormatLow()
        {
            HashCode128.Parse("0123456789fsdrtyrtyttytrty");
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void BadFormatDouble0X()
        {
            HashCode128.Parse("0x0x1234565434567654456");
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void BadFormatTooShort()
        {
            HashCode128.Parse("");
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void BadFormatTooShortHigh()
        {
            HashCode128.Parse(new string(' ', 16));
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void BadFormatTooShortLow()
        {
            HashCode128.Parse("1234567890        ");
        }
        [Test]
        public void ToStringTests()
        {
            Assert.AreEqual(HashCode128.Parse("0123456789abcdef").ToString(), "0123456789ABCDEF");
        }
        [Test]
        public void EqualsObj()
        {
            Assert.AreNotEqual(HashCode128.Zero, null);
            Assert.AreEqual(HashCode128.Zero, (object)HashCode128.Zero);
            object boxed = HashCode128.Parse("0123456789abcdef");
            Assert.True(boxed.Equals(HashCode128.Parse("0123456789ABCDEF")));
            Assert.False(boxed.Equals(HashCode128.Zero));
            Assert.False(boxed.Equals("not a hash code"));
            Assert.True(object.Equals(HashCode128.Parse("fed c b a9876543210"), HashCode128.Parse("FE DCBA 98765 432 10")));
        }
        [Test]
        #pragma warning disable 1718 //Yes, I'm testing the obvious!
        public void EqualsOps()
        {
            Assert.True(HashCode128.Zero == HashCode128.Zero);
            Assert.True(HashCode128.Parse("0123456789abcdef") == HashCode128.Parse("0123456789ABCDEF"));
            Assert.False(HashCode128.Zero != HashCode128.Zero);
            Assert.False(HashCode128.Parse("0123456789abcdef") != HashCode128.Parse("0123456789ABCDEF"));
            Assert.True(HashCode128.Parse("0123456789abcdef") != HashCode128.Zero);
            Assert.False(HashCode128.Parse("0123456789abcdef") == HashCode128.Zero);
        }
    }
}