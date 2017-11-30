﻿// WellDistributedHashAttributeTests.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014–2017 Jon Hanna
//
// Licensed under the MIT license. See the LICENSE file in the repository root for more details.

using System.Collections.Generic;
using SpookilySharp;
using Xunit;

namespace SpookyHashTesting
{
    public class WellDistributedHashAttributeTests
    {
        private class Bad : IEqualityComparer<int>
        {
            public bool Equals(int x, int y) => true;

            public virtual int GetHashCode(int obj) => 0;
        }

        // What a lie! This of course is exactly the sort of class that should least
        // have this attribute applied, but it serves our testing puroses.
        private class Good : IEqualityComparer<int>
        {
            public bool Equals(int x, int y) => true;

            [WellDistributedHash]
            public virtual int GetHashCode(int obj) => 0;
        }

        private sealed class InheritsBad : Bad
        {
        }

        private sealed class InheritsGood : Good
        {
        }

        private sealed class InheritsBadOverridesBad : Bad
        {
            public override int GetHashCode(int obj) => 0;
        }

        private sealed class InheritsBadOverridesGood : Bad
        {
            [WellDistributedHash]
            public override int GetHashCode(int obj) => 0;
        }

        private sealed class InheritsGoodOverridesBad : Good
        {
            public override int GetHashCode(int obj) => 0;
        }

        private sealed class InheritsGoodOverridesGood : Good
        {
            [WellDistributedHash]
            public override int GetHashCode(int obj) => 0;
        }

        private sealed class InheritsBadExpliticOverridesBad : Bad, IEqualityComparer<int>
        {
            int IEqualityComparer<int>.GetHashCode(int obj) => 0;
        }

        private sealed class InheritsBadExpliticOverridesGood : Bad, IEqualityComparer<int>
        {
            [WellDistributedHash]
            int IEqualityComparer<int>.GetHashCode(int obj) => 0;
        }

        private sealed class InheritsGoodExpliticOverridesBad : Good, IEqualityComparer<int>
        {
            int IEqualityComparer<int>.GetHashCode(int obj) => 0;
        }

        private sealed class InheritsGoodExpliticOverridesGood : Good, IEqualityComparer<int>
        {
            [WellDistributedHash]
            int IEqualityComparer<int>.GetHashCode(int obj) => 0;
        }

        private sealed class GoodButAlsoBad : IEqualityComparer<int>, IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => true;

            public int GetHashCode(string obj) => 0;

            public bool Equals(int x, int y) => true;

            [WellDistributedHash]
            public int GetHashCode(int obj) => 0;
        }

        private sealed class BadButAlsoGood : IEqualityComparer<int>, IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => true;

            [WellDistributedHash]
            public int GetHashCode(string obj) => 0;

            public bool Equals(int x, int y) => true;

            public int GetHashCode(int obj) => 0;
        }

        private sealed class ExplicitGood : IEqualityComparer<int>
        {
            bool IEqualityComparer<int>.Equals(int x, int y) => true;

            [WellDistributedHash]
            int IEqualityComparer<int>.GetHashCode(int obj) => 0;
        }

        private sealed class ExplicitBad : IEqualityComparer<int>
        {
            bool IEqualityComparer<int>.Equals(int x, int y) => true;

            int IEqualityComparer<int>.GetHashCode(int obj) => 0;
        }

        private class SelfGood
        {
            [WellDistributedHash]
            public override int GetHashCode() => 0;
        }

        private class SelfBadFromGood : SelfGood
        {
            public override int GetHashCode() => 0;
        }

        private class SelfGoodFromBad : SelfBadFromGood
        {
            [WellDistributedHash]
            public override int GetHashCode() => 0;
        }

        [Fact]
        public void TestAttributeDetection()
        {
            // Repeat so we both find it by calculation and by lookup.
            for (int i = 0; i != 3; ++i)
            {
                ConfirmBad(new Bad());
                ConfirmGood(new Good());
                ConfirmBad(new InheritsBad());
                ConfirmGood(new InheritsGood());
                ConfirmBad(new InheritsBadOverridesBad());
                ConfirmGood(new InheritsBadOverridesGood());
                ConfirmBad(new InheritsGoodOverridesBad());
                ConfirmGood(new InheritsGoodOverridesGood());
                ConfirmBad(new InheritsBadExpliticOverridesBad());
                ConfirmGood(new InheritsBadExpliticOverridesGood());
                ConfirmBad(new InheritsGoodExpliticOverridesBad());
                ConfirmGood(new InheritsGoodExpliticOverridesGood());
                ConfirmGood<int>(new GoodButAlsoBad());
                ConfirmBad<int>(new BadButAlsoGood());
                ConfirmBad<string>(new GoodButAlsoBad());
                ConfirmGood<string>(new BadButAlsoGood());
                ConfirmBad(new ExplicitBad());
                ConfirmGood(new ExplicitGood());
                ConfirmBad(EqualityComparer<int>.Default);
                ConfirmGood(EqualityComparer<SelfGood>.Default);
                ConfirmBad(EqualityComparer<SelfBadFromGood>.Default);
                ConfirmGood(EqualityComparer<SelfGoodFromBad>.Default);
                ConfirmBad(EqualityComparer<string>.Default);
            }
        }

        private static void ConfirmBad<T>(IEqualityComparer<T> cmp)
        {
            Assert.NotSame(cmp, cmp.WellDistributed());
        }

        private static void ConfirmGood<T>(IEqualityComparer<T> cmp)
        {
            Assert.Same(cmp, cmp.WellDistributed());
        }
    }
}
