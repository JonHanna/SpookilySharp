﻿// WellDistributedHashAttributeTests.cs
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
using NUnit.Framework;
using SpookilySharp;

namespace SpookyHashTesting
{
    [TestFixture]
    public class WellDistributedHashAttributeTests
    {
        private class Bad : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return true;
            }
            public virtual int GetHashCode(int obj)
            {
                return 0;
            }
        }
        // What a lie! This of course is exactly the sort of class that should least
        // have this attribute applied, but it serves our testing puroses.
        private class Good : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return true;
            }
            [WellDistributedHash]
            public virtual int GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsBad : Bad
        {
        }
        private class InheritsGood : Good
        {
        }
        private class InheritsBadOverridesBad : Bad
        {
            public override int GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsBadOverridesGood : Bad
        {
            [WellDistributedHash]
            public override int GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsGoodOverridesBad : Good
        {
            public override int GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsGoodOverridesGood : Good
        {
            [WellDistributedHash]
            public override int GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsBadExpliticOverridesBad : Bad, IEqualityComparer<int>
        {
            int IEqualityComparer<int>.GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsBadExpliticOverridesGood : Bad, IEqualityComparer<int>
        {
            [WellDistributedHash]
            int IEqualityComparer<int>.GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsGoodExpliticOverridesBad : Good, IEqualityComparer<int>
        {
            int IEqualityComparer<int>.GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class InheritsGoodExpliticOverridesGood : Good, IEqualityComparer<int>
        {
            [WellDistributedHash]
            int IEqualityComparer<int>.GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class GoodButAlsoBad : IEqualityComparer<int>, IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return true;
            }
            public int GetHashCode(string obj)
            {
                return 0;
            }
            public bool Equals(int x, int y)
            {
                return true;
            }
            [WellDistributedHash]
            public int GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class BadButAlsoGood : IEqualityComparer<int>, IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return true;
            }
            [WellDistributedHash]
            public int GetHashCode(string obj)
            {
                return 0;
            }
            public bool Equals(int x, int y)
            {
                return true;
            }
            public int GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class ExplicitGood : IEqualityComparer<int>
        {
            bool IEqualityComparer<int>.Equals(int x, int y)
            {
                return true;
            }
            [WellDistributedHash]
            int IEqualityComparer<int>.GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class ExplicitBad : IEqualityComparer<int>
        {
            bool IEqualityComparer<int>.Equals(int x, int y)
            {
                return true;
            }
            int IEqualityComparer<int>.GetHashCode(int obj)
            {
                return 0;
            }
        }
        private class SelfGood
        {
            [WellDistributedHash]
            public override int GetHashCode()
            {
                return 0;
            }
        }
        private class SelfBadFromGood : SelfGood
        {
            public override int GetHashCode()
            {
                return 0;
            }
        }
        private class SelfGoodFromBad : SelfBadFromGood
        {
            [WellDistributedHash]
            public override int GetHashCode()
            {
                return 0;
            }
        }
        [Test]
        public void TestAttributeDetection()
        {
            for(int i = 0; i != 3; ++i)//Repeat so we both find it by calculation and by lookup.
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
        private void ConfirmBad<T>(IEqualityComparer<T> cmp)
        {
            Assert.AreNotSame(cmp, SpookierEqualityComparers.WellDistributed(cmp));
        }
        private void ConfirmGood<T>(IEqualityComparer<T> cmp)
        {
            Assert.AreSame(cmp, SpookierEqualityComparers.WellDistributed(cmp));
        }
    }
}