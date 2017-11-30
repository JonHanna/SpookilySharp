// HasherTests.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014–2017 Jon Hanna
//
// Licensed under the MIT license. See the LICENSE file in the repository root for more details.

using System;
using System.IO;
using SpookilySharp;
using Xunit;

namespace SpookyHashTesting
{
    public sealed partial class HasherTests
    {
        private static Stream GetStream() => new FileStream("testfile.xml", FileMode.Open, FileAccess.Read, FileShare.Read);

        [Fact]
        public void StringExtension()
        {
            string testString;
            using (Stream stm = GetStream())
            using (StreamReader tr = new StreamReader(stm))
            {
                testString = tr.ReadToEnd();
            }
            SpookyHash sh = new SpookyHash();
            sh.Update(testString);
            HashCode128 h = sh.Final();
            int len = testString.Length;
            Assert.Equal(h, testString.SpookyHash128());
            Assert.Equal(h, testString.SpookyHash128(0, len, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            Assert.Equal(
                h,
                unchecked(testString.SpookyHash128(0, len, (long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(h, testString.SpookyHash128(0, len));
            HashCode128 hashSlice = testString.SpookyHash128(50, 100, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF);
            Assert.NotEqual(h, hashSlice);
            Assert.Equal(
                hashSlice,
                unchecked(testString.SpookyHash128(50, 100, (long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            long longHash = testString.SpookyHash64(0, len, unchecked((long)0xDEADBEEFDEADBEEF));
            Assert.Equal(longHash, testString.SpookyHash64(unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(longHash, testString.SpookyHash64(0, len));
            Assert.Equal(longHash, testString.SpookyHash64());
            int hash = testString.SpookyHash32(0, len, unchecked((int)0xDEADBEEF));
            Assert.Equal(hash, testString.SpookyHash32(unchecked((int)0xDEADBEEF)));
            Assert.Equal(hash, testString.SpookyHash32(0, len));
            Assert.Equal(hash, testString.SpookyHash32());
            Assert.Equal(HashCode128.Zero, default(string).SpookyHash128());
            Assert.Equal(
                HashCode128.Zero,
                default(string).SpookyHash128(0, 200, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            Assert.Equal(
                HashCode128.Zero,
                unchecked(default(string).SpookyHash128(0, 200, (long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(HashCode128.Zero, default(string).SpookyHash128(0, 200));
            Assert.Equal(
                HashCode128.Zero,
                default(string).SpookyHash128(50, 100, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            Assert.Equal(0, default(string).SpookyHash64(0, 200, unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(0, default(string).SpookyHash64(unchecked((long)0xDEADBEEFDEADBEEF)));
            Assert.Equal(0, default(string).SpookyHash64(0, 200));
            Assert.Equal(0, default(string).SpookyHash64());
            Assert.Equal(0, default(string).SpookyHash32(0, 200, unchecked((int)0xDEADBEEF)));
            Assert.Equal(0, default(string).SpookyHash32(unchecked((int)0xDEADBEEF)));
            Assert.Equal(0, default(string).SpookyHash32(0, 200));
            Assert.Equal(0, default(string).SpookyHash32());
        }

        [Fact]
        public void NegativeOffest32String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "".SpookyHash32(-1, 2));
        }

        [Fact]
        public void ExcessiveOffest32String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "".SpookyHash32(40, 2));
        }

        [Fact]
        public void ExcessiveLength32String()
        {
            Assert.Throws<ArgumentException>(() => "a".SpookyHash32(0, 2));
        }

        [Fact]
        public void NegativeLength32String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("length", () => "".SpookyHash32(0, -3));
        }

        [Fact]
        public void NegativeOffest64String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "".SpookyHash64(-1, 2));
        }

        [Fact]
        public void ExcessiveOffest64String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "".SpookyHash64(40, 2));
        }

        [Fact]
        public void ExcessiveLength64String()
        {
            Assert.Throws<ArgumentException>(() => "a".SpookyHash64(0, 2));
        }

        [Fact]
        public void NegativeLength64String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("length", () => "".SpookyHash64(0, -3));
        }

        [Fact]
        public void NegativeOffest128String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "".SpookyHash128(-1, 2));
        }

        [Fact]
        public void ExcessiveOffest128String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "".SpookyHash128(40, 2));
        }

        [Fact]
        public void ExcessiveLength128String()
        {
            Assert.Throws<ArgumentException>(() => "a".SpookyHash128(0, 2));
        }

        [Fact]
        public void NegativeLength128String()
        {
            Assert.Throws<ArgumentOutOfRangeException>("length", () => "".SpookyHash128(0, -3));
        }
    }
}
