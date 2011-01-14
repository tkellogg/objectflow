using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.tests.integration
{
    /// <summary>
    /// Contains integration tests for workflows that are composed of other workflows.
    /// </summary>
    public class CompositeWorkflows : Specification
    {
        [Observation]
        public void ShouldExecuteregisteredWorkflows()
        {
            IWorkflow<string> inner = Workflow<string>.Definition().Do((c) => c += ", orange, yellow");
            
            var outer = Workflow<string>.Definition()
                .Do(c => c += "red")
                .Do(inner);

            var result = outer.Start();

            result.ShouldBe("red, orange, yellow");
        }

        [Observation]
        public void ShouldExecuteRegisteredWorkflowsWithConstraints()
        {
            var inner = Workflow<string>.Definition().Do((c) => c += ", orange, yellow");
            var outer = Workflow<string>.Definition()
                .Do(c => c += "red")
                .Do(inner, If.IsTrue(true));

            var result = outer.Start();

            result.ShouldBe("red, orange, yellow");
        }

        [Observation]
        public void ShouldNotRunWorkflowForFailingConstraint()
        {
            var inner = Workflow<string>.Definition().Do((c) => c += ", orange, yellow");
            var outer = Workflow<string>.Definition()
                .Do(c => c += "red")
                .Do(inner, If.IsTrue(false));

            var result = outer.Start();

            result.ShouldBe("red");
        }
    }
}
