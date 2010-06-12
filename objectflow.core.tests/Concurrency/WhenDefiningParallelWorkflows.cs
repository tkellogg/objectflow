using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.Concurrency
{
    [TestFixture]
    public class WhenDefiningParallelWorkflows
    {
        private Workflow<string> _workflow;

        [SetUp]
        public void Given()
        {
            _workflow = new Workflow<string>();
        }

        [Test]
        public void ShouldCreateSequenceWithOperationsAdded()
        {
            _workflow.Do((a) => "red").And.Do((b) => "orange").And.Do((c) => "yellow").Then();
            Assert.That(_workflow.RegisteredOperations.Count, Is.EqualTo(1));
            Assert.That(((ParallelInvoker<string>)_workflow.RegisteredOperations[0].Command).RegisteredOperations.Count, Is.EqualTo(3));
        }
    }
}
