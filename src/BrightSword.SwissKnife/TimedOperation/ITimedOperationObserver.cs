using System;

namespace BrightSword.SwissKnife
{
    public interface ITimedOperationObserver
    {
        void Started();
        void Succeeded();
        void Completed(TimeSpan elapsedTimeFromStart);
        void FailedWithException(Exception exception);
    }
}