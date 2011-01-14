using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.tests.integration
{
    public class WhenUsingRetryPolicyWithFunctions : Specification
    {
        private IWorkflow<string> _workflow;

        [Scenario]
        public void Given()
        {
            _workflow = Workflow<string>.Definition() as IWorkflow<string>;
        }

        [Observation]
        public void ShouldNotRetrySuccessfullFunction()
        {
            _workflow.Do((s) => s += "Red").Retry().Twice().With.Interval.Of.Seconds(2);

            var result = _workflow.Start();

           result.ShouldBe("Red");
        }
    }
}
