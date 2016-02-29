using System;

namespace BrightSword.SwissKnife
{
    /// <summary>
    ///     Extension methods to coerce loosely-typed object values into a strictly specified type
    /// </summary>
    /// <remarks>
    ///     This code is made available for open-source use at http://swissknife.codeplex.com/
    /// </remarks>
    /// <see cref="http://swissknife.codeplex.com/" />
    public static class CoerceExtensions
    {
        /// <summary>
        ///     Tries to coerce the value contained in <paramref name="value" /> to the specified <paramref name="targetType" />
        /// </summary>
        /// <param name="value">
        ///     The source value to coerce into <paramref name="targetType" />
        /// </param>
        /// <param name="targetType">
        ///     The target type to coerce <paramref name="value" /> to
        /// </param>
        /// <param name="defaultValue"> The default value to return if coercion is not successful </param>
        /// <returns> the coerced type as an object </returns>
        public static object CoerceType(this object value, Type targetType, object defaultValue)
        {
            if (targetType.IsInstanceOfType(value)) { return value; }

            if ((value is string)
                && (targetType == typeof (bool)))
            {
                var stringValue = value.ToString();

                if (string.IsNullOrEmpty(stringValue)) { return false; }

                if (stringValue.Equals("y", StringComparison.InvariantCultureIgnoreCase)
                    || stringValue.Equals("True", StringComparison.InvariantCultureIgnoreCase)) {
                        return true;
                    }

                if (stringValue.Equals("n", StringComparison.InvariantCultureIgnoreCase)
                    || stringValue.Equals("False", StringComparison.InvariantCultureIgnoreCase)) {
                        return false;
                    }

                throw new ArgumentException("Boolean should be either 'y' or 'n'");
            }

            object result;

            if (targetType.IsEnum)
            {
                if (value.CoerceType(
                    targetType,
                    out result,
                    _ => true,
                    (_type, _value) => Enum.Parse(_type, _value.ToString(), true),
                    defaultValue)) {
                        return result;
                    }
                //throw new InvalidCastException(String.Format("Cannot cast {0} to {1}",
                //                                             value,
                //                                             targetType));
            }

            if (value.CoerceType(targetType, out result, (_, _value) => bool.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => decimal.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => long.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => int.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => short.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => byte.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => char.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => double.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => float.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            if (value.CoerceType(targetType, out result, (_, _value) => DateTime.Parse(_value.ToString()), defaultValue)) {
                return result;
            }

            try {
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception) {
                return defaultValue;
            }
        }

        /// <summary>
        ///     Tries to coerce the value contained in <paramref name="value" /> to the specified <paramref name="targetType" /> ,
        ///     passing back the coerced value in
        ///     <param
        ///         name="returnValue" />
        ///     Utility function to streamline coercion to CLR types.
        /// </summary>
        /// <param name="value">
        ///     The source value to coerce into <paramref name="targetType" />
        /// </param>
        /// <param name="targetType">
        ///     The target type to coerce <paramref name="value" /> to
        /// </param>
        /// <param name="parseFunc">
        ///     A method which parses a loose-typed object into an instance of
        ///     <typeparam name="T" />
        /// </param>
        /// <param name="defaultValue"> The default value to return if coercion is not successful </param>
        /// <param name="returnValue"> The coerced value </param>
        /// <returns> true if coercion succeeded, false otherwise </returns>
        /// <example>
        ///     <code>if (value.CoerceType(targetType, out result, (_, _value) => bool.Parse(_value.ToString()), defaultValue))
        ///     {
        ///     return result;
        ///     }
        /// 
        ///     if (value.CoerceType(targetType, out result, (_, _value) => decimal.Parse(_value.ToString()), defaultValue))
        ///     {
        ///     return result;
        ///     }</code>
        /// </example>
        public static bool CoerceType<T>(
            this object value,
            Type targetType,
            out object returnValue,
            Func<Type, object, T> parseFunc,
            object defaultValue)
        {
            return value.CoerceType(
                targetType,
                out returnValue,
                _ => _.IsAssignableFrom(typeof (T)),
                (_type, _value) => parseFunc(_type, _value),
                defaultValue);
        }

        /// <summary>
        ///     Tries to coerce the value contained in <paramref name="value" /> to the specified <paramref name="targetType" /> ,
        ///     passing back the coerced value in
        ///     <param
        ///         name="returnValue" />
        /// </summary>
        /// <param name="value">
        ///     The source value to coerce into <paramref name="targetType" />
        /// </param>
        /// <param name="targetType">
        ///     The target type to coerce <paramref name="value" /> to
        /// </param>
        /// <param name="checkFunc">
        ///     A method which checks if the loose-typed object can be coerced into an instance of
        ///     <paramref
        ///         name="targetType" />
        /// </param>
        /// <param name="parseFunc">
        ///     A method which parses a loose-typed object into an instance of <paramref name="targetType" />
        /// </param>
        /// <param name="defaultValue"> The default value to return if coercion is not successful </param>
        /// <param name="returnValue"> The coerced value </param>
        /// <returns> true if coercion succeeded, false otherwise </returns>
        /// <example>
        ///     <code>return value.CoerceType(
        ///     targetType, out returnValue, _ => _.IsAssignableFrom(typeof (T)), (_type, _value) => parseFunc(_type, _value), defaultValue);</code>
        /// </example>
        public static bool CoerceType(
            this object value,
            Type targetType,
            out object returnValue,
            Func<Type, bool> checkFunc,
            Func<Type, object, object> parseFunc,
            object defaultValue)
        {
            var defaultReturnValue = ((defaultValue != null) && defaultValue.GetType()
                                                                            .IsAssignableFrom(targetType))
                                         ? defaultValue
                                         : targetType.IsValueType
                                               ? Activator.CreateInstance(targetType)
                                               : null;
            try
            {
                if (checkFunc(targetType))
                {
                    returnValue = parseFunc(targetType, value);
                    return true;
                }

                returnValue = defaultReturnValue;
                return false;
            }
            catch (Exception)
            {
                returnValue = defaultReturnValue;
                return true;
            }
        }
    }
}