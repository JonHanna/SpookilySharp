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
using System.Collections.Generic;
using SpookilySharp;
using Xunit;

namespace SpookyHashTesting
{
    public class EqualityComparerTests
    {
        private static IEnumerable<SpookyStringEqualityComparer> TestComparers
        {
            get
            {
                yield return new SpookyStringEqualityComparer();
                yield return new SpookyStringEqualityComparer(false);
                yield return new SpookyStringEqualityComparer(true);

                Random rand = new Random();
                for (int i = 0; i != 8; ++i)
                {
                    yield return new SpookyStringEqualityComparer(rand.Next(int.MinValue, int.MaxValue));
                }
            }
        }

        [Fact]
        public void NullsToZero()
        {
            foreach (SpookyStringEqualityComparer eq in TestComparers)
            {
                Assert.Equal(0, eq.GetHashCode((string)null));
                Assert.Equal(0, eq.GetHashCode((char[])null));
            }
        }

        [Fact]
        public void ConsistentHash()
        {
            SpookyStringEqualityComparer x = new SpookyStringEqualityComparer();
            SpookyStringEqualityComparer y = new SpookyStringEqualityComparer();
            Assert.Equal(x.GetHashCode("abcde"), y.GetHashCode("abcde"));
            Assert.Equal(x.GetHashCode("abcde".ToCharArray()), y.GetHashCode("abcde".ToCharArray()));
            Assert.Equal(x.GetHashCode("abcde"), y.GetHashCode("abcde".ToCharArray()));
            x = new SpookyStringEqualityComparer(0);
            y = new SpookyStringEqualityComparer(0);
            Assert.Equal(x.GetHashCode("abcde"), y.GetHashCode("abcde"));
            Assert.Equal(x.GetHashCode("abcde".ToCharArray()), y.GetHashCode("abcde".ToCharArray()));
            Assert.Equal(x.GetHashCode("abcde"), y.GetHashCode("abcde".ToCharArray()));
        }

        [Fact]
        public void EqualsTest()
        {
            foreach (SpookyStringEqualityComparer eq in TestComparers)
            {
                string source = "abcdefghi%©\"'faszעבריתಕನ್ನಡქართული";
                for (int i = 0; i <= source.Length; ++i)
                {
                    Assert.True(eq.Equals(source.Substring(0, i), source.Substring(0, i)));
                    Assert.True(eq.Equals(source.Substring(0, i).ToCharArray(), source.Substring(0, i).ToCharArray()));
                    Assert.True(eq.Equals(source.Substring(0, i), source.Substring(0, i).ToCharArray()));
                    Assert.True(eq.Equals(source.Substring(0, i).ToCharArray(), source.Substring(0, i)));
                }
            }
        }

        [Fact]
        public void NullsAndReferencesCorrect()
        {
            string str = "abcdefg";
            char[] arr = "abcdefg".ToCharArray();
            SpookyStringEqualityComparer eq = new SpookyStringEqualityComparer();
            Assert.True(eq.Equals(arr, arr));
            Assert.True(eq.Equals(default(string), default(string)));
            Assert.True(eq.Equals(default(string), default(char[])));
            Assert.True(eq.Equals(default(char[]), default(string)));
            Assert.True(eq.Equals(default(char[]), default(char[])));
            Assert.False(eq.Equals(arr, default(char[])));
            Assert.False(eq.Equals(default(char[]), arr));
            Assert.False(eq.Equals(arr, default(string)));
            Assert.False(eq.Equals(default(string), arr));
            Assert.False(eq.Equals(default(string), str));
            Assert.False(eq.Equals(str, default(string)));
            Assert.False(eq.Equals(str, default(char[])));
        }

        [Fact]
        public void SelfEquals()
        {
            SpookyStringEqualityComparer eq = new SpookyStringEqualityComparer(42);
            SpookyStringEqualityComparer eqX = new SpookyStringEqualityComparer(42);
            Assert.True(Equals(eq, eqX));
            HashSet<SpookyStringEqualityComparer> hset =
                new HashSet<SpookyStringEqualityComparer> {new SpookyStringEqualityComparer()};
            Assert.True(hset.Add(eq));
            Assert.False(hset.Add(eqX));
        }

        [Fact]
        public void NotEvenClose()
        {
            string x = "a";
            string y = "It was the best of times, it was the worse of times";
            SpookyStringEqualityComparer eq = new SpookyStringEqualityComparer();
            Assert.False(eq.Equals(x, y));
            Assert.False(eq.Equals(x, y.ToCharArray()));
            Assert.False(eq.Equals(x.ToCharArray(), y));
            Assert.False(eq.Equals(x.ToCharArray(), y.ToCharArray()));
        }

        [Fact]
        public void RollAlong()
        {
            char[] x = new char[65];
            char[] y = new char[65];
            SpookyStringEqualityComparer eq = new SpookyStringEqualityComparer();
            for (int i = 0; i != x.Length; ++i)
            {
                x[i] = (char)(i + 'a');
                Assert.False(eq.Equals(x, y));
                y[i] = (char)(i + 'a');
            }

            Assert.True(eq.Equals(x, y));
        }
    }
}
