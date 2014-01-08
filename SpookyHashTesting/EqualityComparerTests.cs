// EqualityComparerTests.cs
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
using NUnit.Framework;
using SpookilySharp;
using System.Collections.Generic;

namespace SpookyHashTesting
{
    [TestFixture]
    public class EqualityComparerTests
    {
        private static IEnumerable<SpookyStringEqualityComparer> TestComparers
        {
            get
            {
                yield return new SpookyStringEqualityComparer();
                yield return new SpookyStringEqualityComparer(false);
                yield return new SpookyStringEqualityComparer(true);
                var rand = new Random();
                for(int i = 0; i != 8; ++i)
                    yield return new SpookyStringEqualityComparer(rand.Next(int.MinValue, int.MaxValue));
            }
        }
        [Test]
        public void NullsToZero()
        {
            foreach(var eq in TestComparers)
            {
                Assert.AreEqual(0, eq.GetHashCode((string)null));
                Assert.AreEqual(0, eq.GetHashCode((char[])null));
            }
        }
        [Test]
        public void ConsistentHash()
        {
            var x = new SpookyStringEqualityComparer();
            var y = new SpookyStringEqualityComparer();
            Assert.AreEqual(x.GetHashCode("abcde"), y.GetHashCode("abcde"));
            Assert.AreEqual(x.GetHashCode("abcde".ToCharArray()), y.GetHashCode("abcde".ToCharArray()));
            Assert.AreEqual(x.GetHashCode("abcde"), y.GetHashCode("abcde".ToCharArray()));
            x = new SpookyStringEqualityComparer(0);
            y = new SpookyStringEqualityComparer(0);
            Assert.AreEqual(x.GetHashCode("abcde"), y.GetHashCode("abcde"));
            Assert.AreEqual(x.GetHashCode("abcde".ToCharArray()), y.GetHashCode("abcde".ToCharArray()));
            Assert.AreEqual(x.GetHashCode("abcde"), y.GetHashCode("abcde".ToCharArray()));
        }
        [Test]
        public void Equals()
        {
            foreach(var eq in TestComparers)
            {
                string source = "abcdefghi%©\"'faszעבריתಕನ್ನಡქართული";
                for(int i = 0; i <= source.Length; ++i)
                {
                    Assert.IsTrue(eq.Equals(source.Substring(0, i), source.Substring(0, i)));
                    Assert.IsTrue(eq.Equals(source.Substring(0, i).ToCharArray(), source.Substring(0, i).ToCharArray()));
                    Assert.IsTrue(eq.Equals(source.Substring(0, i), source.Substring(0, i).ToCharArray()));
                    Assert.IsTrue(eq.Equals(source.Substring(0, i).ToCharArray(), source.Substring(0, i)));
                }
            }
        }
        [Test]
        public void NullsAndReferencesCorrect()
        {
            var str = "abcdefg";
            var arr = "abcdefg".ToCharArray();
            char[] nulArr = null;
            string nulStr = null; 
            var eq = new SpookyStringEqualityComparer();
            Assert.True(eq.Equals(arr, arr));
            Assert.True(eq.Equals(nulStr, nulStr));
            Assert.True(eq.Equals(nulStr, nulArr));
            Assert.True(eq.Equals(nulArr, nulStr));
            Assert.True(eq.Equals(nulArr, nulArr));
            Assert.False(eq.Equals(arr, nulArr));
            Assert.False(eq.Equals(nulArr, arr));
            Assert.False(eq.Equals(arr, nulStr));
            Assert.False(eq.Equals(nulStr, arr));
            Assert.False(eq.Equals(nulStr, str));
            Assert.False(eq.Equals(str, nulStr));
            Assert.False(eq.Equals(str, nulArr));
        }
        [Test]
        public void SelfEquals()
        {
            var eq = new SpookyStringEqualityComparer(42);
            var eqX = new SpookyStringEqualityComparer(42);
            Assert.True(object.Equals(eq, eqX));
            var hset = new HashSet<SpookyStringEqualityComparer>();
            hset.Add(new SpookyStringEqualityComparer());
            Assert.True(hset.Add(eq));
            Assert.False(hset.Add(eqX));
        }
        [Test]
        public void NotEvenClose()
        {
            string x = "a";
            string y = "It was the best of times, it was the worse of times";
            var eq = new SpookyStringEqualityComparer();
            Assert.False(eq.Equals(x, y));
            Assert.False(eq.Equals(x, y.ToCharArray()));
            Assert.False(eq.Equals(x.ToCharArray(), y));
            Assert.False(eq.Equals(x.ToCharArray(), y.ToCharArray()));
        }
        [Test]
        public void RollAlong()
        {
            var x = new char[65];
            var y = new char[65];
            var eq = new SpookyStringEqualityComparer();
            for(int i = 0; i != x.Length; ++i)
            {
                x[i] = (char)(i + (int)'a');
                Assert.False(eq.Equals(x, y));
                y[i] = (char)(i + (int)'a');
            }
            Assert.True(eq.Equals(x, y));
        }
    }
}