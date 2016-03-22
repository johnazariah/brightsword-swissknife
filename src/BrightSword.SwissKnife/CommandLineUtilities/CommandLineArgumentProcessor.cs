using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    public static class CommandLineArgumentProcessor
    {
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
            private const BindingFlags C_BINDING_FLAGS =
                BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public;

            private static IEnumerable<PropertyInfo> Properties => typeof (T).GetProperties(C_BINDING_FLAGS)
                .ToList();

            private static IEnumerable<PropertyInfo> PropertiesWithAttributes => from property in Properties
                let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                where claa != null
                select property;

            public static T ParseFrom(string[] args) => SetSpecifiedValues(SetDefaultValues(new T()), args);

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
                        : null
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
                    where matchesArgument(claa?.Name) || matchesArgument(property.Name)
                    let specifiedValue = claa?.IsFlag ?? argumentValue.CoerceType(property.PropertyType, null)
                    select new Func<T, T>(
                        _ =>
                        {
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
                var strings = (from property in Properties
                    let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                    let argumentName = claa?.Name ?? property.Name
                    let argumentType = claa?.ParamType ?? property.PropertyType
                    let description = claa?.Description ?? ""
                    let isOptional = claa?.IsOptional ?? false
                    let isFlag = claa?.IsFlag ?? false
                    let defaultValue = claa?.DefaultValue
                    let effectiveValue = property.GetValue(_this, null)
                    let argumentDescriptor = claa?.ArgumentDescriptor ?? $"--{property.Name}=<value>"
                    let usageStr = $"***\t {argumentDescriptor,-18} : {isOptional}{description}"
                    let defaultValueStr = $"***\t\t --{argumentName} has a default value of [{defaultValue}]"
                    let effectiveValueStr = $"***\t\t The effective value of --{argumentName} is [{effectiveValue}]"
                    let enumValuesStr = claa?.ParamType.Maybe(
                        _ => _.IsEnum
                            ? $"***\t\t --{argumentName} should be one of [{string.Join(", ", Enum.GetNames(_))}]"
                            : "",
                        "")
                    select new
                    {
                        UsageString = usageStr,
                        DefaultValueString = defaultValueStr,
                        EffectiveValueString = effectiveValueStr,
                        EnumValuesString = enumValuesStr
                    }).ToList();

                var usageString = string.Join("\n", strings.Select(_ => _.UsageString));
                var enumValuesString = string.Join("\n", strings.Select(_ => _.EnumValuesString));
                var defaultValuesString = string.Join("\n", strings.Select(_ => _.DefaultValueString));
                var effectiveValuesString = string.Join("\n", strings.Select(_ => _.EffectiveValueString));

                return
                    $@"
*** [Usage]
{usageString}
***
{enumValuesString}
***
*** [Defaults]
{defaultValuesString}
***
*** [Effective Values]
{effectiveValuesString}
";

            }
        }
    }
}