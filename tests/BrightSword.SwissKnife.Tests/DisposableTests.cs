using System;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class DisposableTests
    {
        [Test]
        public void TestDisposeActionIsFiredIfProvided_InputCreateFunction()
        {
            const string message = "hello";
            var result = string.Empty;

            using (var foo = new Disposable<string>(() => message, _ => { result = message; })) {
                Assert.AreEqual(message, foo.Instance);
            }

            Assert.AreEqual(result, message);
        }

        [Test]
        public void TestDisposeActionIsFiredIfProvided_InputValue()
        {
            const string message = "hello";
            try
            {
                using (var foo = new Disposable<string>(message, _ => { throw new Exception(message); })) {
                    Assert.AreEqual(message, foo.Instance);
                }

                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual(message, ex.Message);
            }
        }
    }
}