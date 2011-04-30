using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.Policy
{
    public class WhenTerminatingRetry : Specification
    {
        private Workflow<Colour> _flow;

        [Scenario]
        public void Given()
        {
            _flow = new Workflow<Colour>();
        }

        [Observation]
        public void ShouldAddOperationAfterOnce()
        {
            _flow
                .Do<DoubleSpace>().Retry().Once()
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }

        [Observation]
        public void ShouldAddOperatinAfterTwice()
        {
            _flow
                .Do<DoubleSpace>().Retry().Twice()
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
 
        }

        [Observation]
        public void ShouldAddOperationAfterAttempts()
        {
            _flow
                .Do<DoubleSpace>().Retry().Attempts(3)
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }
    }
}
