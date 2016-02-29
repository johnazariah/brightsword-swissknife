using System;
using System.Diagnostics;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class MemoizeTests
    {
        private static readonly Func<int, double> Factorial = (n => n < 2
                                                                        ? 1.0d
                                                                        : n*Factorial(n - 1));

        private static readonly Func<int, long> FastFibonacci = Functional.MemoizeFix<int, long>(
            fib => n => (n <= 0
                             ? 0
                             : (n <= 2
                                    ? 1
                                    : fib(n - 1) + fib(n - 2))));

        private static readonly Func<int, long> FastLucas =
            Functional.MemoizeFix<int, long>(luc => (n => FastFibonacci(n) + FastFibonacci(n + 2)));

        private static readonly Func<int, double> FastFactorial = Functional.MemoizeFix<int, double>(
            fact => n => n < 2
                             ? 1.0d
                             : n*fact(n - 1));

        private static readonly Func<int, double> FakeMemoizedFactorial = Factorial.Memoize();

        private static long SlowFibonacci(int n, ref int callCount)
        {
            ++callCount;
            return (n <= 0
                        ? 0
                        : (n <= 2
                               ? 1
                               : SlowFibonacci(n - 1, ref callCount) + SlowFibonacci(n - 2, ref callCount)));
        }

        private static long SlowLucas(int n, ref int callCount)
        {
            return SlowFibonacci(n, ref callCount) + SlowFibonacci(n + 2, ref callCount);
        }

        private static double SlowFactorial(int n, ref int callCount)
        {
            ++callCount;
            return n < 2
                       ? 1
                       : n*SlowFactorial(n - 1, ref callCount);
        }

        [Test]
        public void TestFactorialCorrectness()
        {
            for (var i = 0;
                 i < 40;
                 i++)
            {
                var stopWatch = Stopwatch.StartNew();
                var _callCount = 0;
                var slowResult = SlowFactorial(i, ref _callCount);
                stopWatch.Stop();
                var slowTiming = stopWatch.ElapsedTicks;

                stopWatch.Reset();
                var fastResult = FastFactorial(i);
                stopWatch.Stop();
                var fastTiming = stopWatch.ElapsedTicks;

                stopWatch.Reset();
                var fakeResult = FakeMemoizedFactorial(i);
                stopWatch.Stop();
                var fakeTiming = stopWatch.ElapsedTicks;

                Assert.AreEqual(slowResult, fastResult);
                Assert.AreEqual(slowResult, fakeResult);

                Assert.IsTrue(slowTiming >= fastTiming);
                Assert.IsTrue(fakeTiming >= fastTiming);

                Trace.WriteLine(
                    $"Fact({i,-2}) = {fastResult,18}\t Fast: {fastTiming,4}\t Slow: {slowTiming,4}\t Fake: {fakeTiming,4} (ticks)");
            }
        }

        [Test]
        public void TestFibonacciCorrectness()
        {
            for (var i = 0;
                 i < 30;
                 i++)
            {
                var stopWatch = Stopwatch.StartNew();
                var _callCount = 0;
                var slowResult = SlowFibonacci(i, ref _callCount);
                stopWatch.Stop();
                var slowTiming = stopWatch.ElapsedTicks;

                stopWatch.Reset();
                var fastResult = FastFibonacci(i);
                stopWatch.Stop();
                var fastTiming = stopWatch.ElapsedTicks;

                Assert.AreEqual(slowResult, fastResult);
                Assert.IsTrue(slowTiming >= fastTiming);

                Trace.WriteLine(
                    $"Fib({i,-2})  = {fastResult,12}\t Fast: {fastTiming,4}\t Slow: {slowTiming,10} ticks, {_callCount,10} calls");
            }
        }

        [Test]
        public void TestLucasCorrectness()
        {
            for (var i = 0;
                 i < 30;
                 i++)
            {
                var stopWatch = Stopwatch.StartNew();
                var _callCount = 0;
                var slowResult = SlowLucas(i, ref _callCount);
                stopWatch.Stop();
                var slowTiming = stopWatch.ElapsedTicks;

                stopWatch.Reset();
                var fastResult = FastLucas(i);
                stopWatch.Stop();
                var fastTiming = stopWatch.ElapsedTicks;

                Assert.AreEqual(slowResult, fastResult);
                Assert.IsTrue(slowTiming >= fastTiming);

                Trace.WriteLine(
                    $"Luc({i,-2})  = {fastResult,12}\t Fast: {fastTiming,4}\t Slow: {slowTiming,10} (ticks)");
            }
        }

        [Test]
        public void TestThatMemoizingTwoFunctionsDoesNotClash()
        {
            for (var i = 0;
                 i < 10;
                 i++)
            {
                var _callCount = 0;
                Assert.AreEqual(SlowFibonacci(i, ref _callCount), FastFibonacci(i));
                Assert.AreEqual(SlowLucas(i, ref _callCount), FastLucas(i));
            }
        }
    }
}