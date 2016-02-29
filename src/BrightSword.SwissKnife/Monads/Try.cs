using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife
{
    public static class Try
    {
        #region Extension Methods to action on Func<> types

        public static Func<Try<TArgument>, IEnumerable<Try<TResult>>> MakeSafe<TArgument, TResult>(
            this Func<TArgument, IEnumerable<TResult>> function)
        {
            return Try<TResult>.Unit(function);
        }

        public static Func<Try<TArgument>, Try<TResult>> MakeSafe<TArgument, TResult>(
            this Func<TArgument, TResult> function)
        {
            return Try<TResult>.Unit(function);
        }

        public static Func<Try<TArgument>, int, Try<TResult>> MakeSafe<TArgument, TResult>(
            this Func<TArgument, int, TResult> function)
        {
            return Try<TResult>.Unit(function);
        }

        public static Func<IEnumerable<Try<TResult>>> MakeSafe<TResult>(this Func<IEnumerable<TResult>> function)
        {
            return Try<TResult>.Unit(function);
        }

        public static Func<Try<TResult>> MakeSafe<TResult>(this Func<TResult> function)
        {
            return Try<TResult>.Unit(function);
        }

        public static Action MakeSafe(this Action action)
        {
            return Try<object>.Unit(action);
        }

        public static Action<Try<TArgument>> MakeSafe<TArgument>(this Action<TArgument> action)
        {
            return Try<TArgument>.Unit(action);
        }

        #endregion

        #region SafeCall Utility Extensions

        public static void SafeCall<TResult>(this Func<TResult> function, Action<TResult> action)
        {
            function.MakeSafe()()
                .Bind(action);
        }

        public static Try<TResult> SafeCall<TArgument, TResult>(
            this Func<TArgument> first,
            Func<TArgument, TResult> second)
        {
            return second.MakeSafe()(first.MakeSafe()());
        }

        #endregion

        #region LINQ Syntactic Sugar for BIND on enumerables

        public static IEnumerable<Try<TResult>> SelectMany<TArgument, TResult>(
            this IEnumerable<Try<TArgument>> _this,
            Func<TArgument, IEnumerable<TResult>> function)
        {
// ReSharper disable once InvokeAsExtensionMethod
            return Enumerable.SelectMany(_this, _ => _.BindMany(function));
        }

        public static IEnumerable<Try<TResult>> Select<TArgument, TResult>(
            this IEnumerable<Try<TArgument>> _this,
            Func<TArgument, TResult> function)
        {
// ReSharper disable once InvokeAsExtensionMethod
            return Enumerable.Select(_this, _ => _.Bind(function));
        }

        //public static IEnumerable<Try<TResult>> Select<TArgument, TResult>(this IEnumerable<Try<TArgument>> _this,
        //                                                                              Func<TArgument, Try<TResult>> function)
        //{
        //    // ReSharper disable once InvokeAsExtensionMethod
        //    return Enumerable.Select(_this,
        //                             _ => _.Bind(function));
        //}

        public static IEnumerable<Try<TResult>> Select<TArgument, TResult>(
            this IEnumerable<Try<TArgument>> _this,
            Func<TArgument, int, TResult> function)
        {
// ReSharper disable once InvokeAsExtensionMethod
            return Enumerable.Select(_this, (_item, _index) => _item.Bind(function, _index));
        }

        //public static IEnumerable<Try<TResult>> Select<TArgument, TResult>(this IEnumerable<Try<TArgument>> _this,
        //                                                                              Func<TArgument, int, Try<TResult>> function)
        //{
        //    // ReSharper disable once InvokeAsExtensionMethod
        //    return Enumerable.Select(_this,
        //                             (_item,
        //                              _index) => _item.Bind(function,
        //                                                    _index));
        //}

        #endregion
    }

    public sealed class Try<T>
    {
        private Try(T result)
        {
            Result = result;
        }

        private Try(Exception exception)
        {
            Exception = exception;
        }

        private T Result { get; }
        private Exception Exception { get; }
        public bool IsException => Exception != null;

        #region BIND for side-effects

        public void Do(Action<T> action)
        {
            if (Exception != null) { throw Exception; }

            action(Result);
        }

        #endregion

        #region UNIT

        public static implicit operator Try<T>(T value)
        {
            return new Try<T>(value);
        }

        public static implicit operator Try<T>(Exception exception)
        {
            return new Try<T>(exception);
        }

        #endregion

        #region AMPLIFIED UNIT??

        public static Func<Try<TResult>> Unit<TResult>(Func<TResult> function)
        {
            return () =>
                   {
                       try {
                           return function();
                       }
                       catch (Exception exception) {
                           return exception;
                       }
                   };
        }

        public static Action Unit(Action action)
        {
            return () =>
                   {
                       try {
                           action();
                       }
                       catch
                       {
// ReSharper disable EmptyStatement
                           ;
// ReSharper restore EmptyStatement
                       }
                   };
        }

        public static Action<Try<TArgument>> Unit<TArgument>(Action<TArgument> action)
        {
            return _ =>
                   {
                       if (_.Exception != null) { return; }

                       Action curriedAction = () => action(_.Result);

                       Unit(curriedAction)();
                   };
        }

        public static Func<IEnumerable<Try<TResult>>> Unit<TResult>(Func<IEnumerable<TResult>> function)
        {
            return () =>
                   {
                       try
                       {
                           var results = function()
                               .ToList(); // ouch - is something seriously wrong here?

                           return from result in results
                                  select new Try<TResult>(result);
                       }
                       catch (Exception exception) {
                           return Enumerable.Repeat(new Try<TResult>(exception), 1);
                       }
                   };
        }

        public static Func<Try<TArgument>, Try<TResult>> Unit<TArgument, TResult>(Func<TArgument, TResult> function)
        {
            return _ =>
                   {
                       if (_.Exception != null) { return _.Exception; }

                       Func<TResult> curriedFunction = () => function(_.Result);
                       return Unit(curriedFunction)();
                   };
        }

        public static Func<Try<TArgument>, int, Try<TResult>> Unit<TArgument, TResult>(
            Func<TArgument, int, TResult> function)
        {
            return (_argument, _index) =>
                   {
                       if (_argument.Exception != null) { return _argument.Exception; }

                       Func<TResult> curriedFunction = () => function(_argument.Result, _index);
                       return Unit(curriedFunction)();
                   };
        }

        public static Func<Try<TArgument>, IEnumerable<Try<TResult>>> Unit<TArgument, TResult>(
            Func<TArgument, IEnumerable<TResult>> function)
        {
            return _ =>
                   {
                       if (_.Exception != null) { return Enumerable.Repeat(new Try<TResult>(_.Exception), 1); }

                       Func<IEnumerable<TResult>> curriedFunction = () => function(_.Result);
                       return Unit(curriedFunction)();
                   };
        }

        #endregion

        #region BIND

        public IEnumerable<Try<U>> BindMany<U>(Func<T, IEnumerable<U>> func)
        {
            return Exception == null
                       ? Unit(func)(Result)
                       : Enumerable.Repeat(new Try<U>(Exception), 1);
        }

        public void Bind(Action<T> action)
        {
            if (Exception != null) { return; }
            Unit(action)(Result);
        }

        public Try<U> Bind<U>(Func<T, U> func)
        {
            return Exception == null
                       ? Unit(func)(Result)
                       : new Try<U>(Exception);
        }

        public Try<U> Bind<U>(Func<T, int, U> func, int index)
        {
            return Exception == null
                       ? Unit(func)(Result, index)
                       : new Try<U>(Exception);
        }

        #endregion

        #region EXTRACT (ugliness)

        public T GetValueOrDefault(T defaultValue = default(T))
        {
            return Exception == null
                       ? Result
                       : defaultValue;
        }

        public Exception GetException()
        {
            return Exception;
        }

        #endregion
    }
}