using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    public abstract class CommandLineArgumentBase
    {
        [CommandLineArgument("help", "Prints this message and quits", IsFlag = true)]
        public bool HelpRequested { get; set; }

        [CommandLineArgument("verbose",
            "Signals that the program should provide as much diagnostic information as possible", IsFlag = true)]
        public bool VerboseRequested { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CommandLineArgumentAttribute : Attribute
    {
        public CommandLineArgumentAttribute(string name, string description, object defaultValue = null)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public bool IsFlag { get; set; }
    }

    public static class CommandLineArgumentProcessor
    {
        public static int Run<T>(this string[] args, Func<T, int> main) where T : CommandLineArgumentBase, new()
        {
            var parsedArgs = args.ParseArguments<T>();
            if (!parsedArgs.IsValidCommandLineParameterSet()
                || parsedArgs.HelpRequested)
            {
                Console.WriteLine(parsedArgs.Usage());
                return -1;
            }

            if (parsedArgs.VerboseRequested) { Console.WriteLine(parsedArgs.Usage()); }

            return main(parsedArgs);
        }

        public static T ParseArguments<T>(this string[] args) where T : new()
        {
            return CommandLineArgumentParser<T>.ParseFrom(args);
        }

        public static bool IsValidCommandLineParameterSet<T>(this T _this) where T : new()
        {
            return CommandLineArgumentParser<T>.IsValid(_this);
        }

        public static string Usage<T>(this T _this) where T : new()
        {
            return CommandLineArgumentParser<T>.Usage(_this);
        }

        private static class CommandLineArgumentParser<T>
            where T : new()
        {
            private const BindingFlags BINDING_FLAGS =
                BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public;

            private static IEnumerable<PropertyInfo> Properties => typeof (T).GetProperties(BINDING_FLAGS)
                .ToList();

            private static IEnumerable<PropertyInfo> PropertiesWithAttributes => from property in Properties
                let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                where claa != null
                select property;

            public static T ParseFrom(string[] args) { return SetSpecifiedValues(SetDefaultValues(new T()), args); }

            private static T SetDefaultValues(T _this)
            {
                var defaultValueSetters = from property in PropertiesWithAttributes
                    let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                    let defaultValue = claa.IsFlag
                        ? false
                        : claa.DefaultValue.CoerceType(property.PropertyType, null)
                    select new Func<T, T>(
                        _ =>
                        {
                            property.SetValue(_, defaultValue, null);
                            return _;
                        });

                return defaultValueSetters.Aggregate(_this, (result, setter) => setter(result));
            }

            private static T SetSpecifiedValues(T _this, string[] args)
            {
                var parsedArguments = (from _arg in args
                    let parts = _arg.Split('=')
                    let name = parts[0].StartsWith("--")
                        ? parts[0].Substring("--".Length)
                        : parts[0]
                    let value = parts.Length > 1
                        ? parts[1].Trim()
                        : "true"
                    select new KeyValuePair<string, string>(name, value)).ToDictionary(
                        _ => _.Key,
                        _ => _.Value,
                        StringComparer.InvariantCultureIgnoreCase);

                return parsedArguments.Aggregate(_this, FindAndSetPropertyValue);
            }

            private static T FindAndSetPropertyValue(T _this, KeyValuePair<string, string> argumentKeyValuePair)
            {
                var argumentName = argumentKeyValuePair.Key;
                var argumentValue = argumentKeyValuePair.Value;

                Func<string, bool> matchesArgument =
                    _ => (string.Compare(argumentName, _, StringComparison.InvariantCultureIgnoreCase) == 0);

                var propertySetters = from property in Properties
                    let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                    where (claa == null ? matchesArgument(property.Name) : matchesArgument(claa.Name))
                    let specifiedValue = claa.Maybe(_ =>_.IsFlag)
                        ? true
                        : argumentValue.CoerceType(property.PropertyType, null)
                    select new Func<T, T>(
                        _ =>
                        {
                            Console.WriteLine("{0} <- {1}", property.Name, specifiedValue);
                            property.SetValue(_, specifiedValue, null);
                            return _;
                        });

                return propertySetters.Distinct()
                    .Aggregate(_this, (result, setter) => setter(result));
            }

            public static bool IsValid(T _this)
            {
                var values = from property in PropertiesWithAttributes
                    let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                    select property.GetValue(_this, null);

                return values.All(_ => _ != null);
            }

            public static string Usage(T _this)
            {
                Func<CommandLineArgumentAttribute, PropertyInfo, bool> isOptional =
                    (claa, property) => property.PropertyType.IsEnum || claa.Maybe(_ =>_.IsFlag || _.DefaultValue != null);

                Func<CommandLineArgumentAttribute, PropertyInfo, string> buildArgumentDescriptor = (claa, property) =>
                {
                    var name = claa.Maybe(_ => _.Name, property.Name);
                    var isFlag = claa.Maybe(_ => _.IsFlag);
                    var nameValueString = $"--{name}{(isFlag ? string.Empty : "=<value>")}";
                    return isOptional(claa, property)
                        ? $"[{nameValueString}]"
                        : nameValueString;
                };

                Func<string, string, string, string> buildUsageString =
                    (argumentDescriptor, optionStr, description) =>
                        $"***\t {argumentDescriptor,-18} : ({optionStr}) : {description}";

                Func<string, object, string> buildDefaultValueString =
                    (argumentName, defaultValue) => $"***\t\t --{argumentName} has a default value of [{defaultValue}]";

                Func<string, object, string> buildEffectiveValueString =
                    (argumentName, effectiveValue) =>
                        $"***\t\t The effective value of --{argumentName} is [{effectiveValue}]";

                Func<PropertyInfo, string, string> buildEnumValuesString =
                    (property, argumentName) => property.PropertyType.IsEnum
                        ? $"***\t\t --{argumentName} : should be one of [{string.Join(", ", Enum.GetNames(property.PropertyType))}]"
                        : null;

                var strings = (from property in Properties
                    let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                    let argumentName = claa.Maybe(_ =>_.Name, property.Name)
                    let description = claa.Maybe(_ => _.Description)
                    let optionStr = isOptional(claa, property)
                        ? "Optional"
                        : "Mandatory"
                    let defaultValue = claa.Maybe(_ => _.DefaultValue)
                    let effectiveValue = property.GetValue(_this, null)
                    let argumentDescriptor = buildArgumentDescriptor(claa, property)
                    let usageStr = buildUsageString(argumentDescriptor, optionStr, description)
                    let defaultValueStr = buildDefaultValueString(argumentName, defaultValue)
                    let effectiveValueStr = buildEffectiveValueString(argumentName, effectiveValue)
                    let enumValuesStr = buildEnumValuesString(property, argumentName)
                    select new
                    {
                        UsageString = usageStr,
                        DefaultValueString = defaultValueStr,
                        EffectiveValueString = effectiveValueStr,
                        EnumValuesString = enumValuesStr
                    }).ToList();

                var usageString = string.Join(
                    "\r\n",
                    strings.Select(_ => _.UsageString)
                        .Where(_ => !string.IsNullOrWhiteSpace(_)));
                var enumValuesString = string.Join(
                    "\r\n",
                    strings.Select(_ => _.EnumValuesString)
                        .Where(_ => !string.IsNullOrWhiteSpace(_)));
                var defaultValuesString = string.Join(
                    "\r\n",
                    strings.Select(_ => _.DefaultValueString)
                        .Where(_ => !string.IsNullOrWhiteSpace(_)));
                var effectiveValuesString = string.Join(
                    "\r\n",
                    strings.Select(_ => _.EffectiveValueString)
                        .Where(_ => !string.IsNullOrWhiteSpace(_)));

                return
                    $@"
***
*** Usage:
***
{usageString}
***
*** Enumerations:
***
{
                        (string.IsNullOrEmpty(enumValuesString)
                            ? "***"
                            : enumValuesString)}
***
*** Defaults:
***
{defaultValuesString
                        }
***
*** Effective Values:
***
{effectiveValuesString}
***";
            }
        }
    }
}