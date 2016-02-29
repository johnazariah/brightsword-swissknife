using System;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class ObjectDescriberTests
    {
        private abstract class Test
        {
            public string SomeProperty { get; private set; }
            public static DateTime SomeStaticProperty => DateTime.Now;
            public abstract void SomeAction();

            public int SomeFunction()
            {
                // bogus code to force SomeFunction to not be made static
                return int.Parse(SomeProperty);
            }

            public static void SomeStaticAction()
            {
            }

            public static int SomeStaticFunction()
            {
                return 42;
            }
        }

        [Test]
        public void TestGetNameForAction()
        {
            Assert.AreEqual("SomeAction", ObjectDescriber.GetName((Test _) => _.SomeAction()));
        }

        [Test]
        public void TestGetNameForComplexExpressionFailsProperly()
        {
            Assert.Throws<NotSupportedException>(
                () => ObjectDescriber.GetName(() => Test.SomeStaticProperty + "Hello"));
        }

        [Test]
        public void TestGetNameForFunction()
        {
            Assert.AreEqual("SomeFunction", ObjectDescriber.GetName((Test _) => _.SomeFunction()));
        }

        [Test]
        public void TestGetNameForProperty()
        {
            Assert.AreEqual("SomeProperty", ObjectDescriber.GetName((Test _) => _.SomeProperty));
        }

        [Test]
        public void TestGetNameForStaticAction()
        {
            Assert.AreEqual("SomeStaticAction", ObjectDescriber.GetName(() => Test.SomeStaticAction()));
        }

        [Test]
        public void TestGetNameForStaticFunction()
        {
            Assert.AreEqual("SomeStaticFunction", ObjectDescriber.GetName(() => Test.SomeStaticFunction()));
        }

        [Test]
        public void TestGetNameForStaticProperty()
        {
            Assert.AreEqual("SomeStaticProperty", ObjectDescriber.GetName(() => Test.SomeStaticProperty));
        }
    }
}