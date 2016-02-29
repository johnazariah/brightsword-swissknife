using System;
using System.Linq;
using System.Reflection;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class GetAllMethodsTests
    {
        private static void Check_GetAllMethods_Count<T>(int expectedCount, Func<MethodInfo, bool> filter = null)
        {
            filter = filter ?? (_ => true);

            Assert.AreEqual(
                expectedCount,
                typeof (T).GetAllMethods()
                          .Where(filter)
                          .Count());

            Assert.AreEqual(
                expectedCount,
                TypeMemberDiscoverer<T>.GetAllMethods()
                                       .Where(filter)
                                       .Count());
        }

        [Test]
        public void Given_ClassWithBase_GetPublicMethods()
        {
            Check_GetAllMethods_Count<ClassWithBase>(8);
        }

        [Test]
        public void Given_ClassWithBaseAndInterface_GetPublicMethods()
        {
            Check_GetAllMethods_Count<ClassWithBaseAndInterfaces>(10);
        }

        [Test]
        public void Given_ClassWithBaseWithInterface_GetPublicMethods()
        {
            Check_GetAllMethods_Count<ClassWithBaseWithInterfaces>(6);
        }

        [Test]
        public void Given_ClassWithoutBase_GetPublicMethods()
        {
            Check_GetAllMethods_Count<ClassWithoutBase>(7);
        }

        [Test]
        public void Given_ClassWithOverride_GetPublicMethods()
        {
            Check_GetAllMethods_Count<ClassWithOverridenMethod>(5);
        }

        [Test]
        public void Given_InterfaceWithBase_GetPublicMethods()
        {
            Check_GetAllMethods_Count<IInterfaceWithBase>(4);
        }

        [Test]
        public void Given_InterfaceWithManyBases_GetPublicMethods()
        {
            Check_GetAllMethods_Count<IInterfaceWithManyBases>(6);
        }

        [Test]
        public void Given_InterfaceWithoutBase_GetPublicMethods()
        {
            Check_GetAllMethods_Count<IInterfaceWithoutBase>(3);
        }

        [Test]
        public void Given_StructWithMethods_GetPublicMethods()
        {
            Check_GetAllMethods_Count<StructWithMethods>(6);
        }

// ReSharper disable UnusedMember.Local

        private abstract class BaseClassWithVirtualMethod
        {
            public virtual decimal ComputeV()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class ClassWithBase : ClassWithoutBase
        {
            public virtual string ComputeD()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class ClassWithBaseAndInterfaces
            : ClassWithBase,
              IAnotherBase,
              IYetAnotherBase
        {
            public virtual int ComputeE()
            {
                throw new NotImplementedException();
            }

            public virtual int ComputeG()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class ClassWithBaseWithInterfaces
            : ClassWithInterface,
              IYetAnotherBase
        {
            public virtual int ComputeG()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class ClassWithInterface : IAnotherBase
        {
            public virtual int ComputeE()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class ClassWithOverridenMethod : BaseClassWithVirtualMethod
        {
            public override decimal ComputeV()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class ClassWithoutBase
        {
            public string ComputeA()
            {
                throw new NotImplementedException();
            }

            public string ComputeB()
            {
                throw new NotImplementedException();
            }

            public string ComputeC()
            {
                throw new NotImplementedException();
            }
        }

        private interface IAnotherBase
        {
            int ComputeE();
        }

        private interface IInterfaceWithBase : IInterfaceWithoutBase
        {
            string ComputeD();
        }

        private interface IInterfaceWithManyBases
            : IInterfaceWithBase,
              IAnotherBase
        {
            double ComputeF();
        }

        private interface IInterfaceWithoutBase
        {
            string ComputeA();
            string ComputeB();
            string ComputeC();
        }

        private interface IReadonlyMethod
        {
            int ComputeG();
        }

        private interface IYetAnotherBase
        {
            int ComputeG();
        }

        private struct StructWithMethods
        {
            public decimal ComputeV()
            {
                throw new NotImplementedException();
            }

            public double ComputeW()
            {
                throw new NotImplementedException();
            }
        }

// ReSharper restore UnusedMember.Local
    }
}