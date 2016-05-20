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
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name { get; set; }
            public int Age { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        private class PropertiesWithFlag
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [CommandLineArgument("a-flag", "a flag", IsFlag = true)]
            public bool AFlag { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        private class PropertiesWithDefaultValue
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [CommandLineArgument("bank-balance", "the bank balance", 100.0)]
            public decimal BankBalance { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
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
            {};

            var parsedArguments = input.ParseArguments<PropertiesWithDefaultValue>();
            Assert.IsTrue(parsedArguments.IsValidCommandLineParameterSet());

            Assert.AreEqual(100.0M, parsedArguments.BankBalance);
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

        [Test]
        public void Test_ShouldProduceTheRightUsageString_NoAttribute()
        {
            var input = new string[]
            {};

            var parsedArguments = input.ParseArguments<SimpleProperties>();
            Console.Write(parsedArguments.Usage());
            const string expected = @"
***
*** Usage:
***
***	 --Name=<value>     : (Mandatory) : 
***	 --Age=<value>      : (Mandatory) : 
***
*** Enumerations:
***
***
***
*** Defaults:
***
***		 --Name has a default value of []
***		 --Age has a default value of []
***
*** Effective Values:
***
***		 The effective value of --Name is []
***		 The effective value of --Age is [0]
***";
            var actual = parsedArguments.Usage();

            Assert.AreEqual(
                expected.Replace("\r", string.Empty)
                    .Replace("\n", string.Empty),
                actual.Replace("\r", string.Empty)
                    .Replace("\n", string.Empty));
        }

        [Test]
        public void Test_ShouldProduceTheRightUsageString_Optional()
        {
            var input = new string[]
            {};

            var parsedArguments = input.ParseArguments<PropertiesWithDefaultValue>();
            Console.Write(parsedArguments.Usage());
            const string expected = @"
***
*** Usage:
***
***	 [--bank-balance=<value>] : (Optional) : the bank balance
***
*** Enumerations:
***
***
***
*** Defaults:
***
***		 --bank-balance has a default value of [100]
***
*** Effective Values:
***
***		 The effective value of --bank-balance is [100]
***";
            var actual = parsedArguments.Usage();

            Assert.AreEqual(
                expected.Replace("\r", string.Empty)
                    .Replace("\n", string.Empty),
                actual.Replace("\r", string.Empty)
                    .Replace("\n", string.Empty));
        }
    }
}