using System;

namespace BrightSword.SwissKnife
{
    public static class Validator
    {
        public static bool Check<TException>(this bool condition, Func<TException> exceptionFactory = null)
            where TException : Exception, new()
        {
            if (condition) { return true; }

            exceptionFactory = exceptionFactory ?? (() => new TException());
            throw exceptionFactory();
        }

        public static bool Check<TException>(this Func<bool> predicate, Func<TException> exceptionFactory = null)
            where TException : Exception, new()
        {
            if (predicate()) { return true; }

            exceptionFactory = exceptionFactory ?? (() => new TException());
            throw exceptionFactory();
        }
    }
}