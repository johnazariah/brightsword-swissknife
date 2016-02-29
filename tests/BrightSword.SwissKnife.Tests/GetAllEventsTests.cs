using System;
using System.Linq;
using System.Reflection;
using BrightSword.SwissKnife;
using NUnit.Framework;

// ReSharper disable EventNeverSubscribedTo.Local

#pragma warning disable 67,108,114

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class GetAllEventsTests
    {
        private static void Check_GetAllEvents_Count<T>(int expectedCount, Func<EventInfo, bool> filter = null)
        {
            filter = filter ?? (_ => true);

            Assert.AreEqual(
                expectedCount,
                typeof (T).GetAllEvents()
                          .Where(filter)
                          .Count());

            Assert.AreEqual(
                expectedCount,
                TypeMemberDiscoverer<T>.GetAllEvents()
                                       .Where(filter)
                                       .Count());
        }

        private abstract class BaseClassWithVirtualEvent
        {
            public event EventHandler EventV;
        }

        private struct StructWithEvent
        {
            public event EventHandler EventV;
        }

        private abstract class ClassWithBase : ClassWithoutBase
        {
            public event EventHandler EventD;
        }

        private abstract class ClassWithBaseAndInterfaces
            : ClassWithBase,
              IAnotherBase,
              IYetAnotherBase
        {
            public event EventHandler EventE;
            public event EventHandler EventG;
        }

        private abstract class ClassWithBaseWithInterfaces
            : ClassWithInterface,
              IYetAnotherBase
        {
            public event EventHandler EventG;
        }

        private abstract class ClassWithInterface : IAnotherBase
        {
            public event EventHandler EventE;
        }

        private abstract class ClassWithOverridenEvent : BaseClassWithVirtualEvent
        {
            public event EventHandler EventV;
        }

        private abstract class ClassWithoutBase
        {
            public event EventHandler EventA;
            public event EventHandler EventB;
            public event EventHandler EventC;
        }

        private interface IAnotherBase
        {
            event EventHandler EventE;
        }

        private interface IInterfaceWithBase : IInterfaceWithoutBase
        {
            event EventHandler EventD;
        }

        private interface IInterfaceWithManyBases
            : IInterfaceWithBase,
              IAnotherBase
        {
            event EventHandler EventF;
        }

        private interface IInterfaceWithoutBase
        {
            event EventHandler EventA;
            event EventHandler EventB;
            event EventHandler EventC;
        }

        private interface IYetAnotherBase
        {
            event EventHandler EventG;
        }

        [Test]
        public void Given_ClassWithBase_GetPublicEvents()
        {
            Check_GetAllEvents_Count<ClassWithBase>(4);
        }

        [Test]
        public void Given_ClassWithBaseAndInterface_GetPublicEvents()
        {
            Check_GetAllEvents_Count<ClassWithBaseAndInterfaces>(6);
        }

        [Test]
        public void Given_ClassWithBaseWithInterface_GetPublicEvents()
        {
            Check_GetAllEvents_Count<ClassWithBaseWithInterfaces>(2);
        }

        [Test]
        public void Given_ClassWithoutBase_GetPublicEvents()
        {
            Check_GetAllEvents_Count<ClassWithoutBase>(3);
        }

        [Test]
        public void Given_ClassWithOverride_GetPublicEvents()
        {
            Check_GetAllEvents_Count<ClassWithOverridenEvent>(1);
        }

        [Test]
        public void Given_InterfaceWithBase_GetPublicEvents()
        {
            Check_GetAllEvents_Count<IInterfaceWithBase>(4);
        }

        [Test]
        public void Given_InterfaceWithManyBases_GetPublicEvents()
        {
            Check_GetAllEvents_Count<IInterfaceWithManyBases>(6);
        }

        [Test]
        public void Given_InterfaceWithoutBase_GetPublicEvents()
        {
            Check_GetAllEvents_Count<IInterfaceWithoutBase>(3);
        }

        [Test]
        public void Given_StructWithEvents_GetPublicEvents()
        {
            Check_GetAllEvents_Count<StructWithEvent>(1);
        }
    }
}