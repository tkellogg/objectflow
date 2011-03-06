using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.ParallelSplit
{
    public class WhenStartingParallelWorkflows:Specification
    {
        private Workflow<string> _workflow;

        [Scenario]
        public void Given_a_workflow_with_unclosed_parallel_operations()
        {
            ServiceLocator<string>.Reset();
            _workflow = new Workflow<string>();
            _workflow.Do((a) => "red").And.Do((b) => "orange");
        }

        [Observation]
        public void Should_implicitly_close_operations()
        {
            _workflow.Start();

            _workflow.RegisteredOperations.Tasks.Count.ShouldBe(1);
        }

        [Observation]
        public void Should_implicitly_close_operations_on_start()
        {
            _workflow.Start("Orange");

            _workflow.RegisteredOperations.Tasks.Count.ShouldBe(1);
    
        }

    }
}