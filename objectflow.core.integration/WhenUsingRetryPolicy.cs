using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.integration
{
    [TestFixture]
    public class WhenUsingRetryPolicy
    {
        private FailOnceOperation _operation;
        private IWorkflow<string> _workflow;

        [SetUp]
        public void Given()
        {
            _operation = new FailOnceOperation();
            _workflow = Workflow<string>.Definition() as IWorkflow<string>;
        }

        [Test]
        public void ShouldExecuteFailedOperationAgain()
        {
            // TODO: remove use of workflow as retry can be used directly now we have moved
            // execution responsbility to that instead of all on engine
            _workflow.Do(_operation).Retry();
            _workflow.Start();

            Assert.That(_operation.ExecuteCount, Is.EqualTo(2));
        }

        [Test]
        public void ShouldRetryManyTimes()
        {
            _workflow.Do(_operation).Retry().Attempts(2);
            _workflow.Start();
            Assert.That(_operation.ExecuteCount, Is.EqualTo(2));                       
        }

        [Test]
        public void ShouldRetryOnce()
        {
            _workflow.Do(_operation).Retry().Once();
            _workflow.Start();
            Assert.That(_operation.ExecuteCount, Is.EqualTo(2));
        }

        [Test]
        public void ShouldRetryTwice()
        {
            _operation = new FailOnceOperation(3);
            _workflow.Do(_operation).Retry().Twice();
            _workflow.Start();
            Assert.That(_operation.ExecuteCount, Is.EqualTo(3));
        }     
  
        [Test]
        public void ShouldStopRetryingWhenSuccessful()
        {
            _workflow.Do(_operation).Retry().Attempts(4);
            _workflow.Start();

            Assert.That(_operation.ExecuteCount, Is.EqualTo(2));
        }

        [Test]
        public void ShouldRetryWithInterval()
        {
            _workflow.Do(_operation).Retry().Twice().With.Interval.Of.Seconds(2);

            string beforeTime = DateTime.Now.ToLongTimeString();
            _workflow.Start();

            DateTime finishTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 2));
            
            Assert.That(finishTime.ToLongTimeString(), Is.EqualTo(beforeTime));
        }
    }

    // TODO: remove this and use mock objects
    public class FailOnceOperation : BasicOperation<string>
    {
        public int ExecuteCount;
        private readonly int _failcount;

        public FailOnceOperation()
            : this(1)
        {

        }

        public FailOnceOperation(int failcount)
        {
            _failcount = failcount;
        }

        public override string Execute(string data)
        {
            ExecuteCount++;

            if (ExecuteCount <= _failcount)
                SetSuccessResult(false);
            else
                SetSuccessResult(true);

            return data;
        }
    }
}
