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

        private static class CommandLineArgumentParser<T>
            where T : new()
        {
            private const BindingFlags C_BINDING_FLAGS =
                BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public;

            private static IEnumerable<PropertyInfo> Properties => typeof (T).GetProperties(C_BINDING_FLAGS)
                                                                             .ToList();

            public static T ParseFrom(string[] args) => SetSpecifiedValues(SetDefaultValues(new T()), args);

            private static T SetDefaultValues(T _this)
            {
                var defaultValueSetters = from property in Properties
                                          let claa = property.GetCustomAttribute<CommandLineArgumentAttribute>()
                                          where claa != null
                                          let defaultValue = claa.IsFlag ? false : claa.DefaultValue.CoerceType(property.PropertyType, null)
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

                return  parsedArguments.Aggregate(_this, FindAndSetPropertyValue);
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

                return propertySetters.Distinct().Aggregate(_this, (result, setter) => setter(result));
            }
        }
    }
}