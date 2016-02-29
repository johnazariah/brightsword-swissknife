using System;
using System.Linq;

namespace BrightSword.SwissKnife
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType
                       ? Activator.CreateInstance(type)
                       : null;
        }

        public static string PrintableName(
            this Type _this,
            string prefix = "",
            string suffix = "",
            Func<Type, string> nameSelector = null)
        {
            nameSelector = nameSelector ?? (_ => _.Name);

            return _this.IsGenericType
                       ? GetPrintableNameForGenericType(_this, prefix, suffix, nameSelector)
                       : GetPrintableNameForNonGenericType(_this, prefix, suffix, nameSelector);
        }

        private static string GetPrintableNameForNonGenericType(
            Type _this,
            string prefix,
            string suffix,
            Func<Type, string> nameSelector)
        {
            var printableNameForNonGenericType = $"{prefix}{nameSelector(_this)}{suffix}";
            return printableNameForNonGenericType;
        }

        private static string GetPrintableNameForGenericType(
            Type _this,
            string prefix,
            string suffix,
            Func<Type, string> nameSelector)
        {
            var genericTypeName = nameSelector(_this.GetGenericTypeDefinition());

            var genericTypeArguments = string.Join(
                ", ",
                _this.GetGenericArguments()
                     .Select(_ => _.PrintableName(prefix, suffix, nameSelector)));

            var printableNameForGenericType =
                $"{prefix}{genericTypeName.Substring(0, genericTypeName.IndexOf('`'))}{suffix}<{genericTypeArguments}>";
            return printableNameForGenericType;
        }
    }
}