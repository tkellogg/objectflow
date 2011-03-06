using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.PreviousOperationSuccess
{
    public class WhenCheckingPreviousOperationConstraint : Specification
    {
        private Colour _result;
        private Dispatcher<Colour> _dispatcher;

        [Scenario]
        public void Given_an_operation_that_depends_on_the_suceess_of_the_previous_failing_operation()
        {
            _dispatcher = new Dispatcher<Colour>();
            Dispatcher<Colour>.LastOperationSucceeded = false;
        }

        public void When_executing_workflow()
        {

        }

        [Observation]
        public void Should_not_execute_operation()
        {
            //When_executing_workflow();
            //_result.Name.ShouldBe("Red");
        }
    }

    public class FailingOperation : BasicOperation<Colour>
    {
        public override Colour Execute(Colour data)
        {
            SetSuccessResult(false);
            return data;
        }
    }
}