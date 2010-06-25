using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenDefiningCompositeWorkflows
    {
        [Test]
        public void ShouldNotAllowNullWorkflows()
        {
            IWorkflow<string> workflow = null;

            Assert.Throws<ArgumentNullException>(() => Workflow<string>.Definition().Do(workflow));
        }

        [Test]
        public void ShouldNotAllowNullConstraints()
        {
            IWorkflow<string> workflow = Workflow<string>.Definition();
            ICheckConstraint constraint = null;

            Assert.Throws<ArgumentNullException>(() => Workflow<string>.Definition().Do(workflow, constraint));
        }

        [Test]
        public void ShouldNotAllowNullworkflowsWithConstraints()
        {
            IWorkflow<string> workflow = null;
            ICheckConstraint constraint = If.IsTrue(true);

            Assert.Throws<ArgumentNullException>(() => Workflow<string>.Definition().Do(workflow, constraint));
        }

        [Test]
        public void ShouldAddToWorkflowDefinition()
        {
            var innerWorkflow = Workflow<string>.Definition();
            var workflow = Workflow<string>.Definition().Do(innerWorkflow);

            Assert.That(workflow.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void AddWorkflow()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition();

            var builder = new SequentialBuilder<string>(workflow);
            builder.AddOperation(innerWorkflow);
            Assert.That(workflow.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void ShouldAddWorkflowWithconstraints()
        {
            var innerWorkflow = Workflow<string>.Definition();
            var workflow = Workflow<string>.Definition().Do(innerWorkflow, If.IsTrue(true));

            Assert.That(workflow.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void AddWorkflowWithConstraint()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition();

            var builder = new SequentialBuilder<string>(workflow);
            builder.AddOperation(innerWorkflow, If.IsTrue(true));
            Assert.That(workflow.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void ShouldAddWhenDefiningParallelCompositeWorkflow()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition();

            var builder = new ParallelSplitBuilder<string>(workflow);
            builder.AddOperation(innerWorkflow);

            Assert.That(builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void ShouldAddWhenDefiningParallelCompositeWorkflowWithConstraint()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition();

            var builder = new ParallelSplitBuilder<string>(workflow);
            builder.AddOperation(innerWorkflow, If.IsTrue(true));

            Assert.That(builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }
    }
}
