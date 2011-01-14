using System;
using objectflow.core.tests.integration.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.tests.integration 
{
    public class WhenUsingRetryPolicy : Specification
    {
        private OperationThatFailsNTimes _operation;
        private IWorkflow<string> _workflow;

        [Scenario]
        public void Given()
        {
            _operation = new OperationThatFailsNTimes();
            _workflow = Workflow<string>.Definition() as IWorkflow<string>;
        }

        [Observation]
        public void ShouldExecuteFailedOperationAgain()
        {
            _workflow.Do(_operation).Retry();
            _workflow.Start();

            _operation.ExecuteCount.ShouldBe(2);
        }

        [Observation]
        public void ShouldRetryManyTimes()
        {
            _workflow.Do(_operation).Retry().Attempts(3);
            _workflow.Start();
            _operation.ExecuteCount.ShouldBe(2);                       
        }

        [Observation]
        public void ShouldRetryOnce()
        {
            _workflow.Do(_operation).Retry().Once();
            _workflow.Start();
            _operation.ExecuteCount.ShouldBe(2);
        }

        [Observation]
        public void ShouldRetryTwice()
        {
            _operation = new OperationThatFailsNTimes(3);
            _workflow.Do(_operation).Retry().Twice();
            _workflow.Start();
            _operation.ExecuteCount.ShouldBe(3);
        }     
  
        [Observation]
        public void ShouldStopRetryingWhenSuccessful()
        {
            _workflow.Do(_operation).Retry().Attempts(4);
            _workflow.Start();

            _operation.ExecuteCount.ShouldBe(2);
        }

        [Observation]
        public void ShouldRetryWithInterval()
        {
            _workflow.Do(_operation).Retry().Twice().With.Interval.Of.Seconds(2);

            string beforeTime = DateTime.Now.ToLongTimeString();
            _workflow.Start();

            DateTime finishTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 2));
            
            finishTime.ToLongTimeString().ShouldBe(beforeTime);
        }

        [Observation]
        public void ShouldNotRetrySuccessfulOperation()
        {
            var workflow = new Workflow<Colour>();
                workflow.Do<DuplicateName>().Retry().Twice();
            
            var result = workflow.Start(new Colour("Red"));

            result.Name.ShouldBe("RedRed");
        }

    }
}
