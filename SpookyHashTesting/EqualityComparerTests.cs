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
    [TestFixture()]
    public class EqualityComparerTests
    {
        private static IEnumerable<SpookyEqualityComparer> TestComparers
        {
            get
            {
                yield return new SpookyEqualityComparer();
                yield return new SpookyEqualityComparer(false);
                yield return new SpookyEqualityComparer(true);
                Random rand = new Random();
                for(int i = 0; i != 8; ++i)
                    yield return new SpookyEqualityComparer(rand.Next(int.MinValue, int.MaxValue));
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
            var x = new SpookyEqualityComparer();
            var y = new SpookyEqualityComparer();
            Assert.AreEqual(x.GetHashCode("abcde"), y.GetHashCode("abcde"));
            Assert.AreEqual(x.GetHashCode("abcde".ToCharArray()), y.GetHashCode("abcde".ToCharArray()));
            Assert.AreEqual(x.GetHashCode("abcde"), y.GetHashCode("abcde".ToCharArray()));
            x = new SpookyEqualityComparer(0);
            y = new SpookyEqualityComparer(0);
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
    }
}