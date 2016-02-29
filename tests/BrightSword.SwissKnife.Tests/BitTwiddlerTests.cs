using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class BitTwiddlerTests
    {
        [Test]
        public void Test_GetReversedBytes_Long()
        {
            const long input = 314159L;
            var expected = new byte[]
                           {
                               0,
                               0,
                               0,
                               0,
                               0,
                               4,
                               203,
                               47
                           };
            var actual = input.GetReversedBytes();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void Test_GetReversedBytes_SignedLong_MaxValue()
        {
            const long input = long.MaxValue;
            var expected = new byte[]
                           {
                               127,
                               255,
                               255,
                               255,
                               255,
                               255,
                               255,
                               255
                           };
            var actual = input.GetReversedBytes();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void Test_GetReversedBytes_SignedLong_MinValue()
        {
            const long input = long.MinValue;
            var expected = new byte[]
                           {
                               128,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0
                           };
            var actual = input.GetReversedBytes();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void Test_GetReversedBytes_UnsignedLong_MaxValue()
        {
            const ulong input = ulong.MaxValue;
            var expected = new byte[]
                           {
                               255,
                               255,
                               255,
                               255,
                               255,
                               255,
                               255,
                               255
                           };
            var actual = input.GetReversedBytes();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void Test_GetReversedBytes_UnsignedLong_MinValue()
        {
            const ulong input = ulong.MinValue;
            var expected = new byte[]
                           {
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0
                           };
            var actual = input.GetReversedBytes();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}