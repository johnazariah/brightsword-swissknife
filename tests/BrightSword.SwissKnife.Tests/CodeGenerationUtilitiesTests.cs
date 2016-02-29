using System;
using System.Collections.Generic;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class CodeGenerationUtilitiesTests
    {
        [Test]
        public void TestRenameCollectionClassWithInterfaceToConcreteType()
        {
            Assert.AreEqual("ListDTO<ConvertibleDTO>", typeof (List<IConvertible>).RenameToConcreteType(suffix: "DTO"));
        }

        [Test]
        public void TestRenameCollectionInterfaceWithInterfaceToConcreteType()
        {
            Assert.AreEqual("List<Convertible>", typeof (IList<IConvertible>).RenameToConcreteType());
        }

        [Test]
        public void TestRenameMalformedGenericInterfaceToConcreteType()
        {
            Assert.AreEqual(
                "MalformedGenericInterface<Int32>",
                typeof (MalformedGenericInterface<int>).RenameToConcreteType());
        }

        [Test]
        public void TestRenameMalformedInterfaceToConcreteType()
        {
            Assert.AreEqual("MalformedInterface", typeof (MalformedInterface).RenameToConcreteType());
        }

        [Test]
        public void TestRenameNonGenericDiscreteTypeToConcreteType()
        {
            Assert.AreEqual("Type", typeof (Type).RenameToConcreteType());
        }

        [Test]
        public void TestRenameNonGenericDiscreteTypeToConcreteTypeWithPrefix()
        {
            Assert.AreEqual("WType", typeof (Type).RenameToConcreteType("W"));
        }

        [Test]
        public void TestRenameNonGenericDiscreteTypeToConcreteTypeWithSuffix()
        {
            Assert.AreEqual("TypeDTO", typeof (Type).RenameToConcreteType(suffix: "DTO"));
        }

        [Test]
        public void TestRenameNonGenericInterfaceToConcreteType()
        {
            Assert.AreEqual("ConvertibleDTO", typeof (IConvertible).RenameToConcreteType(suffix: "DTO"));
        }

        [Test]
        public void TestRenamePrimitiveTypeToConcreteType()
        {
            Assert.AreEqual("Int32", typeof (int).RenameToConcreteType());
        }

        // ReSharper disable InconsistentNaming
        private interface MalformedGenericInterface<T>
        {
        }

        private interface MalformedInterface
        {
        }

        // ReSharper restore InconsistentNaming
    }
}