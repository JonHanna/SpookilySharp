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
using SpookilySharp;
using Xunit;

namespace SpookyHashTesting
{
    public sealed partial class HasherTests
    {
        private Stream GetStream()
        {
            return new FileStream("nunit.framework.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        [Fact]
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
            Assert.Equal(h, SpookyHasher.SpookyHash128(testString));
            Assert.Equal(h, SpookyHasher.SpookyHash128(testString, 0, len, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            Assert.Equal(
                h,
                unchecked(SpookyHasher.SpookyHash128(
                    testString,
                    0,
                    len,
                    (long)0xDEADBEEFDEADBEEF,
                    (long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(h, SpookyHasher.SpookyHash128(testString, 0, len));
            var hashSlice = SpookyHasher.SpookyHash128(testString, 50, 100, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF);
            Assert.NotEqual(h, hashSlice);
            Assert.Equal(
                hashSlice,
                unchecked(SpookyHasher.SpookyHash128(
                    testString,
                    50,
                    100,
                    (long)0xDEADBEEFDEADBEEF,
                    (long)0xDEADBEEFDEADBEEF)));
            long longHash = SpookyHasher.SpookyHash64(testString, 0, len, unchecked((long)0xDEADBEEFDEADBEEF));
            Assert.Equal(longHash, SpookyHasher.SpookyHash64(testString, unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(longHash, SpookyHasher.SpookyHash64(testString, 0, len));
            Assert.Equal(longHash, SpookyHasher.SpookyHash64(testString));
            int hash = SpookyHasher.SpookyHash32(testString, 0, len, unchecked((int)0xDEADBEEF));
            Assert.Equal(hash, SpookyHasher.SpookyHash32(testString, unchecked((int)0xDEADBEEF)));
            Assert.Equal(hash, SpookyHasher.SpookyHash32(testString, 0, len));
            Assert.Equal(hash, SpookyHasher.SpookyHash32(testString));
            testString = null;
            Assert.Equal(HashCode128.Zero, SpookyHasher.SpookyHash128(testString));
            Assert.Equal(
                HashCode128.Zero,
                SpookyHasher.SpookyHash128(
                    testString,
                    0,
                    200,
                    0xDEADBEEFDEADBEEF,
                    0xDEADBEEFDEADBEEF));
            Assert.Equal(
                HashCode128.Zero,
                unchecked(SpookyHasher.SpookyHash128(
                    testString,
                    0,
                    200,
                    (long)0xDEADBEEFDEADBEEF,
                    (long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(HashCode128.Zero, SpookyHasher.SpookyHash128(testString, 0, 200));
            Assert.Equal(
                HashCode128.Zero,
                SpookyHasher.SpookyHash128(
                    testString,
                    50,
                    100,
                    0xDEADBEEFDEADBEEF,
                    0xDEADBEEFDEADBEEF));
            Assert.Equal(0, SpookyHasher.SpookyHash64(testString, 0, 200, unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(0, SpookyHasher.SpookyHash64(testString, unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(0, SpookyHasher.SpookyHash64(testString, 0, 200));
            Assert.Equal(0, SpookyHasher.SpookyHash64(testString));
            Assert.Equal(0, SpookyHasher.SpookyHash32(testString, 0, 200, unchecked((int)0xDEADBEEF)));
            Assert.Equal(0, SpookyHasher.SpookyHash32(testString, unchecked((int)0xDEADBEEF)));
            Assert.Equal(0, SpookyHasher.SpookyHash32(testString, 0, 200));
            Assert.Equal(0, SpookyHasher.SpookyHash32(testString));
        }
        [Fact]
        public void NegativeOffest32String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash32("", -1, 2));
        }
        [Fact]
        public void ExcessiveOffest32String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash32("", 40, 2));
        }
        [Fact]
        public void ExcessiveLength32String()
        {
            Assert.Throws<ArgumentException>(() => SpookyHasher.SpookyHash32("a", 0, 2));
        }
        [Fact]
        public void NegativeLength32String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash32("", 0, -3));
        }
        [Fact]
        public void NegativeOffest64String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash64("", -1, 2));
        }
        [Fact]
        public void ExcessiveOffest64String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash64("", 40, 2));
        }
        [Fact]
        public void ExcessiveLength64String()
        {
            Assert.Throws<ArgumentException>(() => SpookyHasher.SpookyHash64("a", 0, 2));
        }
        [Fact]
        public void NegativeLength64String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash64("", 0, -3));
        }
        [Fact]
        public void NegativeOffest128String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash128("", -1, 2));
        }
        [Fact]
        public void ExcessiveOffest128String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash128("", 40, 2));
        }
        [Fact]
        public void ExcessiveLength128String()
        {
            Assert.Throws<ArgumentException>(() => SpookyHasher.SpookyHash128("a", 0, 2));
        }
        [Fact]
        public void NegativeLength128String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => SpookyHasher.SpookyHash128("", 0, -3));
        }
    }
}