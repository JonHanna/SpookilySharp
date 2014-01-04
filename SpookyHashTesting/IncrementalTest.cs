// Test.cs
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

// Based on Bob Jenkins’ tests.

using System;
using System.Collections.Generic;
using System.Linq;
using SpookilySharp;
using NUnit.Framework;

namespace SpookyHashTesting
{

    [TestFixture]
    
    public class IncrementalTest
    {
        private const int BUFSIZE = 1024;
        [Test]
        public unsafe void TestPieces()
        {
            var bufArr = new byte[BUFSIZE];
            for (int i=0; i<BUFSIZE; ++i)
            {
                bufArr[i] = unchecked((byte)i);
            }
            for (int i=0; i<BUFSIZE; ++i)
            {
                ulong a,b,c,d,seed1=1,seed2=2;
                var state = new SpookyHash();

                // all as one call
                a = seed1;
                b = seed2;
                fixed(byte* buf = bufArr)
                    SpookyHash.Hash128(buf, i, ref a, ref b);

                // all as one piece
                c = 0xdeadbeefdeadbeef;
                d = 0xbaceba11baceba11;
                state.Init(seed1, seed2);
                fixed(byte* buf = bufArr)
                    state.Update(buf, i);
                state.Final(out c, out d);

                Assert.AreEqual(a, c, "A-C mismatch at: " + i);
                Assert.AreEqual(b, d, "B-D mismatch at: " + i);

                for (int j=0; j<i; ++j)
                {
                    c = seed1;
                    d = seed2;
                    state.Init(c, d);
                    fixed(byte* buf = bufArr)
                    {
                        state.Update(buf, j);
                        state.Update(buf + j, i - j);
                    }
                    state.Final(out c, out d);
                    Assert.AreEqual(a, c, "A-C mismatch at: " + i + "," + j);
                    Assert.AreEqual(b, d, "B-D mismatch at: " + i + "," + j);
                }
            }
        }
    }
}