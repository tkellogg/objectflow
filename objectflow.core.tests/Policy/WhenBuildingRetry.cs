using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;
using Rhino.Mocks;

namespace Objectflow.core.tests.Policy
{
    [TestFixture]
    public class WhenBuildingRetry
    {
        private IRetryPolicy _retry;
        private Workflow<string> _workflow;

        [SetUp]
        private void Given()
        {
            _workflow = Workflow<string>.Definition().Do((r) => "red");
            _retry = _workflow.Retry();
        }

        [Test]
        public void ShouldCreateRetryPolicy()
        {
            Given();

            Assert.IsNotNull(_retry, "object creation failed");
            Assert.That(_retry, Is.InstanceOf(typeof (Retry)));
        }

        [Test]
        public void ShouldSetDefaultTime()
        {
            Given();

            Assert.That(((Retry)_retry).IntervalTime, Is.EqualTo(0));
        }

        [Test]
        public void ShouldSetDefaultRetryAttempts()
        {
            Given();

            Assert.That(((Retry)_retry).Times, Is.EqualTo(1));            
        }

        [Test]
        public void ShouldAddRetryToInvoker()
        {
            Given();
            Assert.That(_workflow.RegisteredOperations.Count, Is.GreaterThan(0), "Number of operations");
            Assert.That(_workflow.RegisteredOperations[0].Command.Policies.Count, Is.EqualTo(1), "Policy");
        }

        [Test]
        public void ShouldSetIntervalMinutes()
        {
            _workflow = Workflow<string>.Definition().Do((r) => "red");
            _workflow.Retry().Once().With.Interval.Of.Minutes(1);                        

            Assert.That(((Retry)_workflow.RegisteredOperations[0].Command.Policies[0]).IntervalTime, Is.EqualTo(60 * 1000));
        }

        [Test]
        public void ShouldSetIntervalSeconds()
        {
            _workflow = Workflow<string>.Definition().Do((r) => "red");
            _workflow.Retry().Once().With.Interval.Of.Seconds(1);

            Assert.That(((Retry)_workflow.RegisteredOperations[0].Command.Policies[0]).IntervalTime, Is.EqualTo(1000));
        }

        [Test]
        public void ShouldSetIntervalMilliSeconds()
        {
            _workflow = Workflow<string>.Definition().Do((r) => "red");
            _workflow.Retry().Once().With.Interval.Of.Milliseconds(500);

            Assert.That(((Retry)_workflow.RegisteredOperations[0].Command.Policies[0]).IntervalTime, Is.EqualTo(500));
        }
    }
}
