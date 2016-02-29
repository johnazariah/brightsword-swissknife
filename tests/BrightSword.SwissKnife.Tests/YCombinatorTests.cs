using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class YCombinatorTests
    {
        [Test]
        public void TestFactorialRecursion()
        {
            var fact = Functional.Y<int, int>(
                f => x => x > 1
                              ? x*f(x - 1)
                              : 1);

            Assert.AreEqual(6, fact(3));
        }

        //notice that the function f is defined in terms of itself and the one parameter n
        [Test]
        public void TestFibonacciRecursion()
        {
            var fib = Functional.Y<int, int>(
                f => x => x > 1
                              ? f(x - 1) + f(x - 2)
                              : x);

            Assert.AreEqual(8, fib(6));
        }
    }
}