using System;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class CoerceExtensionTests
    {
        private enum Colors
        {
            Red,

            Green,

            Blue
        }

        private enum ErsatzBoolean
        {
            True,

            False
        };

        [Test]
        public void Test_Coerce_ObjectToBool()
        {
            Assert.AreEqual(true, true.CoerceType(typeof (bool), default(bool)));
            Assert.AreEqual(false, false.CoerceType(typeof (bool), default(bool)));
            Assert.AreEqual(true, ErsatzBoolean.True.CoerceType(typeof (bool), default(bool)));
            Assert.AreEqual(false, ErsatzBoolean.False.CoerceType(typeof (bool), default(bool)));
        }

        [Test]
        public void Test_Coerce_ObjectToByte()
        {
            const byte expected = 0xA;

            Assert.AreEqual(expected, expected.CoerceType(typeof (byte), default(byte)));
            Assert.AreEqual(expected, 10.CoerceType(typeof (byte), default(byte)));
            Assert.AreEqual(expected, "10".CoerceType(typeof (byte), default(byte)));
            Assert.AreEqual(default(byte), "Hello".CoerceType(typeof (byte), default(byte)));
        }

        [Test]
        public void Test_Coerce_ObjectToChar()
        {
            const char expected = 'x';

            Assert.AreEqual(expected, expected.CoerceType(typeof (char), default(char)));
            Assert.AreEqual(expected, 'x'.CoerceType(typeof (char), default(char)));
            Assert.AreEqual(expected, "x".CoerceType(typeof (char), default(char)));
            Assert.AreEqual(default(char), "Hello".CoerceType(typeof (char), default(char)));
        }

        [Test]
        public void Test_Coerce_ObjectToDateTime()
        {
            var now = DateTime.Now;

            var expected = now;

            Assert.AreEqual(expected, expected.CoerceType(typeof (DateTime), default(DateTime)));

            Assert.AreEqual(
                expected,
                now.ToString("o")
                   .CoerceType(typeof (DateTime), default(DateTime)));

            Assert.AreEqual(default(DateTime), "Hello".CoerceType(typeof (DateTime), default(DateTime)));
        }

        [Test]
        public void Test_Coerce_ObjectToDecimal()
        {
            const decimal expected = 3.14M;

            Assert.AreEqual(expected, expected.CoerceType(typeof (decimal), default(decimal)));
            Assert.AreEqual(expected, 3.14F.CoerceType(typeof (decimal), default(decimal)));
            Assert.AreEqual(expected, "3.14".CoerceType(typeof (decimal), default(decimal)));
            Assert.AreEqual(default(decimal), "Hello".CoerceType(typeof (decimal), default(decimal)));
        }

        [Test]
        public void Test_Coerce_ObjectToDouble()
        {
            const double expected = 3.14D;

            Assert.AreEqual(expected, expected.CoerceType(typeof (double), default(double)));
            Assert.AreEqual(expected, 3.14M.CoerceType(typeof (double), default(double)));
            Assert.AreEqual(expected*1000, "3.14E3".CoerceType(typeof (double), default(double)));
            Assert.AreEqual(default(double), "Hello".CoerceType(typeof (double), default(double)));
        }

        [Test]
        public void Test_Coerce_ObjectToEnum()
        {
            Assert.AreEqual(Colors.Blue, (Colors) "Blue".CoerceType(typeof (Colors), default(Colors)));
            Assert.AreEqual(Colors.Blue, (Colors) "2".CoerceType(typeof (Colors), default(Colors)));
            Assert.AreEqual(Colors.Blue, (Colors) 2.CoerceType(typeof (Colors), default(Colors)));
            Assert.AreEqual(Colors.Blue, (Colors) Colors.Blue.CoerceType(typeof (Colors), default(Colors)));
            Assert.AreEqual(default(Colors), "Hungry".CoerceType(typeof (Colors), default(Colors)));

            Assert.AreEqual(default(Colors), "Hungry".CoerceType(typeof (Colors), null));

            Assert.AreEqual(Colors.Green, "Hungry".CoerceType(typeof (Colors), Colors.Green));

            //Assert.Throws<ArgumentException>(() =>
            //                                                   {
            //                                                       var result = "Hungry".CoerceType(typeof (Colors),
            //                                                                                        null);
            //                                                   });
        }

        [Test]
        public void Test_Coerce_ObjectToFloat()
        {
            const float expected = 3.14F;

            Assert.AreEqual(expected, expected.CoerceType(typeof (float), default(float)));
            Assert.AreEqual(expected, 3.14M.CoerceType(typeof (float), default(float)));
            Assert.AreEqual(expected, "3.14".CoerceType(typeof (float), default(float)));
            Assert.AreEqual(default(float), "Hello".CoerceType(typeof (float), default(float)));
        }

        [Test]
        public void Test_Coerce_ObjectToInt()
        {
            const int expected = 65537;

            Assert.AreEqual(expected, expected.CoerceType(typeof (int), default(int)));
            Assert.AreEqual(expected, 65537L.CoerceType(typeof (int), default(int)));
            Assert.AreEqual(expected, "65537".CoerceType(typeof (int), default(int)));
            Assert.AreEqual(default(int), "Hello".CoerceType(typeof (int), default(int)));
        }

        [Test]
        public void Test_Coerce_ObjectToLong()
        {
            const long expected = 65537L;

            Assert.AreEqual(expected, expected.CoerceType(typeof (long), default(long)));
            Assert.AreEqual(expected, 65537.CoerceType(typeof (long), default(long)));

            Assert.AreEqual(expected, "65537".CoerceType(typeof (long), default(long)));
            Assert.AreEqual(default(long), "Hello".CoerceType(typeof (long), default(long)));
        }

        [Test]
        public void Test_Coerce_ObjectToShort()
        {
            const short expected = 0x10;

            Assert.AreEqual(expected, expected.CoerceType(typeof (short), default(short)));
            Assert.AreEqual(expected, 16.CoerceType(typeof (short), default(short)));
            Assert.AreEqual(expected, "16".CoerceType(typeof (short), default(short)));
            Assert.AreEqual(default(short), "Hello".CoerceType(typeof (short), default(short)));
        }

        [Test]
        public void Test_Coerce_ObjectToType()
        {
            var expected = typeof (int);

            Assert.AreEqual(expected, expected.CoerceType(typeof (Type), Type.EmptyTypes));
            Assert.AreEqual(null, CoerceExtensions.CoerceType(null, typeof (Type), typeof (string)));
        }

        [Test]
        public void Test_Coerce_StringToBool()
        {
            Assert.IsTrue((bool) "y".CoerceType(typeof (bool), default(bool)));
            Assert.IsTrue((bool) "True".CoerceType(typeof (bool), default(bool)));
            Assert.IsFalse((bool) "False".CoerceType(typeof (bool), default(bool)));
            Assert.IsFalse((bool) "False".CoerceType(typeof (bool), default(bool)));

            Assert.IsFalse((bool) string.Empty.CoerceType(typeof (bool), default(bool)));

            Assert.Throws<ArgumentException>(
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var result = "Hungry".CoerceType(typeof (bool), default(bool));
                });
        }

        [Test]
        public void Test_Coerce_StringToString()
        {
            Assert.AreEqual("Hello World", "Hello World".CoerceType(typeof (string), string.Empty));
        }
    }
}