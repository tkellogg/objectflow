using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;


namespace Objectflow.core.tests.ParallelSplit
{
    public class WhenDefiningParallelWorkflows:Specification
    {
        private Workflow<string> _workflow;

        [Scenario]
        public void Given()
        {
            _workflow = new Workflow<string>();
        }

        [Observation]
        public void ShouldCreateSequenceWithOperationsAdded()
        {
            _workflow.Do((a) => "red").And.Do((b) => "orange").And.Do((c) => "yellow").Then();
            Assert.That(_workflow.RegisteredOperations.Tasks.Count, Is.EqualTo(1));
            Assert.That(((ParallelInvoker<string>)_workflow.RegisteredOperations.Tasks[0].Command).RegisteredOperations.Count, Is.EqualTo(3));
        }
    }
}