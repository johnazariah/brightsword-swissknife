using System;

namespace BrightSword.SwissKnife
{
    public struct TimedResult<T>
    {
        public TimedResult(T result, TimeSpan elapsedTime) : this()
        {
            ElapsedTime = elapsedTime;
            Result = result;
        }

        public T Result { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
    }
}