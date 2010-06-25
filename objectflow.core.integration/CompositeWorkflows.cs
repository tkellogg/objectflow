using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.integration
{
    [TestFixture]
    public class CompositeWorkflows
    {
        [Test]
        public void ShouldAllowWorkflowsInDefinition()
        {
            IWorkflow<string> inner = Workflow<string>.Definition().Do((c) => c += ", orange, yellow");
            var outer = Workflow<string>.Definition()
                .Do(c => c += "red")
                .Do(inner);

            var result = outer.Start();

            Assert.That(result, Is.EqualTo("red, orange, yellow"));
        }

        [Test]
        public void ShouldAllowWorkflowsWithConstraintInDefinition()
        {
            var inner = Workflow<string>.Definition().Do((c) => c += ", orange, yellow");
            var outer = Workflow<string>.Definition()
                .Do(c => c += "red")
                .Do(inner, If.IsTrue(true));

            var result = outer.Start();

            Assert.That(result, Is.EqualTo("red, orange, yellow"));
        }

        [Test]
        public void ShouldNotRunWorkflowForFailingConstraint()
        {
            var inner = Workflow<string>.Definition().Do((c) => c += ", orange, yellow");
            var outer = Workflow<string>.Definition()
                .Do(c => c += "red")
                .Do(inner, If.IsTrue(false));

            var result = outer.Start();

            Assert.That(result, Is.EqualTo("red"));
        }
    }
}
