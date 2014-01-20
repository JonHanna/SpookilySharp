// StreamTests.cs
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
using System.Net;
using NUnit.Framework;
using SpookilySharp;

namespace SpookyHashTesting
{
    [TestFixture]
    public class StreamTests
    {
        private FileStream GetFileStream()
        {
            return new FileStream("nunit.framework.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        private void GetStreams(out FileStream fs, out MemoryStream ms)
        {
            fs = GetFileStream();
            ms = new MemoryStream();
        }
        private void WriteOut(HashedStream inStr, HashedStream outStr)
        {
            Random rand = new Random();
            using(inStr)
            using(outStr)
            {
                Assert.IsTrue(inStr.CanRead);
                Assert.IsTrue(outStr.CanWrite);
                if(inStr.CanTimeout)
                    Assert.AreNotEqual(0, inStr.ReadTimeout);
                if(outStr.CanTimeout)
                    Assert.AreNotEqual(0, outStr.WriteTimeout);
                for(;;)
                {
                    var buffer = new byte[rand.Next(1, 20000)];
                    int read = inStr.Read(buffer, 0, buffer.Length);
                    if(read == 0)
                        return;
                    outStr.Write(buffer, 0, read);
                    int by = inStr.ReadByte();
                    if(by == -1)
                        return;
                    outStr.WriteByte((byte)by);
                }
            }
        }
        [Test]
        public void DefaultConst()
        {
            FileStream fs;
            MemoryStream ms;
            GetStreams(out fs, out ms);
            var hsIn = new HashedStream(fs);
            var hsOut = new HashedStream(ms);
            WriteOut(hsIn, hsOut);
            Assert.False(hsIn.WasMoved);
            Assert.True(hsIn.ReadHash128 == hsOut.WriteHash128);
            Assert.True(hsIn.ReadHash32 == hsOut.WriteHash32);
            Assert.True(hsIn.ReadHash64 == hsOut.WriteHash64);
        }
        [Test]
        public void ULongConst()
        {
            FileStream fs;
            MemoryStream ms;
            GetStreams(out fs, out ms);
            var hsIn = new HashedStream(fs, 42UL, 53UL);
            var hsOut = new HashedStream(ms, 42UL, 53UL);
            WriteOut(hsIn, hsOut);
            Assert.False(hsIn.WasMoved);
            Assert.True(hsIn.ReadHash128 == hsOut.WriteHash128);
            Assert.True(hsIn.ReadHash32 == hsOut.WriteHash32);
            Assert.True(hsIn.ReadHash64 == hsOut.WriteHash64);
        }
        [Test]
        public void LongConst()
        {
            FileStream fs;
            MemoryStream ms;
            GetStreams(out fs, out ms);
            var hsIn = new HashedStream(fs, 42L, 53L);
            var hsOut = new HashedStream(ms, 42L, 53L);
            WriteOut(hsIn, hsOut);
            Assert.False(hsIn.WasMoved);
            Assert.True(hsIn.ReadHash128 == hsOut.WriteHash128);
            Assert.True(hsIn.ReadHash32 == hsOut.WriteHash32);
            Assert.True(hsIn.ReadHash64 == hsOut.WriteHash64);
        }
        [Test]
        public void LongDiffConst()
        {
            FileStream fs;
            MemoryStream ms;
            GetStreams(out fs, out ms);
            var hsIn = new HashedStream(fs, 42L, 53L, 23L, 34L);
            var hsOut = new HashedStream(ms, 42L, 53L, 23L, 34L);
            WriteOut(hsIn, hsOut);
            Assert.False(hsIn.WasMoved);
            Assert.False(hsIn.ReadHash128 == hsOut.WriteHash128);
            Assert.False(hsIn.ReadHash32 == hsOut.WriteHash32);
            Assert.False(hsIn.ReadHash64 == hsOut.WriteHash64);
        }
        [Test]
        public void MoveStream()
        {
            using(var hsOut = new HashedStream(new MemoryStream()))
            using(var tw = new StreamWriter(hsOut))
            {
                string asciiOnlyString = "Something or other";
                tw.Write(asciiOnlyString);
                tw.Flush();
                Assert.AreEqual(asciiOnlyString.Length, hsOut.Length);
                Assert.AreEqual(asciiOnlyString.Length, hsOut.Position);
                Assert.IsTrue(hsOut.CanSeek);
                hsOut.Seek(0, SeekOrigin.Begin);
                Assert.IsTrue(hsOut.WasMoved);
            }
            using(var hsOut = new HashedStream(new MemoryStream()))
            using(var tw = new StreamWriter(hsOut))
            {
                tw.Write("Something or other");
                hsOut.SetLength(0);
                Assert.IsTrue(hsOut.WasMoved);
            }
            using(var hsOut = new HashedStream(new MemoryStream()))
            using(var tw = new StreamWriter(hsOut))
            {
                tw.Write("Something or other");
                Assert.IsTrue(hsOut.CanSeek);
                hsOut.Position = 0;
                Assert.IsTrue(hsOut.WasMoved);
            }
        }
        [Test]
        public void TimeoutRead()
        {
            WebRequest wr = WebRequest.Create("http://www.google.com/");
            using(var rsp = wr.GetResponse())
            {
                var stm = rsp.GetResponseStream();
                using(var hs = new HashedStream(stm))
                {
                    Assert.AreEqual(stm.ReadTimeout, hs.ReadTimeout);
                    hs.ReadTimeout = stm.ReadTimeout;
                }
            }
        }
        [Test]
        public void TimeoutWrite()
        {
            WebRequest wr = WebRequest.Create("http://www.google.com/");
            wr.Method = "POST";
            var stm = wr.GetRequestStream();
            using(var hs = new HashedStream(stm))
            {
                Assert.AreEqual(stm.WriteTimeout, hs.WriteTimeout);
                hs.WriteTimeout = stm.WriteTimeout;
            }
        }
        [Test]
        [ExpectedException]
        public void NullStream()
        {
            new HashedStream(null);
        }
        [Test]
        public void HashExternal128()
        {
            HashCode128 hash;
            using(var hs = new HashedStream(GetFileStream()))
            {
                hash = SpookyHasher.SpookyHash128(hs);
                Assert.AreEqual(hash, hs.ReadHash128);
            }
            using(var fs = GetFileStream())
                Assert.AreEqual(hash, SpookyHasher.SpookyHash128(fs, 0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            using(var fs = GetFileStream())
                Assert.AreEqual(hash, unchecked(SpookyHasher.SpookyHash128(fs, (long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
        }
        [Test]
        public void HashExternal64()
        {
            long hash;
            using(var hs = new HashedStream(GetFileStream()))
            {
                hash = SpookyHasher.SpookyHash64(hs);
                Assert.AreEqual(hash, hs.ReadHash64);
            }
            using(var fs = GetFileStream())
                Assert.AreEqual((ulong)hash, SpookyHasher.SpookyHash64(fs, 0xDEADBEEFDEADBEEF));
            using(var fs = GetFileStream())
                Assert.AreEqual(hash, unchecked(SpookyHasher.SpookyHash64(fs, (long)0xDEADBEEFDEADBEEF)));
        }
        [Test]
        public void HashExternal32()
        {
            int hash;
            using(var fs = GetFileStream())
                hash = SpookyHasher.SpookyHash32(fs);
            using(var fs = GetFileStream())
                Assert.AreEqual(hash, SpookyHasher.SpookyHash32(fs, unchecked((int)0xDEADBEEF)));
            using(var fs = GetFileStream())
                Assert.AreEqual(hash, unchecked(SpookyHasher.SpookyHash32(fs, (int)0xDEADBEEF)));
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStream128()
        {
            SpookyHasher.SpookyHash128(((Stream)null));
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStreamS128()
        {
            SpookyHasher.SpookyHash128(((Stream)null), 1L, 1L);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStream64()
        {
            SpookyHasher.SpookyHash64(((Stream)null));
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStream32()
        {
            SpookyHasher.SpookyHash32(((Stream)null));
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStreamU32()
        {
            SpookyHasher.SpookyHash32(((Stream)null), 1U);
        }
    }
}
