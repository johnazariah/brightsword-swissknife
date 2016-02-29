using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class UnlessMonadExtensionsTest
    {
        [Test]
        public void TestUnlessActionWithNonNullValueAndFalse()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => false, _ => { }));
        }

        [Test]
        public void TestUnlessActionWithNonNullValueAndTrue()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => true, _ => { }));
        }

        [Test]
        public void TestUnlessActionWithNullValueAndFalse()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => false, _ => { }));
        }

        [Test]
        public void TestUnlessActionWithNullValueAndTrue()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = default(string);

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => true, _ => { }));
        }

        [Test]
        public void TestUnlessFuncWithNonNullValueNoDefaultAndFalse()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = "Hello John";

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => false, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestUnlessFuncWithNonNullValueNoDefaultAndTrue()
        {
            const string C_INPUT = "Hello World";
            const string C_EXPECTED = default(string);

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => true, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestUnlessFuncWithNonNullValueWithDefaultAndFalse()
        {
            const string C_INPUT = "Hello World";
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = "Hello John";

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => false, _ => _.Replace("World", "John"), C_DEFAULT));
        }

        [Test]
        public void TestUnlessFuncWithNonNullValueWithDefaultAndTrue()
        {
            const string C_INPUT = "Hello World";
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = C_DEFAULT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => true, _ => _.Replace("World", "John"), C_DEFAULT));
        }

        [Test]
        public void TestUnlessFuncWithNullValueNoDefaultAndFalse()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = C_INPUT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => false, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestUnlessFuncWithNullValueNoDefaultAndTrue()
        {
            const string C_INPUT = null;
            const string C_EXPECTED = default(string);

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => true, _ => _.Replace("World", "John")));
        }

        [Test]
        public void TestUnlessFuncWithNullValueWithDefaultAndFalse()
        {
            const string C_INPUT = null;
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = C_DEFAULT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => false, _ => _.Replace("World", "John"), C_DEFAULT));
        }

        [Test]
        public void TestUnlessFuncWithNullValueWithDefaultAndTrue()
        {
            const string C_INPUT = null;
            const string C_DEFAULT = "Wayne's World";
            const string C_EXPECTED = C_DEFAULT;

            Assert.AreEqual(C_EXPECTED, C_INPUT.Unless(_ => true, _ => _.Replace("World", "John"), C_DEFAULT));
        }
    }
}