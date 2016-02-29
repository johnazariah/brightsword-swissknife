using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Field
        | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class TagAttribute : Attribute
    {
        public TagAttribute(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }

    /// <summary>
    ///     Extension methods to simplify and streamline operations with Attributes on members (Methods, Properties and
    ///     Fields).
    /// </summary>
    /// <remarks>
    ///     This code is made available for open-source use at http://swissknife.codeplex.com/
    /// </remarks>
    /// <see cref="http://swissknife.codeplex.com/" />
    public static class AttributeExtensions
    {
        /// <summary>
        ///     Get the first attribute of type <typeparamref name="TAttribute" /> decorating the type specified by
        ///     <paramref name="_this" />.
        /// </summary>
        /// <typeparam name="TAttribute"> Type of the attribute to search for </typeparam>
        /// <param name="_this"> The member on which to search for the attribute </param>
        /// <param name="flags">
        ///     Optional binding flags parameter to apply finer control on members being inspected. By default,
        ///     public and non-public instances are inspected.
        /// </param>
        /// <returns>
        ///     The first attribute of type <typeparamref name="TAttribute" /> decorating the member specified by
        ///     <paramref
        ///         name="_this" />
        ///     . Null if none such are found.
        /// </returns>
        public static TAttribute GetCustomAttribute<TAttribute>(
            this Type _this,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            return (TAttribute) _this.GetCustomAttributes(typeof (TAttribute), true)
                                     .FirstOrDefault();
        }

        /// <summary>
        ///     Get the first attribute of type <typeparamref name="TAttribute" /> decorating the member specified by
        ///     <paramref
        ///         name="_this" />
        ///     .
        /// </summary>
        /// <typeparam name="TAttribute"> Type of the attribute to search for </typeparam>
        /// <param name="_this"> The member on which to search for the attribute </param>
        /// <param name="flags">
        ///     Optional binding flags parameter to apply finer control on members being inspected. By default,
        ///     public and non-public instances are inspected.
        /// </param>
        /// <returns>
        ///     The first attribute of type <typeparamref name="TAttribute" /> decorating the member specified by
        ///     <paramref
        ///         name="_this" />
        ///     . Null if none such are found.
        /// </returns>
        public static TAttribute GetCustomAttribute<TAttribute>(
            this MemberInfo _this,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            return (TAttribute) _this.GetCustomAttributes(typeof (TAttribute), true)
                                     .FirstOrDefault();
        }

        /// <summary>
        ///     Get the first attribute of type <typeparamref name="TAttribute" /> decorating the type specified by
        ///     <paramref
        ///         name="_this" />
        ///     .
        /// </summary>
        /// <typeparam name="TAttribute"> Type of the attribute to search for </typeparam>
        /// <typeparam name="TResult">
        ///     Type of result expected. This can be the attribute itself or some property specified by the
        ///     <paramref
        ///         name="selector" />
        ///     function
        /// </typeparam>
        /// <param name="_this"> The member on which to search for the attribute </param>
        /// <param name="selector">
        ///     Optional func to select a property of the attribute to return. If none is specified, the
        ///     specified default value is returned
        /// </param>
        /// <param name="defaultValue">The default value to return if the attribute value is not found</param>
        /// <param name="flags">
        ///     Optional binding flags parameter to apply finer control on members being inspected. By default,
        ///     public and non-public instances are inspected.
        /// </param>
        /// <returns>
        ///     The first attribute of type <typeparamref name="TAttribute" /> decorating the member specified by
        ///     <paramref
        ///         name="_this" />
        ///     . Null if none such are found.
        /// </returns>
        public static TResult GetCustomAttributeValue<TAttribute, TResult>(
            this Type _this,
            Func<TAttribute, TResult> selector,
            TResult defaultValue = default(TResult),
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            return _this.GetCustomAttributes(typeof (TAttribute), true)
                        .FirstOrDefault()
                        .Maybe(_ => selector((TAttribute) _), defaultValue);
        }

        /// <summary>
        ///     Get the first attribute of type <typeparamref name="TAttribute" /> decorating the member specified by
        ///     <paramref
        ///         name="_this" />
        ///     .
        /// </summary>
        /// <typeparam name="TAttribute"> Type of the attribute to search for </typeparam>
        /// <typeparam name="TResult">
        ///     Type of result expected. This can be the attribute itself or some property specified by the
        ///     <paramref name="selector" /> function
        /// </typeparam>
        /// <param name="_this"> The member on which to search for the attribute </param>
        /// <param name="selector">
        ///     Optional func to select a property of the attribute to return. If none is specified, the
        ///     specified default value is returned
        /// </param>
        /// <param name="defaultValue">The default value to return if the attribute value is not found</param>
        /// <param name="flags">
        ///     Optional binding flags parameter to apply finer control on members being inspected. By default,
        ///     public and non-public instances are inspected.
        /// </param>
        /// <returns>
        ///     The first attribute of type <typeparamref name="TAttribute" /> decorating the member specified by
        ///     <paramref
        ///         name="_this" />
        ///     . Null if none such are found.
        /// </returns>
        public static TResult GetCustomAttributeValue<TAttribute, TResult>(
            this MemberInfo _this,
            Func<TAttribute, TResult> selector,
            TResult defaultValue = default(TResult),
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            return _this.GetCustomAttributes(typeof (TAttribute), true)
                        .FirstOrDefault()
                        .Maybe(_ => selector((TAttribute) _), defaultValue);
        }

        public static IEnumerable<TResult> GetCustomAttributeValues<TAttribute, TResult>(
            this MemberInfo _this,
            Func<TAttribute, TResult> selector,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            return _this.GetCustomAttributes(typeof (TAttribute), true)
                        .Select(_ => selector((TAttribute) _));
        }

        public static string GetName(this Enum value)
        {
            return value.GetType()
                        .GetField(value.ToString())
                        .GetCustomAttributeValue<NameAttribute, string>(_ => _.Value, value.ToString());
        }

        public static T GetNamedValue<T>(this Enum value, string name)
        {
            return value.GetType()
                        .GetField(value.ToString())
                        .GetCustomAttributes(typeof (TagAttribute), true)
                        .Cast<TagAttribute>()
                        .Where(_ => _.Name.Equals(name, StringComparison.InvariantCulture))
                        .Select(_ => _.Value)
                        .Cast<T>()
                        .FirstOrDefault();
        }

        public static T GetNamedValue<T>(this Type value, string name)
        {
            return value.GetCustomAttributes(typeof (TagAttribute), true)
                        .Cast<TagAttribute>()
                        .Where(_ => _.Name.Equals(name, StringComparison.InvariantCulture))
                        .Select(_ => _.Value)
                        .Cast<T>()
                        .FirstOrDefault();
        }
    }
}