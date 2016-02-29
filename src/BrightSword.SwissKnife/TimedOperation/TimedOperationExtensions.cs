using System;
using System.Diagnostics;

namespace BrightSword.SwissKnife
{
    public static class TimedOperationExtensions
    {
        public static TimeSpan Time(this Action _this, ITimedOperationObserver observer = null)
        {
            var stopWatch = new Stopwatch();
            observer = observer ?? new TraceObserver();

            stopWatch.Stop();
            stopWatch.Start();
            try
            {
                observer.Started();

                _this();

                observer.Succeeded();
            }
            catch (Exception e)
            {
                observer.FailedWithException(e);
                throw;
            }
            finally
            {
                stopWatch.Stop();
                observer.Completed(stopWatch.Elapsed);
            }

            return stopWatch.Elapsed;
        }

        public static TimedResult<T> Time<T>(this Func<T> _this, ITimedOperationObserver observer = null)
        {
            var stopWatch = new Stopwatch();
            observer = observer ?? new TraceObserver();

            stopWatch.Stop();
            stopWatch.Start();
            T result;
            try
            {
                observer.Started();

                result = _this();

                observer.Succeeded();
            }
            catch (Exception e)
            {
                observer.FailedWithException(e);
                throw;
            }
            finally
            {
                stopWatch.Stop();
                observer.Completed(stopWatch.Elapsed);
            }

            return new TimedResult<T>(result, stopWatch.Elapsed);
        }
    }
}