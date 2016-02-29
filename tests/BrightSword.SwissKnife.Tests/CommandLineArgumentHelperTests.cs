using System;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class CommandLineArgumentHelperTests
    {
        [Test]
        public void Test_GivenArgumentsWithoutMinusMinus_CommandLineArgumentHelper_MustNotCrash()
        {
            var instance = new CommandLineArgumentHelper("help", "name=foo", "weight=bar");
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.HelpRequested);
            Assert.AreEqual(3, instance.Count);
            Assert.AreEqual("foo", instance["name"]);
            Assert.AreEqual("foo", instance["Name"]);
            Assert.AreEqual("bar", instance["WEIGHT"]);
            Assert.AreEqual("bar_of_soap", instance.GetArgumentValue("weight", _ => $"{_}_of_soap", string.Empty));

            Assert.Throws<ArgumentOutOfRangeException>(() => { var result = instance["date"]; });

            Assert.AreEqual(string.Empty, instance.GetArgumentValue("date", _ => $"{_}_of_soap", string.Empty));
        }

        [Test]
        public void Test_GivenArgumentValuesQuotedWithEmbeddedSpaces_CommandLineArgumentHelper_MustNotCrash()
        {
            var instance = new CommandLineArgumentHelper("help", "name='foo is fun'", "weight=\"bar is not fun\"");
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.HelpRequested);
            Assert.AreEqual(3, instance.Count);
            Assert.AreEqual("'foo is fun'", instance["name"]);
            Assert.AreEqual("'foo is fun'", instance["Name"]);
            Assert.AreEqual("\"bar is not fun\"", instance["WEIGHT"]);
            Assert.AreEqual("\"bar is not fun\"", instance.GetArgumentValue("weight", _ => _, string.Empty));
        }

        [Test]
        public void Test_GivenNoArguments_CommandLineArgumentHelper_MustNotCrash()
        {
            var instance = new CommandLineArgumentHelper();
            Assert.IsNotNull(instance);
            Assert.IsFalse(instance.HelpRequested);
            Assert.AreEqual(0, instance.Count);
        }

        [Test]
        public void Test_GivenValidArguments_CommandLineArgumentHelper_MustNotCrash()
        {
            var instance = new CommandLineArgumentHelper("--help", "--name=foo", "--weight=bar");
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.HelpRequested);
            Assert.AreEqual(3, instance.Count);
            Assert.AreEqual("foo", instance["name"]);
            Assert.AreEqual("foo", instance["Name"]);
            Assert.AreEqual("bar", instance["WEIGHT"]);
            Assert.AreEqual("bar", instance.GetArgumentValue("weight", _ => _, string.Empty));
        }
    }
}