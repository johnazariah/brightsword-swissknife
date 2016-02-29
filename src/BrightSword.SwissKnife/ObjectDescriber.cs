using System;
using System.Linq.Expressions;

namespace BrightSword.SwissKnife
{
    public static class ObjectDescriber
    {
        private static string GetName<TFunc>(Expression<TFunc> e)
        {
            var member = e.Body as MemberExpression;
            if (member != null) { return member.Member.Name; }

            var method = e.Body as MethodCallExpression;
            if (method != null) { return method.Method.Name; }

            throw new NotSupportedException("Cannot operate on given expression: " + e.Body);
        }

        /// <summary>
        ///     Used when the expression refers to a method with void return
        /// </summary>
        /// <param name="e"> </param>
        /// <returns> </returns>
        public static string GetName(Expression<Action> e)
        {
            return GetName<Action>(e);
        }

        /// <summary>
        ///     Used when the expression refers to a method returning TResult on a non-null object participating in the expression
        /// </summary>
        /// <typeparam name="TResult"> </typeparam>
        /// <param name="selector"> </param>
        /// <returns> </returns>
        public static string GetName<TResult>(Expression<Func<TResult>> selector)
        {
            return GetName<Func<TResult>>(selector);
        }

        /// <summary>
        ///     Used when the expression refers to a method returning TResult, but when no non-null object exists
        /// </summary>
        /// <typeparam name="TArg"> </typeparam>
        /// <typeparam name="TResult"> </typeparam>
        /// <param name="selector"> </param>
        /// <returns> </returns>
        /// <example>
        ///     ObjectDescriber.GetName((IPayBillEvent b) => b.Lines)
        /// </example>
        public static string GetName<TArg, TResult>(Expression<Func<TArg, TResult>> selector)
        {
            return GetName<Func<TArg, TResult>>(selector);
        }

        /// <summary>
        ///     Used when the expression refers to a method returning TResult, but when no non-null object exists
        /// </summary>
        /// <typeparam name="TArg"> </typeparam>
        /// <param name="selector"> </param>
        /// <returns> </returns>
        /// <example>
        ///     ObjectDescriber.GetName((IPayBillEvent b) => b.Lines)
        /// </example>
        public static string GetName<TArg>(Expression<Action<TArg>> selector)
        {
            return GetName<Action<TArg>>(selector);
        }
    }
}