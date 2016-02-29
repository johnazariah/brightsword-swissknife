using System;

namespace BrightSword.SwissKnife
{
    public static class MonadExtensions
    {
        public static TResult Maybe<T, TResult>(
            this T _this,
            Func<T, TResult> func,
            TResult defaultResult = default(TResult)) where T : class
        {
            return _this == null
                       ? defaultResult
                       : func(_this);
        }

        public static T Maybe<T>(this T _this, Action<T> action) where T : class
        {
            if (_this != null) { action(_this); }

            return _this;
        }

        public static TResult When<T, TResult>(
            this T _this,
            Func<T, bool> predicate,
            Func<T, TResult> func,
            TResult defaultResult = default(TResult)) where T : class
        {
            return _this.Maybe(
                _ => predicate(_)
                         ? func(_)
                         : defaultResult,
                defaultResult);
        }

        public static T When<T>(this T _this, Func<T, bool> predicate, Action<T> action) where T : class
        {
            return _this.Maybe(
                _ =>
                {
                    if (predicate(_)) { action(_); }

                    return _;
                });
        }

        public static TResult Unless<T, TResult>(
            this T _this,
            Func<T, bool> predicate,
            Func<T, TResult> func,
            TResult defaultResult = default(TResult)) where T : class
        {
            return _this.Maybe(
                _ => !predicate(_)
                         ? func(_)
                         : defaultResult,
                defaultResult);
        }

        public static T Unless<T>(this T _this, Func<T, bool> predicate, Action<T> action) where T : class
        {
            return _this.Maybe(
                _ =>
                {
                    if (!predicate(_)) { action(_); }

                    return _;
                });
        }
    }
}