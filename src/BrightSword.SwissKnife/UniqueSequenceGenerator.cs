using System;
using System.Collections.Generic;
using System.Threading;

namespace BrightSword.SwissKnife
{
    public static class UniqueSequenceGenerator
    {
        private static long _forwardCounter = DateTime.Now.Ticks;
        private static long _reverseCounter = DateTime.MaxValue.Ticks;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        static UniqueSequenceGenerator()
        {
            // ReSharper disable ObjectCreationAsStatement
            new Timer(ResetsCounters, null, 0, 1000);
            // ReSharper restore ObjectCreationAsStatement

            ResetReverseCounter();
        }

        public static long NextDescendingUniqueValue
        {
            get
            {
                try
                {
                    _lock.Wait();
                    Interlocked.Decrement(ref _reverseCounter);
                    return _reverseCounter;
                }
                finally {
                    _lock.Release();
                }
            }
        }

        public static long NextAscendingUniqueValue
        {
            get
            {
                try
                {
                    _lock.Wait();
                    Interlocked.Increment(ref _forwardCounter);
                    return _forwardCounter;
                }
                finally {
                    _lock.Release();
                }
            }
        }

        private static void ResetsCounters(object state)
        {
            ResetReverseCounter();
            ResetForwardCounter();
        }

        public static IEnumerable<long> GenerateDecreasingSequence(int length)
        {
            if (length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length), "A sequence must have a positive number of items");
            }
            for (var i = 0; i < length; i++) { yield return NextDescendingUniqueValue; }
        }

        public static IEnumerable<long> GenerateIncreasingSequence(int length)
        {
            if (length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length), "A sequence must have a positive number of items");
            }
            for (var i = 0; i < length; i++) { yield return NextAscendingUniqueValue; }
        }

        private static void ResetReverseCounter()
        {
            _lock.Wait();

            try
            {
                // ensure that _counter is monotonically decreasing and that the tick counter doesn't reset it to a larger number
                _reverseCounter = Math.Min(DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, _reverseCounter) - 1;
            }

            finally {
                _lock.Release();
            }
        }

        private static void ResetForwardCounter()
        {
            _lock.Wait();

            try
            {
                // ensure that _counter is monotonically increasing and that the tick counter doesn't reset it to a smaller number
                _forwardCounter = Math.Max(DateTime.Now.Ticks, _forwardCounter) + 1;
            }

            finally {
                _lock.Release();
            }
        }
    }
}