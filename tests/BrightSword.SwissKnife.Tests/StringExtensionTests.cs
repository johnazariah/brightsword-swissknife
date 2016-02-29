using System.Linq;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        public void Given_AllCapsWord_SplitCamelCase()
        {
            var input = "HELLO";

            var expected = (new[]
                            {
                                "HELLO"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_CamelCaseAndUnderscore_SplitCamelCaseAndUnderscore()
        {
            var input = "Hello_HappyWorld";

            var expected = (new[]
                            {
                                "Hello",
                                "Happy",
                                "World"
                            });

            var actual = input.SplitCamelCaseAndUnderscore()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_CamelCasedMultipleSegment_SplitCamelCase()
        {
            var input = "helloWorld";

            var expected = (new[]
                            {
                                "hello",
                                "World"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_EmptyString_SplitCamelCase()
        {
            var input = string.Empty;

            var expected = Enumerable.Empty<string>()
                                     .ToArray();

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_EmptyString_SplitDotted()
        {
            var input = string.Empty;

            var expected = Enumerable.Empty<string>()
                                     .ToArray();

            var actual = input.SplitDotted()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_MixedSeparatedSegments_SplitCamelCase()
        {
            var input = "HELLO worldOut There";

            var expected = (new[]
                            {
                                "HELLO",
                                "world",
                                "Out",
                                "There"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_MultipleDot_SplitDotted()
        {
            var input = "...";

            var expected = Enumerable.Empty<string>()
                                     .ToArray();

            var actual = input.SplitDotted()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_MultipleSegmentDotted_SplitDotted()
        {
            var input = "Hello.World";

            var expected = (new[]
                            {
                                "Hello",
                                "World"
                            });

            var actual = input.SplitDotted()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_MultipleSegmentUnderscore_SplitDotted()
        {
            var input = "Hello_World";

            var expected = (new[]
                            {
                                "Hello_World"
                            });

            var actual = input.SplitDotted()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_OneLowerCasedCharacter_SplitCamelCase()
        {
            var input = "a";

            var expected = (new[]
                            {
                                "a"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_OneUpperCasedCharacter_SplitCamelCase()
        {
            var input = "A";

            var expected = (new[]
                            {
                                "A"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_PascalCasedMultipleSegment_SplitCamelCase()
        {
            var input = "HelloWorld";

            var expected = (new[]
                            {
                                "Hello",
                                "World"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_SingleDot_SplitDotted()
        {
            var input = ".";

            var expected = Enumerable.Empty<string>()
                                     .ToArray();

            var actual = input.SplitDotted()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_SingleLowerCaseSegment_SplitCamelCase()
        {
            var input = "hello";

            var expected = (new[]
                            {
                                "hello"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_SingleSegmentNoDots_SplitDotted()
        {
            var input = "hello";

            var expected = (new[]
                            {
                                "hello"
                            });

            var actual = input.SplitDotted()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_SpaceSeparatedSegments_SplitCamelCase()
        {
            var input = "HELLO world   Out   There";

            var expected = (new[]
                            {
                                "HELLO",
                                "world",
                                "Out",
                                "There"
                            });

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_StringWithAcronym_SplitCamelCase()
        {
            var input = "AMACharter";

            var expected = new[]
                           {
                               "AMA",
                               "Charter"
                           };

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_WhitespaceString_SplitCamelCase()
        {
            var input = "  ";

            var expected = Enumerable.Empty<string>()
                                     .ToArray();

            var actual = input.SplitCamelCase()
                              .ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}