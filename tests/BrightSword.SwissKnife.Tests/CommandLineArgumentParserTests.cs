using System;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class CommandLineArgumentParserTests
    {
        private class SimpleProperties
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private class PropertiesWithFlag
        {
            [CommandLineArgument("a-flag", "a flag", IsFlag = true)]
            public bool AFlag { get; set; }
        }

        private class PropertiesWithDefaultValue
        {
            [CommandLineArgument("bank-balance", "the bank balance", 100.0)]
            public decimal BankBalance { get; set; }
        }

        [Test]
        public void Test_ShouldParseArgumentsIntoProperties_NoAttribute()
        {
            var input = new[]
                        {
                            "--name=John",
                            "--age=99"
                        };

            var parsedArguments = input.ParseArguments<SimpleProperties>();
            Assert.IsTrue(parsedArguments.IsValidCommandLineParameterSet());

            Assert.AreEqual("John", parsedArguments.Name);
            Assert.AreEqual(99, parsedArguments.Age);
        }

        [Test]
        public void Test_ShouldParseArgumentWithDefaultValueIntoProperties()
        {
            var input = new string[]
                        {
                        };

            var parsedArguments = input.ParseArguments<PropertiesWithDefaultValue>();
            Assert.IsTrue(parsedArguments.IsValidCommandLineParameterSet());

            Assert.AreEqual(100.0M, parsedArguments.BankBalance);

            Console.Write(parsedArguments.Usage());
//            Assert.AreEqual(@"*** [Usage]
//***	 [--bank-balance=<value>] : the bank balance
//***
//***
//*** [Defaults]
//***		 --bank-balance has a default value of [100]
//***
//*** [Effective Values]
//***		 The effective value of --bank-balance is [100]", parsedArguments.Usage());
        }

        [Test]
        public void Test_ShouldParseFlagArgumentsIntoProperties()
        {
            var input = new[]
                        {
                            "--a-flag",
                            "--age=99"
                        };

            var parsedArguments = input.ParseArguments<PropertiesWithFlag>();
            Assert.IsTrue(parsedArguments.IsValidCommandLineParameterSet());

            Assert.AreEqual(true, parsedArguments.AFlag);
        }
    }
}