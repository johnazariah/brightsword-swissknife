using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace BrightSword.SwissKnife
{
    [ExcludeFromCodeCoverage]
    public class TraceObserver : ITimedOperationObserver
    {
        public TraceObserver(TextWriter trace = null)
        {
            Trace = trace ?? Console.Error;
        }

        protected TextWriter Trace { get; }

        public virtual void Started()
        {
            Trace.WriteLine(@"
    Starting :");
        }

        public virtual void Succeeded()
        {
            Trace.WriteLine(@"
    Starting : {0}");
        }

        public void Completed(TimeSpan elapsedTimeFromStart)
        {
            Trace.WriteLine(
                @"    Completed in : {0:dd} days {0:hh} hours {0:mm} minutes {0:ss\.fffffff} seconds",
                elapsedTimeFromStart);
        }

        public void FailedWithException(Exception exception)
        {
            Trace.WriteLine(@"    *** FAILURE with Exception {0} ", exception.Message);
        }
    }
}