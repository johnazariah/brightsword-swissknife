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
    [TestFixture]
    public class ReverseSequentialGuidTests
    {
        private const int C_MAX_GUIDS = 1000*1000;

        [Test]
        public void PrintAFewReverseSequentialGuids()
        {
            for (var i = 0;
                 i < 25;
                 i++) {
                     Trace.WriteLine(SequentialGuid.NewReverseSequentialGuid());
                 }
        }

        [Test]
        public void TestGuidsAreUnique()
        {
            var _guids = new List<Guid>(C_MAX_GUIDS);

            for (var i = 0;
                 i < C_MAX_GUIDS;
                 i++) {
                     _guids.Add(SequentialGuid.NewReverseSequentialGuid());
                 }

            var guids = from _g in _guids
                        orderby _g.ToString()
                        select _g;

            Assert.IsTrue(
                guids.ToList()
                     .SortedListIsUnique());
        }

        [Test]
        public void TestGuidsAreUniqueMultiThreaded()
        {
            var _guids = new ConcurrentBag<Guid>();

            Parallel.For(
                0,
                C_MAX_GUIDS,
                () => 0,
                (_, __, ___) =>
                {
                    _guids.Add(SequentialGuid.NewReverseSequentialGuid());
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

        [Test]
        public void TestNewGuidPerformance()
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
                     SequentialGuid.NewReverseSequentialGuid();
                 }

            // measure ReverseSequential guid creation
            sw.Start();
            for (var i = 0;
                 i < C_MAX_GUIDS;
                 i++) {
                     SequentialGuid.NewReverseSequentialGuid();
                 }
            sw.Stop();

            var timeTakenForReverseSequentialGuids = sw.ElapsedMilliseconds;

            Trace.WriteLine(
                $"Creating {C_MAX_GUIDS} ReverseSequential Guids took {timeTakenForReverseSequentialGuids} ms");

            //// ReverseSequential guids should be faster
            //Assert.IsTrue(timeTakenForReverseSequentialGuids < timeTakenForNormalGuids);
        }
    }
}