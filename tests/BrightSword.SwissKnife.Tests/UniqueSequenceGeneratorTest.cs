using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class UniqueSequenceGeneratorTest
    {
        private const int C_MAX_ITEMS = 1000*1000;

        public static void TestSequenceIsUniqueMultiThreaded(Func<long> sequenceGenerator)
        {
            var items = new ConcurrentBag<long>();

            Parallel.For(
                0,
                C_MAX_ITEMS,
                () => 0,
                (_, __, ___) =>
                {
                    items.Add(sequenceGenerator());
                    return items.Count;
                },
                _ => { });

            Assert.AreEqual(C_MAX_ITEMS, items.Count);

            Assert.IsTrue(
                items.OrderBy(_g => _g.ToString(CultureInfo.InvariantCulture))
                     .ToList()
                     .SortedListIsUnique());
        }

        [Test]
        public void Test_DecreasingSequenceIsDecreasing()
        {
            var sequence = UniqueSequenceGenerator.GenerateDecreasingSequence(100*1000)
                                                  .ToList();

            Assert.IsTrue(
                sequence.Select((_item, _index) => _index == 0 || sequence[_index - 1] > _item)
                        .All(_ => _));
            Assert.IsTrue(sequence.AllUnique());
        }

        [Test]
        public void Test_DecreasingSequenceIsDecreasingMT()
        {
            TestSequenceIsUniqueMultiThreaded(() => UniqueSequenceGenerator.NextDescendingUniqueValue);
        }

        [Test]
        public void Test_DecreasingSequenceIsUnique()
        {
            var sequence = UniqueSequenceGenerator.GenerateDecreasingSequence(100*1000);
            Assert.IsTrue(sequence.AllUnique());
        }

        [Test]
        public void Test_IncreasingSequenceIsIncreasing()
        {
            var sequence = UniqueSequenceGenerator.GenerateIncreasingSequence(100*1000)
                                                  .ToList();

            Assert.IsTrue(
                sequence.Select((_item, _index) => _index == 0 || sequence[_index - 1] < _item)
                        .All(_ => _));
            Assert.IsTrue(sequence.AllUnique());
        }

        [Test]
        public void Test_IncreasingSequenceIsIncreasingMT()
        {
            TestSequenceIsUniqueMultiThreaded(() => UniqueSequenceGenerator.NextAscendingUniqueValue);
        }

        [Test]
        public void Test_IncreasingSequenceIsUnique()
        {
            var sequence = UniqueSequenceGenerator.GenerateIncreasingSequence(100*1000);
            Assert.IsTrue(sequence.AllUnique());
        }

        [Test]
        public void Test_SequenceWithNonNegativeLengthHasCorrectLength()
        {
            Assert.AreEqual(
                0,
                UniqueSequenceGenerator.GenerateDecreasingSequence(0)
                                       .Count());
            Assert.AreEqual(
                0,
                UniqueSequenceGenerator.GenerateIncreasingSequence(0)
                                       .Count());
            Assert.AreEqual(
                10,
                UniqueSequenceGenerator.GenerateDecreasingSequence(10)
                                       .Count());
            Assert.AreEqual(
                10,
                UniqueSequenceGenerator.GenerateIncreasingSequence(10)
                                       .Count());
        }

        [Test]
        public void Test_SequenceWithNonsensicalLengthThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    var foo = UniqueSequenceGenerator.GenerateDecreasingSequence(-1)
                                                     .ToList();
                });
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    var foo = UniqueSequenceGenerator.GenerateIncreasingSequence(-1)
                                                     .ToList();
                });
        }
    }
}