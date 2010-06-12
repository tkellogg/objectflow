using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;

namespace objectflow.core.tests.Concurrency
{
    [TestFixture]
    public class AndSpecification
    {
        private Workflow<string> _workflow;

        [Test]
        public void Given()
        {
            _workflow = new Workflow<string>();
        }

        [Test]
        public void ShouldReturnWorkflow()
        {
            var pipe = _workflow.And;

            Assert.That(pipe, Is.Not.Null, "object not instantiated");
            Assert.That(pipe.Equals(_workflow), "workflow instance");
        }


    }
}
