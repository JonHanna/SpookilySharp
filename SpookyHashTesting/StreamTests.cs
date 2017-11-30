﻿// StreamTests.cs
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
using System.Threading.Tasks;
using SpookilySharp;
using Xunit;

namespace SpookyHashTesting
{
    public class StreamTests
    {
        private static FileStream GetFileStream() => new FileStream("testfile.xml", FileMode.Open, FileAccess.Read, FileShare.Read);

        private static void WriteOut(HashedStream inStr, HashedStream outStr)
        {
            Random rand = new Random();
            using (inStr)
            using (outStr)
            {
                Assert.True(inStr.CanRead);
                Assert.True(outStr.CanWrite);
                if (inStr.CanTimeout)
                {
                    Assert.NotEqual(0, inStr.ReadTimeout);
                }
                if (outStr.CanTimeout)
                {
                    Assert.NotEqual(0, outStr.WriteTimeout);
                }
                for (;;)
                {
                    byte[] buffer = new byte[rand.Next(1, 20000)];
                    int read = inStr.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        return;
                    }

                    outStr.Write(buffer, 0, read);
                    int by = inStr.ReadByte();
                    if (by == -1)
                    {
                        return;
                    }

                    outStr.WriteByte((byte)by);
                }
            }
        }

        private static async Task WriteOutAsync(HashedStream inStr, HashedStream outStr)
        {
            Random rand = new Random();
            using (inStr)
            using (outStr)
            {
                Assert.True(inStr.CanRead);
                Assert.True(outStr.CanWrite);
                if (inStr.CanTimeout)
                {
                    Assert.NotEqual(0, inStr.ReadTimeout);
                }
                if (outStr.CanTimeout)
                {
                    Assert.NotEqual(0, outStr.WriteTimeout);
                }
                for (;;)
                {
                    byte[] buffer = new byte[rand.Next(1, 20000)];
                    int read = await inStr.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        return;
                    }

                    await outStr.WriteAsync(buffer, 0, read);
                    int by = inStr.ReadByte();
                    if (by == -1)
                    {
                        return;
                    }

                    outStr.WriteByte((byte)by);
                }
            }
        }

        [Fact]
        public void DefaultConst()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs);
            HashedStream hashOut = new HashedStream(ms);
            WriteOut(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.True(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.True(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.True(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public async Task DefaultConstAsync()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs);
            HashedStream hashOut = new HashedStream(ms);
            await WriteOutAsync(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.True(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.True(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.True(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public void ULongConst()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs, 42UL, 53UL);
            HashedStream hashOut = new HashedStream(ms, 42UL, 53UL);
            WriteOut(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.True(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.True(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.True(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public async Task ULongConstAsync()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs, 42UL, 53UL);
            HashedStream hashOut = new HashedStream(ms, 42UL, 53UL);
            await WriteOutAsync(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.True(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.True(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.True(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public void LongConst()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs, 42L, 53L);
            HashedStream hashOut = new HashedStream(ms, 42L, 53L);
            WriteOut(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.True(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.True(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.True(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public async Task LongConstAsync()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs, 42L, 53L);
            HashedStream hashOut = new HashedStream(ms, 42L, 53L);
            await WriteOutAsync(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.True(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.True(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.True(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public void LongDiffConst()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs, 42L, 53L, 23L, 34L);
            HashedStream hashOut = new HashedStream(ms, 42L, 53L, 23L, 34L);
            WriteOut(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.False(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.False(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.False(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public async Task LongDiffConstAsync()
        {
            FileStream fs = GetFileStream();
            Stream ms = Stream.Null;
            HashedStream hashIn = new HashedStream(fs, 42L, 53L, 23L, 34L);
            HashedStream hashOut = new HashedStream(ms, 42L, 53L, 23L, 34L);
            await WriteOutAsync(hashIn, hashOut);
            Assert.False(hashIn.WasMoved);
            Assert.False(hashIn.ReadHash128 == hashOut.WriteHash128);
            Assert.False(hashIn.ReadHash32 == hashOut.WriteHash32);
            Assert.False(hashIn.ReadHash64 == hashOut.WriteHash64);
        }

        [Fact]
        public void MoveStream()
        {
            using (HashedStream hashOut = new HashedStream(new MemoryStream()))
            using (StreamWriter tw = new StreamWriter(hashOut))
            {
                string asciiOnlyString = "Something or other";
                tw.Write(asciiOnlyString);
                tw.Flush();
                Assert.Equal(asciiOnlyString.Length, hashOut.Length);
                Assert.Equal(asciiOnlyString.Length, hashOut.Position);
                Assert.True(hashOut.CanSeek);
                hashOut.Seek(0, SeekOrigin.Begin);
                Assert.True(hashOut.WasMoved);
            }
            using (HashedStream hashOut = new HashedStream(new MemoryStream()))
            using (StreamWriter tw = new StreamWriter(hashOut))
            {
                tw.Write("Something or other");
                hashOut.SetLength(0);
                Assert.True(hashOut.WasMoved);
            }
            using (HashedStream hashOut = new HashedStream(new MemoryStream()))
            using (StreamWriter tw = new StreamWriter(hashOut))
            {
                tw.Write("Something or other");
                Assert.True(hashOut.CanSeek);
                hashOut.Position = 0;
                Assert.True(hashOut.WasMoved);
            }
        }

        [Fact]
        public async Task MoveStreamAsync()
        {
            using (HashedStream hashOut = new HashedStream(new MemoryStream()))
            using (StreamWriter tw = new StreamWriter(hashOut))
            {
                string asciiOnlyString = "Something or other";
                await tw.WriteAsync(asciiOnlyString);
                await tw.FlushAsync();
                Assert.Equal(asciiOnlyString.Length, hashOut.Length);
                Assert.Equal(asciiOnlyString.Length, hashOut.Position);
                Assert.True(hashOut.CanSeek);
                hashOut.Seek(0, SeekOrigin.Begin);
                Assert.True(hashOut.WasMoved);
            }
            using (HashedStream hashOut = new HashedStream(new MemoryStream()))
            using (StreamWriter tw = new StreamWriter(hashOut))
            {
                await tw.WriteAsync("Something or other");
                hashOut.SetLength(0);
                Assert.True(hashOut.WasMoved);
            }
            using (HashedStream hashOut = new HashedStream(new MemoryStream()))
            using (StreamWriter tw = new StreamWriter(hashOut))
            {
                await tw.WriteAsync("Something or other");
                Assert.True(hashOut.CanSeek);
                hashOut.Position = 0;
                Assert.True(hashOut.WasMoved);
            }
        }

        [Fact(Skip = "Need a new way to test this.")]
        public void TimeoutRead()
        {
            WebRequest wr = WebRequest.Create("http://www.google.com/");
            using (WebResponse rsp = wr.GetResponse())
            {
                Stream stm = rsp.GetResponseStream();
                using (HashedStream hs = new HashedStream(stm))
                {
                    Assert.Equal(stm.ReadTimeout, hs.ReadTimeout);
                    hs.ReadTimeout = stm.ReadTimeout;
                }
            }
        }

        [Fact(Skip = "Need a new way to test this.")]
        public void TimeoutWrite()
        {
            WebRequest wr = WebRequest.Create("http://www.google.com/");
            wr.Method = "POST";
            Stream stm = wr.GetRequestStream();
            using (HashedStream hs = new HashedStream(stm))
            {
                Assert.Equal(stm.WriteTimeout, hs.WriteTimeout);
                hs.WriteTimeout = stm.WriteTimeout;
            }
        }

        [Fact]
        public void NullStream()
        {
            Assert.Throws<ArgumentNullException>("stream", () => new HashedStream(null));
        }

        [Fact]
        public void HashExternal128()
        {
            HashCode128 hash;
            using (HashedStream hs = new HashedStream(GetFileStream()))
            {
                hash = hs.SpookyHash128();
                Assert.Equal(hash, hs.ReadHash128);
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, fs.SpookyHash128(0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(
                    hash,
                    unchecked(fs.SpookyHash128((long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            }
        }

        [Fact]
        public async Task HashExternal128Async()
        {
            HashCode128 hash;
            using (HashedStream hs = new HashedStream(GetFileStream()))
            {
                hash = await hs.SpookyHash128Async();
                Assert.Equal(hash, hs.ReadHash128);
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, await fs.SpookyHash128Async(0xDEADBEEFDEADBEEF, 0xDEADBEEFDEADBEEF));
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(
                    hash,
                    await unchecked(fs.SpookyHash128Async((long)0xDEADBEEFDEADBEEF, (long)0xDEADBEEFDEADBEEF)));
            }
        }

        [Fact]
        public void HashExternal64()
        {
            long hash;
            using (HashedStream hs = new HashedStream(GetFileStream()))
            {
                hash = hs.SpookyHash64();
                Assert.Equal(hash, hs.ReadHash64);
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal((ulong)hash, fs.SpookyHash64(0xDEADBEEFDEADBEEF));
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, unchecked(fs.SpookyHash64((long)0xDEADBEEFDEADBEEF)));
            }
        }

        [Fact]
        public async Task HashExternal64Async()
        {
            long hash;
            using (HashedStream hs = new HashedStream(GetFileStream()))
            {
                hash = await SpookyHasher.SpookyHash64Async(hs);
                Assert.Equal(hash, hs.ReadHash64);
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal((ulong)hash, await fs.SpookyHash64Async(0xDEADBEEFDEADBEEF));
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, unchecked(await fs.SpookyHash64Async((long)0xDEADBEEFDEADBEEF)));
            }
        }

        [Fact]
        public void HashExternal32()
        {
            int hash;
            using (FileStream fs = GetFileStream())
            {
                hash = fs.SpookyHash32();
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, fs.SpookyHash32(unchecked((int)0xDEADBEEF)));
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, unchecked(fs.SpookyHash32((int)0xDEADBEEF)));
            }
        }

        [Fact]
        public async Task HashExternal32Async()
        {
            int hash;
            using (FileStream fs = GetFileStream())
            {
                hash = await fs.SpookyHash32Async();
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, await fs.SpookyHash32Async(unchecked((int)0xDEADBEEF)));
            }
            using (FileStream fs = GetFileStream())
            {
                Assert.Equal(hash, unchecked(await fs.SpookyHash32Async((int)0xDEADBEEF)));
            }
        }

        [Fact]
        public void NullStream128()
        {
            Assert.Throws<ArgumentNullException>("stream", () => ((Stream)null).SpookyHash128());
        }

        [Fact]
        public void NullStreamS128()
        {
            Assert.Throws<ArgumentNullException>("stream", () => ((Stream)null).SpookyHash128(1L, 1L));
        }

        [Fact]
        public void NullStream64()
        {
            Assert.Throws<ArgumentNullException>("stream", () => ((Stream)null).SpookyHash64());
        }

        [Fact]
        public void NullStream32()
        {
            Assert.Throws<ArgumentNullException>("stream", () => ((Stream)null).SpookyHash32());
        }

        [Fact]
        public void NullStreamU32()
        {
            Assert.Throws<ArgumentNullException>("stream", () => SpookyHasher.SpookyHash32(null, 1U));
        }
    }
}
