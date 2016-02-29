using System;
using System.Linq;
using System.Reflection;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class GetAllPropertiesTests
    {
        private static void Check_GetAllProperties_Count<T>(int expectedCount, Func<PropertyInfo, bool> filter = null)
        {
            filter = filter ?? (_ => true);

            Assert.AreEqual(
                expectedCount,
                typeof (T).GetAllProperties()
                          .Where(filter)
                          .Count());

            Assert.AreEqual(
                expectedCount,
                TypeMemberDiscoverer<T>.GetAllProperties()
                                       .Where(filter)
                                       .Count());
        }

        private abstract class BaseClassWithVirtualProperty
        {
            public virtual decimal V { get; set; }
        }

        private abstract class ClassWithBase : ClassWithoutBase
        {
            public string D { get; set; }
        }

        private abstract class ClassWithBaseAndInterfaces
            : ClassWithBase,
              IAnotherBase,
              IYetAnotherBase
        {
            public int E { get; set; }
            public int G { get; set; }
        }

        private abstract class ClassWithBaseWithInterfaces
            : ClassWithInterface,
              IYetAnotherBase
        {
            public int G { get; set; }
        }

        private abstract class ClassWithInterface : IAnotherBase
        {
            public int E { get; set; }
        }

        private abstract class ClassWithOverridenProperty : BaseClassWithVirtualProperty
        {
            public override decimal V { get; set; }
        }

        private abstract class ClassWithoutBase
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
        }

        private interface IAnotherBase
        {
            int E { get; set; }
        }

        private interface IInterfaceWithBase : IInterfaceWithoutBase
        {
            string D { get; set; }
        }

        private interface IInterfaceWithManyBases
            : IInterfaceWithBase,
              IAnotherBase
        {
            double F { get; set; }
        }

        private interface IInterfaceWithoutBase
        {
            string A { get; set; }
            string B { get; set; }
            string C { get; set; }
        }

        private interface IReadonlyProperty
        {
            int G { get; }
        }

        private interface IYetAnotherBase
        {
            int G { get; set; }
        }

        private abstract class ReadonlyPropertyClass
        {
            public int AutoProperty { get; private set; }

            public string ReadonlyProperty
            {
                get { throw new NotSupportedException(); }
            }
        }

        private struct StructWithProperties
        {
            public decimal V { get; set; }
            public double W { get; set; }
        }

        [Test]
        public void Given_ClassWithBase_GetPublicProperties()
        {
            Check_GetAllProperties_Count<ClassWithBase>(4);
        }

        [Test]
        public void Given_ClassWithBaseAndInterface_GetPublicProperties()
        {
            Check_GetAllProperties_Count<ClassWithBaseAndInterfaces>(6);
        }

        [Test]
        public void Given_ClassWithBaseWithInterface_GetPublicProperties()
        {
            Check_GetAllProperties_Count<ClassWithBaseWithInterfaces>(2);
        }

        [Test]
        public void Given_ClassWithoutBase_GetPublicProperties()
        {
            Check_GetAllProperties_Count<ClassWithoutBase>(3);
        }

        [Test]
        public void Given_ClassWithOverride_GetPublicProperties()
        {
            Check_GetAllProperties_Count<ClassWithOverridenProperty>(1);
        }

        [Test]
        public void Given_ClassWithReadonlyProperty_GetReadonlyProperties()
        {
            Check_GetAllProperties_Count<ReadonlyPropertyClass>(1, _ => !_.CanWrite);
            Check_GetAllProperties_Count<ReadonlyPropertyClass>(0, _ => _.GetSetMethod() != null);
        }

        [Test]
        public void Given_IInterfaceWithManyBases_GetPropertyByName()
        {
            Assert.IsNotNull(
                typeof (IInterfaceWithManyBases).GetProperty(
                    ObjectDescriber.GetName((IInterfaceWithManyBases _) => _.A),
                    true));
            Assert.IsNotNull(
                typeof (IInterfaceWithManyBases).GetProperty(
                    ObjectDescriber.GetName((IInterfaceWithManyBases _) => _.B),
                    true));
            Assert.IsNotNull(
                typeof (IInterfaceWithManyBases).GetProperty(
                    ObjectDescriber.GetName((IInterfaceWithManyBases _) => _.C),
                    true));
            Assert.IsNotNull(
                typeof (IInterfaceWithManyBases).GetProperty(
                    ObjectDescriber.GetName((IInterfaceWithManyBases _) => _.D),
                    true));
            Assert.IsNotNull(
                typeof (IInterfaceWithManyBases).GetProperty(
                    ObjectDescriber.GetName((IInterfaceWithManyBases _) => _.E),
                    true));
            Assert.IsNotNull(
                typeof (IInterfaceWithManyBases).GetProperty(
                    ObjectDescriber.GetName((IInterfaceWithManyBases _) => _.F),
                    false));
        }

        [Test]
        public void Given_InterfaceWithBase_GetPublicProperties()
        {
            Check_GetAllProperties_Count<IInterfaceWithBase>(4);
        }

        [Test]
        public void Given_InterfaceWithManyBases_GetPublicProperties()
        {
            Check_GetAllProperties_Count<IInterfaceWithManyBases>(6);
        }

        [Test]
        public void Given_InterfaceWithoutBase_GetPublicProperties()
        {
            Check_GetAllProperties_Count<IInterfaceWithoutBase>(3);
        }

        [Test]
        public void Given_InterfaceWithReadonlyProperty_GetReadonlyProperties()
        {
            Check_GetAllProperties_Count<IReadonlyProperty>(1, _ => !_.CanWrite);
            Check_GetAllProperties_Count<IReadonlyProperty>(0, _ => _.GetSetMethod() != null);
        }

        [Test]
        public void Given_Struct_GetPublicProperties()
        {
            Check_GetAllProperties_Count<StructWithProperties>(2);
        }
    }
}