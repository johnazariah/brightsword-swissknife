using System;
using System.Collections.Generic;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class GetDefaultValueTests
    {
        private struct Foo
        {
            private decimal Amount { get; set; }
        }

        [Test]
        public void Test_GetDefaultValue()
        {
            Assert.AreEqual(default(int), typeof (int).GetDefaultValue());
            Assert.AreEqual(default(decimal), typeof (decimal).GetDefaultValue());
            Assert.AreEqual(default(Foo), typeof (Foo).GetDefaultValue());
            Assert.AreEqual(default(int), typeof (int).GetDefaultValue());
            Assert.AreEqual(default(Tuple<int, string>), typeof (Tuple<int, string>).GetDefaultValue());
            Assert.AreEqual(
                default(IDictionary<string, string>),
                typeof (IDictionary<string, string>).GetDefaultValue());
        }
    }
}