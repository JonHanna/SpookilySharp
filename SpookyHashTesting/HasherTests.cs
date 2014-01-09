// HasherTests.cs
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
using System.IO;
using NUnit.Framework;
using SpookilySharp;

namespace SpookyHashTesting
{
    [TestFixture]
    public sealed partial class HasherTests
    {
        private Stream GetStream()
        {
            return new FileStream("nunit.framework.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        [Test]
        public void StringExtension()
        {
            string testString;
            using(var stm = GetStream())
            using(var tr = new StreamReader(stm))
                testString = tr.ReadToEnd();
            var sh = new SpookyHash();
            sh.Update(testString);
            var h = sh.Final();
            int len = testString.Length;
            Assert.AreEqual(h, testString.SpookyHash128());
            Assert.AreEqual(h, testString.SpookyHash128(0, len, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            Assert.AreEqual(h, unchecked(testString.SpookyHash128(0, len, (long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            Assert.AreEqual(h, testString.SpookyHash128(0, len));
            var hSlice = testString.SpookyHash128(50, 100, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF);
            Assert.AreNotEqual(h, hSlice);
            Assert.AreEqual(hSlice, unchecked(testString.SpookyHash128(50, 100, (long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            long lHash = testString.SpookyHash64(0, len, unchecked((long)0xDEADBEEFDEADBEEF));
            Assert.AreEqual(lHash, testString.SpookyHash64(unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.AreEqual(lHash, testString.SpookyHash64(0, len));
            Assert.AreEqual(lHash, testString.SpookyHash64());
            int hash = testString.SpookyHash32(0, len, unchecked((int)0xDEADBEEF));
            Assert.AreEqual(hash, testString.SpookyHash32(unchecked((int)0xDEADBEEF)));
            Assert.AreEqual(hash, testString.SpookyHash32(0, len));
            Assert.AreEqual(hash, testString.SpookyHash32());
            testString = null;
            Assert.AreEqual(HashCode128.Zero, testString.SpookyHash128());
            Assert.AreEqual(HashCode128.Zero, testString.SpookyHash128(0, 200, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            Assert.AreEqual(HashCode128.Zero, unchecked(testString.SpookyHash128(0, 200, (long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            Assert.AreEqual(HashCode128.Zero, testString.SpookyHash128(0, 200));
            Assert.AreEqual(HashCode128.Zero, testString.SpookyHash128(50, 100, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            Assert.AreEqual(0, testString.SpookyHash64(0, 200, unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.AreEqual(0, testString.SpookyHash64(unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.AreEqual(0, testString.SpookyHash64(0, 200));
            Assert.AreEqual(0, testString.SpookyHash64());
            Assert.AreEqual(0, testString.SpookyHash32(0, 200, unchecked((int)0xDEADBEEF)));
            Assert.AreEqual(0, testString.SpookyHash32(unchecked((int)0xDEADBEEF)));
            Assert.AreEqual(0, testString.SpookyHash32(0, 200));
            Assert.AreEqual(0, testString.SpookyHash32());
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeOffest32String()
        {
            "".SpookyHash32(-1, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExcessiveOffest32String()
        {
            "".SpookyHash32(40, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExcessiveLength32String()
        {
            "".SpookyHash32(0, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeOffest64String()
        {
            "".SpookyHash64(-1, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExcessiveOffest64String()
        {
            "".SpookyHash64(40, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExcessiveLength64String()
        {
            "".SpookyHash64(0, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeOffest128String()
        {
            "".SpookyHash128(-1, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExcessiveOffest128String()
        {
            "".SpookyHash128(40, 2);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExcessiveLength128String()
        {
            "".SpookyHash128(0, 2);
        }
    }
}