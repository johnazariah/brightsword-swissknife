using System;
using System.Collections.Concurrent;

namespace BrightSword.SwissKnife
{
    public static class Functional
    {
        /// <summary>
        ///     The Y fixed-point combinator for a function with one argument of type TArgument
        /// </summary>
        /// <typeparam name="TArgument">The type of the single argument of the function</typeparam>
        /// <typeparam name="TResult">The type of the result of the function</typeparam>
        /// <param name="func">A lambda whose first argument is a function which needs to be made anonymously recursive</param>
        /// <returns></returns>
        public static Func<TArgument, TResult> Y<TArgument, TResult>(
            Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
        {
            // ReSharper disable AccessToModifiedClosure
            Func<TArgument, TResult> funcFixed = null;

            funcFixed = func(_ => funcFixed(_));

            return funcFixed;
            // ReSharper restore AccessToModifiedClosure
        }

        private static Func<TArgument, TResult> Trace<TArgument, TResult>(this Func<TArgument, TResult> func)
        {
            return _ =>
                   {
#if TRACE
                       System.Diagnostics.Trace.WriteLine($"{func}({_}) called");
#endif
                       return func(_);
                   };
        }

        public static Func<TArgument, TResult> Memoize<TArgument, TResult>(this Func<TArgument, TResult> func)
        {
            var cache = new ConcurrentDictionary<TArgument, TResult>();

            return _ => cache.GetOrAdd(_, func.Trace());
        }

        public static Func<TArgument, TResult> MemoizeFix<TArgument, TResult>(
            Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
        {
            // ReSharper disable AccessToModifiedClosure
            Func<TArgument, TResult> funcMemoized = null;

            funcMemoized = func(_ => funcMemoized(_));
            funcMemoized = Memoize(funcMemoized);

            return funcMemoized;
            // ReSharper restore AccessToModifiedClosure
        }

#if MULTI_ARG_FUNCS
        private static Func<TArg1, TArg2, TResult> Memoize<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func)
        {
            var _map = new ConcurrentDictionary<Tuple<TArg1, TArg2>, TResult>();
            return (_a1,
                    _a2) => _map.GetOrAdd(
                        new Tuple<TArg1, TArg2>(
                                _a1,
                                _a2),
                        _ => func(
                            _.Item1,
                            _.Item2));
        }

        public static Func<TArg1, TArg2, TResult> MemoizeFix<TArg1, TArg2, TResult>(Func<Func<TArg1, TArg2, TResult>, Func<TArg1, TArg2, TResult>> func)
        {
            // ReSharper disable AccessToModifiedClosure
            Func<TArg1, TArg2, TResult> funcMemoized = null;

            funcMemoized = func(
                (_a1,
                 _a2) => funcMemoized(
                     _a1,
                     _a2));
            funcMemoized = funcMemoized.Memoize();

            return funcMemoized;
            // ReSharper restore AccessToModifiedClosure
        }

        private static Func<TArg1, TArg2, TArg3, TResult> Memoize<TArg1, TArg2, TArg3, TResult>(this Func<TArg1, TArg2, TArg3, TResult> func)
        {
            var _map = new ConcurrentDictionary<Tuple<TArg1, TArg2, TArg3>, TResult>();
            return (_a1,
                    _a2,
                    _a3) => _map.GetOrAdd(
                        new Tuple<TArg1, TArg2, TArg3>(
                                _a1,
                                _a2,
                                _a3),
                        _ => func(
                            _.Item1,
                            _.Item2,
                            _.Item3));
        }

        public static Func<TArg1, TArg2, TArg3, TResult> MemoizeFix<TArg1, TArg2, TArg3, TResult>(Func<Func<TArg1, TArg2, TArg3, TResult>, Func<TArg1, TArg2, TArg3, TResult>> func)
        {
            // ReSharper disable AccessToModifiedClosure
            Func<TArg1, TArg2, TArg3, TResult> funcMemoized = null;

            funcMemoized = func(
                (_a1,
                 _a2,
                 _a3) => funcMemoized(
                     _a1,
                     _a2,
                     _a3));
            funcMemoized = funcMemoized.Memoize();

            return funcMemoized;
            // ReSharper restore AccessToModifiedClosure
        }
#endif
    }
}