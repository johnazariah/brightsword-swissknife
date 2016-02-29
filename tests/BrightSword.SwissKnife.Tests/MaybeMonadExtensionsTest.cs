using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class MaybeMonadExtensionsTest
    {
        [Test]
        public void TestMaybeActionWithNonNullValue()
        {
            const string C_EXPECTED = "Hello World";

            Assert.AreEqual(C_EXPECTED, C_EXPECTED.Maybe(_ => { }));
        }

        [Test]
        public void TestMaybeActionWithNullValue()
        {
            const string C_EXPECTED = null;

            Assert.AreEqual(C_EXPECTED, C_EXPECTED.Maybe(_ => { }));
        }

        [Test]
        public void TestMaybeFuncWithNonNullValueNoDefault()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = "Hello John";

            Assert.AreEqual(C_EXPECTED, C_INPUT.Maybe(_ => _.Replace("World", "John")));
        }

        [Test]
        public void TestMaybeFuncWithNonNullValueWithDefault()
        {
            const string C_INPUT = "Hello World";
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = "Hello John";

            Assert.AreEqual(C_EXPECTED, C_INPUT.Maybe(_ => _.Replace("World", "John"), C_DEFAULT));
        }

        [Test]
        public void TestMaybeFuncWithNullValueNoDefault()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Maybe(_ => _.Replace("World", "John")));
        }

        [Test]
        public void TestMaybeFuncWithNullValueWithDefault()
        {
            const string C_INPUT = null;
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = C_DEFAULT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Maybe(_ => _.Replace("World", "John"), C_DEFAULT));
        }
    }
}