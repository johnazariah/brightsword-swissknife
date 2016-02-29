using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    internal static class SequentialGuidTestHelper
    {
        private const int C_MAX_GUIDS = 1000*1000;

        public static void TestGuidsAreUnique(Func<Guid> guidGenerator)
        {
            var _guids = new List<Guid>(C_MAX_GUIDS);

            for (var i = 0;
                 i < C_MAX_GUIDS;
                 i++) {
                     _guids.Add(guidGenerator());
                 }

            var guids = from _g in _guids
                        orderby _g.ToString()
                        select _g;

            Assert.IsTrue(
                guids.ToList()
                     .SortedListIsUnique());
        }

        public static void TestGuidsAreUniqueMultiThreaded(Func<Guid> guidGenerator)
        {
            var _guids = new ConcurrentBag<Guid>();

            Parallel.For(
                0,
                C_MAX_GUIDS,
                () => 0,
                (_, __, ___) =>
                {
                    _guids.Add(guidGenerator());
                    return _guids.Count;
                },
                _ => { });

            Assert.AreEqual(C_MAX_GUIDS, _guids.Count);

            var guids = from _g in _guids
                        orderby _g.ToString()
                        select _g;

            Assert.IsTrue(
                guids.ToList()
                     .SortedListIsUnique());
        }

        public static void TestNewGuidPerformance(Func<Guid> guidGenerator)
        {
            // prime the pump before measuring
            for (var i = 0;
                 i < 10;
                 i++) {
                     Guid.NewGuid();
                 }

            var sw = new Stopwatch();
            // measure normal guid creation
            sw.Start();
            for (var i = 0;
                 i < C_MAX_GUIDS;
                 i++) {
                     Guid.NewGuid();
                 }
            sw.Stop();

            var timeTakenForNormalGuids = sw.ElapsedMilliseconds;

            Trace.WriteLine($"Creating {C_MAX_GUIDS} Normal Guids took {timeTakenForNormalGuids} ms");

            sw.Reset();

            // prime the pump before measuring
            for (var i = 0;
                 i < 10;
                 i++) {
                     guidGenerator();
                 }

            // measure sequential guid creation
            sw.Start();
            for (var i = 0;
                 i < C_MAX_GUIDS;
                 i++) {
                     guidGenerator();
                 }
            sw.Stop();

            var timeTakenForSequentialGuids = sw.ElapsedMilliseconds;

            Trace.WriteLine($"Creating {C_MAX_GUIDS} Sequential Guids took {timeTakenForSequentialGuids} ms");

            //// sequential guids should be faster
            //Assert.IsTrue(timeTakenForSequentialGuids < timeTakenForNormalGuids);
        }

        public static void PrintAFewSequentialGuids(Func<Guid> guidGenerator)
        {
            for (var i = 0;
                 i < 25;
                 i++) {
                     Trace.WriteLine(guidGenerator());
                 }
        }
    }

    [TestFixture]
    public class SequentialGuidTests
    {
        [Test]
        public void PrintAFewSequentialGuidsForward()
        {
            SequentialGuidTestHelper.PrintAFewSequentialGuids(() => SequentialGuid.NewSequentialGuid());
        }

        [Test]
        public void PrintAFewSequentialGuidsReverse()
        {
            SequentialGuidTestHelper.PrintAFewSequentialGuids(() => SequentialGuid.NewReverseSequentialGuid());
        }

        [Test]
        public void TestGuidsAreUniqueForward()
        {
            SequentialGuidTestHelper.TestGuidsAreUnique(() => SequentialGuid.NewSequentialGuid());
        }

        [Test]
        public void TestGuidsAreUniqueForwardSeeded()
        {
            SequentialGuidTestHelper.TestGuidsAreUnique(() => SequentialGuid.NewSequentialGuid(10, 10, 4));
        }

        [Test]
        public void TestGuidsAreUniqueMultiThreadedForward()
        {
            SequentialGuidTestHelper.TestGuidsAreUniqueMultiThreaded(() => SequentialGuid.NewSequentialGuid());
        }

        [Test]
        public void TestGuidsAreUniqueMultiThreadedReverse()
        {
            SequentialGuidTestHelper.TestGuidsAreUniqueMultiThreaded(() => SequentialGuid.NewReverseSequentialGuid());
        }

        [Test]
        public void TestGuidsAreUniqueReverse()
        {
            SequentialGuidTestHelper.TestGuidsAreUnique(() => SequentialGuid.NewReverseSequentialGuid());
        }

        [Test]
        public void TestGuidsAreUniqueReverseSeeded()
        {
            SequentialGuidTestHelper.TestGuidsAreUnique(() => SequentialGuid.NewReverseSequentialGuid(10, 10, 4));
        }

        [Test]
        public void TestNewGuidPerformanceForward()
        {
            SequentialGuidTestHelper.TestNewGuidPerformance(() => SequentialGuid.NewSequentialGuid());
        }

        [Test]
        public void TestNewGuidPerformanceReverse()
        {
            SequentialGuidTestHelper.TestNewGuidPerformance(() => SequentialGuid.NewReverseSequentialGuid());
        }
    }
}