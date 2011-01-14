using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.RepeatUntil
{
    public class WhenConfiguringRepeat:Specification
    {
        private IDefine<string> _workflow;
        private Workflow<string> _flow;

        [Scenario]
        public void Given_an_operation_that_repeats_once()
        {
            _workflow = Workflow<string>.Definition();
            _workflow.Do((s) => s+="Red").Repeat().Once();
            _flow = _workflow as Workflow<string>;
        }

        [Observation]
        public void Should_register_one_operation_with_a_policy()
        {
            _flow.RegisteredOperations.Tasks.Count.ShouldBe(1);  
            _flow.RegisteredOperations.Tasks[0].Command.Policies.Count.ShouldBe(1);
        }

        [Observation]
        public void Should_set_retry_count_to_one()
        {
            ((Repeat)_flow.RegisteredOperations.Tasks[0].Command.Policies[0]).Count.ShouldBe(1);
        }

        [Observation]
        public void ShouldFlow()
        {
            
        }

    }
}
