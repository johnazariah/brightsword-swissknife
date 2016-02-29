using System;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class ValidatorTests
    {
        [Test]
        public void Test_CheckShouldNotThrowWhenSucceeded()
        {
            Func<bool> succeed = () => true;

            Assert.IsTrue(succeed.Check<ArgumentException>());
            Assert.IsTrue(succeed.Check(() => new ArgumentException("Something was false")), "Something was false");
        }

        [Test]
        public void Test_CheckShouldNotThrowWithTrue()
        {
            Assert.IsTrue(true.Check<ArgumentException>());
            Assert.IsTrue(true.Check(() => new ArgumentException("Something was false")), "Something was false");
        }

        [Test]
        public void Test_CheckShouldThrowWhenFailed()
        {
            Func<bool> fail = () => false;

            Assert.Throws<ArgumentException>(() => fail.Check<ArgumentException>());
            Assert.Throws<ArgumentException>(
                () => fail.Check(() => new ArgumentException("Something was false")),
                "Something was false");
        }

        [Test]
        public void Test_CheckShouldThrowWithFalse()
        {
            Assert.Throws<ArgumentException>(() => false.Check<ArgumentException>());
            Assert.Throws<ArgumentException>(
                () => false.Check(() => new ArgumentException("Something was false")),
                "Something was false");
        }
    }
}