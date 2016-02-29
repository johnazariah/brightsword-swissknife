using System;
using System.Diagnostics.CodeAnalysis;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class ByteArrayReverseTests
    {
        private const long C_TEST_MIN = -10000000L;
        private const long C_TEST_MAX = 10000000L;

        private static byte[] GetReversedBytesSlow(long value)
        {
            var rgb = BitConverter.GetBytes(value);
            Array.Reverse(rgb);

            return rgb;
        }

        private static void EnsureArraysAreIdentical(byte[] expected, byte[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0;
                 i < expected.Length;
                 i++) { Assert.AreEqual(expected[i], actual[i]); }
        }

        [Test]
        [ExcludeFromCodeCoverage]
        public void TestBitTwiddlerIsFaster()
        {
            GetReversedBytesSlow(100L);
            100L.GetReversedBytes();

            var timeStartArrayReverse = DateTime.Now;
            for (var l = C_TEST_MIN;
                 l < C_TEST_MAX;
                 l++) { GetReversedBytesSlow(l); }
            var timeTakenForArrayReverse = DateTime.Now - timeStartArrayReverse;

            var timeStartBitTwiddling = DateTime.Now;
            for (var l = C_TEST_MIN;
                 l < C_TEST_MAX;
                 l++) { l.GetReversedBytes(); }
            var timeTakenForBitTwiddling = DateTime.Now - timeStartBitTwiddling;

            Assert.IsTrue(timeTakenForBitTwiddling < timeTakenForArrayReverse);
        }

        [Test]
        public void TestReverseBytes64Bit()
        {
            for (var l = C_TEST_MIN;
                 l < C_TEST_MAX;
                 l++) { EnsureArraysAreIdentical(GetReversedBytesSlow(l), l.GetReversedBytes()); }
        }
    }
}