using System.Collections.Generic;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class TypeGetNameExtensionTests
    {
        [Test]
        public void Given_GenericTypeMultipleArguments_GetName()
        {
            Assert.AreEqual(
                "IDictionary<Int32, IList<String>>",
                typeof (IDictionary<int, IList<string>>).PrintableName());
        }

        [Test]
        public void Given_GenericTypeOneArgument_GetName()
        {
            Assert.AreEqual("IList<Int32>", typeof (IList<int>).PrintableName());
        }

        [Test]
        public void Given_SimpleType_GetName()
        {
            Assert.AreEqual("Int32", typeof (int).PrintableName());
        }
    }
}