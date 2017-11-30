// IncrementalTest.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014–2017 Jon Hanna
//
// Licensed under the MIT license. See the LICENSE file in the repository root for more details.

// Based on Bob Jenkins’ tests.

using SpookilySharp;
using Xunit;

namespace SpookyHashTesting
{
    public class IncrementalTest
    {
        private const int BufferSize = 1024;

        [Fact]
        public unsafe void TestPieces()
        {
            byte[] bufArr = new byte[BufferSize];
            for (int i = 0; i < BufferSize; ++i)
            {
                bufArr[i] = unchecked((byte)i);
            }
            for (int i = 0; i < BufferSize; ++i)
            {
                ulong a, b, c, d, seed1 = 1, seed2 = 2;
                SpookyHash state = new SpookyHash();

                // all as one call
                a = seed1;
                b = seed2;
                fixed (byte* buf = bufArr)
                {
                    SpookyHash.Hash128(buf, i, ref a, ref b);
                }

                // all as one piece
                c = 0xdeadbeefdeadbeef;
                d = 0xbaceba11baceba11;
                state.Init(seed1, seed2);
                fixed (byte* buf = bufArr)
                {
                    state.Update(buf, i);
                }
                state.Final(out c, out d);

                Assert.Equal(a, c);
                Assert.Equal(b, d);

                for (int j = 0; j < i; ++j)
                {
                    c = seed1;
                    d = seed2;
                    state.Init(c, d);
                    fixed (byte* buf = bufArr)
                    {
                        state.Update(buf, j);
                        state.Update(buf + j, i - j);
                    }
                    state.Final(out c, out d);
                    Assert.Equal(a, c);
                    Assert.Equal(b, d);
                }
            }
        }
    }
}
