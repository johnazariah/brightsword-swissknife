using System;
using System.Diagnostics;

namespace BrightSword.SwissKnife
{
    public static class CodeGenerationUtilities
    {
        public static string RenameToConcreteType(this Type _this, string prefix = "", string suffix = "")
        {
            return RenameToConcreteTypeInternal(_this, prefix, suffix, false);
        }

        private static string RenameToConcreteTypeInternal(
            this Type _this,
            string prefix,
            string suffix,
            bool calledFromPrintableName)
        {
            if (_this.IsGenericType)
            {
                if (_this.IsGenericTypeDefinition)
                {
                    if (_this.IsInterface)
                    {
                        return _this.Name.Substring(
                            _this.Name.StartsWith("I")
                                ? 1
                                : 0);
                    }

                    Debug.Assert(_this.IsClass);

                    return _this.Name;
                }

                return _this.PrintableName(prefix, suffix, _ => RenameToConcreteTypeInternal(_, prefix, suffix, true));
            }

            var printableName = calledFromPrintableName
                                    ? _this.Name
                                    : _this.PrintableName(prefix, suffix);

            if (_this.IsPrimitive
                || _this.IsClass) {
                    return printableName;
                }

            Debug.Assert(_this.IsInterface);

            return printableName.Substring(
                _this.Name.StartsWith("I")
                    ? 1
                    : 0);
        }
    }
}