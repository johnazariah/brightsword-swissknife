using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    public static class TypeMemberDiscoverer
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public;

        public static IEnumerable<PropertyInfo> GetAllProperties(
            this Type _this,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            return _this.GetAllMembers((_type, _bindingFlags) => _type.GetProperties(_bindingFlags), bindingFlags);
        }

        public static IEnumerable<MethodInfo> GetAllMethods(
            this Type _this,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            return _this.GetAllMembers((_type, _bindingFlags) => _type.GetMethods(_bindingFlags), bindingFlags);
        }

        public static IEnumerable<EventInfo> GetAllEvents(
            this Type _this,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            return _this.GetAllMembers((_type, _bindingFlags) => _type.GetEvents(_bindingFlags), bindingFlags);
        }

        public static PropertyInfo GetProperty(
            this Type _this,
            string propertyName,
            bool walkInterfaceInheritanceHierarchy)
        {
            return walkInterfaceInheritanceHierarchy
                       ? _this.GetAllProperties()
                              .FirstOrDefault(_pi => _pi.Name == propertyName)
                       : _this.GetProperty(propertyName);
        }
    }

    public static class TypeMemberDiscoverer<T>
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public;

        public static IEnumerable<PropertyInfo> GetAllProperties()
        {
            return _propertiesL.Value;
        }

        public static IEnumerable<EventInfo> GetAllEvents()
        {
            return _eventsL.Value;
        }

        public static IEnumerable<MethodInfo> GetAllMethods()
        {
            return _methodsL.Value;
        }

// ReSharper disable StaticFieldInGenericType
        private static readonly Lazy<IEnumerable<PropertyInfo>> _propertiesL =
            new Lazy<IEnumerable<PropertyInfo>>(
                () =>
                typeof (T).GetAllMembers(
                    (_type, _bindingFlags) => _type.GetProperties(_bindingFlags),
                    DefaultBindingFlags));

        private static readonly Lazy<IEnumerable<EventInfo>> _eventsL =
            new Lazy<IEnumerable<EventInfo>>(
                () =>
                typeof (T).GetAllMembers((_type, _bindingFlags) => _type.GetEvents(_bindingFlags), DefaultBindingFlags));

        private static readonly Lazy<IEnumerable<MethodInfo>> _methodsL =
            new Lazy<IEnumerable<MethodInfo>>(
                () =>
                typeof (T).GetAllMembers((_type, _bindingFlags) => _type.GetMethods(_bindingFlags), DefaultBindingFlags));

// ReSharper restore StaticFieldInGenericType
    }

    internal static class TypeMemberDiscoveryHelper
    {
        internal static IEnumerable<TMember> GetAllMembers<TMember>(
            this Type type,
            Func<Type, BindingFlags, IEnumerable<TMember>> accessor,
            BindingFlags bindingFlags) where TMember : MemberInfo
        {
            return type.IsInterface
                       ? type.GetInterfaceMembers(_ => accessor(_, bindingFlags | BindingFlags.DeclaredOnly))
                       : type.IsClass
                             ? type.GetClassMembers(_ => accessor(_, bindingFlags & ~(BindingFlags.DeclaredOnly)))
                             : type.GetStructMembers(_ => accessor(_, bindingFlags & ~(BindingFlags.DeclaredOnly)));
        }

        private static IEnumerable<TMember> GetClassMembers<TMember>(
            this Type type,
            Func<Type, IEnumerable<TMember>> accessor) where TMember : MemberInfo
        {
            Debug.Assert(type.IsClass);

            return accessor(type);
        }

        private static IEnumerable<TMember> GetStructMembers<TMember>(
            this Type type,
            Func<Type, IEnumerable<TMember>> accessor) where TMember : MemberInfo
        {
            Debug.Assert(type.IsValueType);

            return accessor(type);
        }

        private static IEnumerable<TMember> GetInterfaceMembers<TMember>(
            this Type type,
            Func<Type, IEnumerable<TMember>> accessor,
            ISet<Type> processedInterfaces = null) where TMember : MemberInfo
        {
            Debug.Assert(type.IsInterface);

            processedInterfaces = processedInterfaces ?? new HashSet<Type>();

            if (processedInterfaces.Contains(type)) { yield break; }

            foreach (var _pi in accessor(type)) { yield return _pi; }

            foreach (var _pi in type.GetInterfaces()
                                    .SelectMany(_ => _.GetInterfaceMembers(accessor, processedInterfaces))) {
                                        yield return _pi;
                                    }

            processedInterfaces.Add(type);
        }
    }
}