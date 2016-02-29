using System;
using System.Threading;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class TimingHelperTests
    {
        [Flags]
        public enum ObservedActions
        {
            None,

            Started,

            Succeeded,

            Completed,

            FailedWithException,

            CorrectSuccess = Started | Succeeded | Completed,

            CorrectFailure = Started | FailedWithException | Completed
        };

        private class TestObserver : ITimedOperationObserver
        {
            public ObservedActions ActionsObserved { get; private set; } = ObservedActions.None;

            public void Started()
            {
                ActionsObserved |= ObservedActions.Started;
            }

            public void Succeeded()
            {
                ActionsObserved |= ObservedActions.Succeeded;
            }

            public void Completed(TimeSpan elapsedTimeFromStart)
            {
                ActionsObserved |= ObservedActions.Completed;
            }

            public void FailedWithException(Exception exception)
            {
                ActionsObserved |= ObservedActions.FailedWithException;
            }

            public void Reset()
            {
                ActionsObserved = ObservedActions.None;
            }
        }

        [Test]
        public void Test_TimeShouldExecuteTheActionProvided_Failure()
        {
            Action failingAction = () => { throw new ArgumentNullException(); };

            Assert.Throws<ArgumentNullException>(() => failingAction.Time());
        }

        [Test]
        public void Test_TimeShouldExecuteTheActionProvided_Success()
        {
            Action succeedingAction = () => Thread.Sleep(100);
            var observer = new TestObserver();
            var timespan = succeedingAction.Time(observer);
            Assert.AreEqual(ObservedActions.CorrectSuccess, observer.ActionsObserved);

            Assert.IsTrue(timespan.TotalMilliseconds >= 10);
        }

        [Test]
        public void Test_TimeShouldExecuteTheFunctionProvided_Failure()
        {
            Func<DateTime> failingFunc = () => { throw new ArgumentNullException(); };
            var observer = new TestObserver();

            Assert.Throws<ArgumentNullException>(() => failingFunc.Time(observer));
            Assert.AreEqual(ObservedActions.CorrectFailure, observer.ActionsObserved);
        }

        [Test]
        public void Test_TimeShouldExecuteTheFunctionProvided_Success()
        {
            Func<int> succeedingFunc = () => 42;
            Assert.AreEqual(
                42,
                succeedingFunc.Time()
                              .Result);
        }
    }
}