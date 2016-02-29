using System;
using System.Collections.Generic;
using System.Linq;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class EnumerableExtensionTests
    {
        internal class Capsule : IEquatable<Capsule>
        {
            public Capsule(string tag)
            {
                Tag = tag;
            }

            // note - this is immutable
            public string Tag { get; }

            public bool Equals(Capsule other)
            {
                return string.Equals(Tag, other.Tag);
            }

            public override int GetHashCode()
            {
                return Tag?.GetHashCode() ?? 0;
            }

            public override bool Equals(object obj)
            {
                var other = obj as Capsule;
                return other != null && Tag == other.Tag;
            }
        }

        internal struct CapsuleStruct
        {
            public CapsuleStruct(string tag) : this()
            {
                Tag = tag;
            }

            public string Tag { get; set; }
        }

        [Test]
        public void Given_ASequence_Then_BatchingTheSequence_ShouldWork()
        {
            var sequence = Enumerable.Range(0, 100)
                                     .ToList();

            Assert.AreEqual(100, sequence.Count);

            var batches = sequence.Batch(10)
                                  .ToList();

            Assert.AreEqual(10, batches.Count);

            Assert.IsTrue(
                batches.First()
                       .SequenceEqual(Enumerable.Range(0, 10)));
        }

        [Test]
        public void Given_EmptyEnumeration_LastButOne_ReturnsNull()
        {
            var input = Enumerable.Empty<string>();

            var actual = input.LastButOne();

            Assert.IsNull(actual);
        }

        [Test]
        public void Given_EnumerationWithMany_LastButOne_ReturnsCorrectly()
        {
            var input = new[]
                        {
                            "hello",
                            "this",
                            "is",
                            "a",
                            "happy",
                            "world"
                        };

            var expected = "happy";
            var actual = input.LastButOne();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_EnumerationWithOneItem_LastButOne_ReturnsNull()
        {
            var input = new[]
                        {
                            "hello"
                        };

            var actual = input.LastButOne();

            Assert.IsNull(actual);
        }

        [Test]
        public void Given_EnumerationWithTwoItem_LastButOne_ReturnsCorrectly()
        {
            var input = new[]
                        {
                            "hello",
                            "world"
                        };

            var expected = "hello";
            var actual = input.LastButOne();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_EqualSequences_Then_SequenceEqual_ShouldBe_True()
        {
            var left = new[]
                       {
                           new Capsule("Aardvark"),
                           new Capsule("Bat"),
                           new Capsule("Crab"),
                           new Capsule("Donkey")
                       };

            var right = new[]
                        {
                            new Capsule("Aardvark"),
                            new Capsule("Crab"),
                            new Capsule("Bat"),
                            new Capsule("Donkey")
                        };

            Assert.IsFalse(right.SequenceEqual(left));
            Assert.IsTrue(
                right.OrderBy(_ => _.Tag)
                     .SequenceEqual(left));
        }

        [Test]
        public void Given_NonEqualSequences_Then_SequenceEqual_ShouldBe_True()
        {
            var left = new[]
                       {
                           new Capsule("Aardvark"),
                           new Capsule("Bat"),
                           new Capsule("Crab"),
                           new Capsule("Donkey")
                       };

            var right = new[]
                        {
                            new Capsule("Aardvark"),
                            new Capsule("Crab"),
                            new Capsule("Bat"),
                            new Capsule("Donkey")
                        };

            Assert.IsFalse(right.SequenceEqual(left));
        }

        [Test]
        public void Given_NonUniqueEnumerationOfClasses_Then_AllUnique_ShouldBe_False()
        {
            var input = new[]
                        {
                            new Capsule("Hello"),
                            new Capsule("Hello")
                        };

            Assert.IsFalse(input.AllUnique());
        }

        [Test]
        public void Given_NonUniqueEnumerationOfStructs_Then_AllUnique_ShouldBe_False()
        {
            var input = new[]
                        {
                            new CapsuleStruct("Hello"),
                            new CapsuleStruct("Hello")
                        };

            Assert.IsFalse(input.AllUnique());
        }

        [Test]
        public void Given_NonUniqueEnumerationOfValues_Then_AllUnique_ShouldBe_False()
        {
            var input = new[]
                        {
                            3,
                            1,
                            4,
                            1,
                            5,
                            9
                        };

            Assert.IsFalse(input.AllUnique());
        }

        [Test]
        public void Given_Null_LastButOne_ReturnsNull()
        {
            IEnumerable<string> input = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var actual = input.LastButOne();

            Assert.IsNull(actual);
        }

        [Test]
        public void Given_SortedListOfNonUniqueValues_Then_SortedListIsUnique_ShouldBe_False()
        {
            var input = new[]
                        {
                            1,
                            2,
                            3,
                            4,
                            4,
                            5,
                            9
                        };

            Assert.IsFalse(input.SortedListIsUnique());
        }

        [Test]
        public void Given_SortedListOfUniqueValues_Then_SortedListIsUnique_ShouldBe_True()
        {
            var input = new[]
                        {
                            1,
                            2,
                            3,
                            5,
                            9
                        };

            Assert.IsTrue(input.SortedListIsUnique());
        }

        [Test]
        public void Given_UniqueEnumerationOfClasses_Then_AllUnique_ShouldBe_True()
        {
            var input = new[]
                        {
                            new Capsule("Hello"),
                            new Capsule("World")
                        };

            Assert.IsTrue(input.AllUnique());
        }

        [Test]
        public void Given_UniqueEnumerationOfStructs_Then_AllUnique_ShouldBe_True()
        {
            var input = new[]
                        {
                            new CapsuleStruct("Hello"),
                            new CapsuleStruct("World")
                        };

            Assert.IsTrue(input.AllUnique());
        }

        [Test]
        public void Given_UniqueEnumerationOfValues_Then_AllUnique_ShouldBe_True()
        {
            var input = new[]
                        {
                            3,
                            1,
                            4,
                            2
                        };

            Assert.IsTrue(input.AllUnique());
        }
    }
}