using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class WhenMonadExtensionsTest
    {
        [Test]
        public void TestWhenActionWithNonNullValueAndFalse()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => false, _ => { }));
        }

        [Test]
        public void TestWhenActionWithNonNullValueAndTrue()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => true, _ => { }));
        }

        [Test]
        public void TestWhenActionWithNullValueAndFalse()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = default(string);

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => false, _ => { }));
        }

        [Test]
        public void TestWhenActionWithNullValueAndTrue()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => true, _ => { }));
        }

        [Test]
        public void TestWhenFuncWithNonNullValueNoDefaultAndFalse()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = default(string);

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => false, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestWhenFuncWithNonNullValueNoDefaultAndTrue()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = "Hello John";

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => true, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestWhenFuncWithNonNullValueWithDefaultAndFalse()
        {
            const string C_INPUT = "Hello World";
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = C_DEFAULT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => false, _ => _.Replace("World", "John"), C_DEFAULT));
        }

        [Test]
        public void TestWhenFuncWithNonNullValueWithDefaultAndTrue()
        {
            const string C_INPUT = "Hello World";
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = "Hello John";

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => true, _ => _.Replace("World", "John"), C_DEFAULT));
        }

        [Test]
        public void TestWhenFuncWithNullValueNoDefaultAndFalse()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = default(string);

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => false, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestWhenFuncWithNullValueNoDefaultAndTrue()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => true, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestWhenFuncWithNullValueWithDefaultAndFalse()
        {
            const string C_INPUT = null;
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = C_DEFAULT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => false, _ => _.Replace("World", "John"), C_DEFAULT));
        }

        [Test]
        public void TestWhenFuncWithNullValueWithDefaultAndTrue()
        {
            const string C_INPUT = null;
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = C_DEFAULT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.When(_ => true, _ => _.Replace("World", "John"), C_DEFAULT));
        }
    }
}