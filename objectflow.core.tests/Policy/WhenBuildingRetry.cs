using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.Policy
{
    public class WhenBuildingRetry : Specification
    {
        private IRetryPolicy _retry;
        private Workflow<string> _workflow;

        [Scenario]
        public void Given()
        {
            ServiceLocator<string>.SetInstance(null);
            _workflow = Workflow<string>.Definition().Do((r) => "red") as Workflow<string>;
        }

        [Observation]
        public void ShouldCreateRetryPolicy()
        {
            _retry = _workflow.Retry();
            Assert.IsNotNull(_retry, "object creation failed");
            Assert.That(_retry, Is.InstanceOf(typeof(Retry)));
        }

        [Observation]
        public void ShouldSetDefaultTime()
        {
            _retry = _workflow.Retry();
            Assert.That(((Retry)_retry).IntervalTime, Is.EqualTo(0));
        }

        [Observation]
        public void ShouldSetDefaultRetryAttempts()
        {
            _retry = _workflow.Retry();
            Assert.That(((Retry)_retry).Count, Is.EqualTo(1));
        }

        [Observation]
        public void ShouldAddRetryToInvoker()
        {
            _retry = _workflow.Retry();
            Assert.That(_workflow.RegisteredOperations.Tasks.Count, Is.GreaterThan(0), "Number of operations");
            Assert.That(_workflow.RegisteredOperations.Tasks[0].Command.Policies.Count, Is.EqualTo(1), "Policy");
        }

        [Observation]
        public void ShouldSetIntervalMinutes()
        {
            _workflow.Retry().Once().With.Interval.Of.Minutes(1);

            Assert.That(((Retry)_workflow.RegisteredOperations.Tasks[0].Command.Policies[0]).IntervalTime, Is.EqualTo(60 * 1000));
        }

        [Observation]
        public void ShouldSetIntervalSeconds()
        {
            _workflow.Retry().Once().With.Interval.Of.Seconds(1);

            Assert.That(((Retry)_workflow.RegisteredOperations.Tasks[0].Command.Policies[0]).IntervalTime, Is.EqualTo(1000));
        }

        [Observation]
        public void ShouldSetIntervalMilliSeconds()
        {
            _workflow.Retry().Once().With.Interval.Of.Milliseconds(500);

            Assert.That(((Retry)_workflow.RegisteredOperations.Tasks[0].Command.Policies[0]).IntervalTime, Is.EqualTo(500));
        }
    }
}