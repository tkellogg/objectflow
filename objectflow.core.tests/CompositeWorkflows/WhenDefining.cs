using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests.CompositeWorkflows
{
    [TestFixture]
    public class WhenDefiningCompositeWorkflows
    {
        private Workflow<string> _childWorkflow;
        private Workflow<string> _parentWorkflow;
        private TaskList<string> _taskList;

        [SetUp]
        public void Given()
        {
            ServiceLocator<string>.SetInstance(null);
            _childWorkflow = Workflow<string>.Definition().Do((c) => "red") as Workflow<string>;
            _parentWorkflow = Workflow<string>.Definition() as Workflow<string>;
            _taskList = new TaskList<string>();

        }

        [Test]
        public void ShouldCreateTaskListForEachWorkflow()
        {
            Assert.IsFalse(ReferenceEquals(_parentWorkflow.RegisteredOperations, _childWorkflow.RegisteredOperations), "Reference");
        }

        [Test]
        public void ShouldNotAllowNullWorkflows()
        {
            IWorkflow<string> workflow = null;

            Assert.Throws<ArgumentNullException>(() => Workflow<string>.Definition().Do(workflow));
        }

        [Test]
        public void ShouldNotAllowNullConstraints()
        {
            IWorkflow<string> workflow = Workflow<string>.Definition() as IWorkflow<string>;
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
            _parentWorkflow.Do(_childWorkflow);

            Assert.That(_parentWorkflow.RegisteredOperations.Tasks.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void ShouldAddWorkflowWithconstraints()
        {
            var innerWorkflow = Workflow<string>.Definition().Do(c => "red");
            var workflow = Workflow<string>.Definition().Do(innerWorkflow, If.IsTrue(true)) as Workflow<string>;

            Assert.That(workflow.RegisteredOperations.Tasks.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void AddWorkflowWithConstraint()
        {
            var childWorkflow = Workflow<string>.Definition() as IWorkflow<string>;
            _parentWorkflow = Workflow<string>.Definition().Do(childWorkflow, If.IsTrue(true)) as Workflow<string>;

            Assert.That(_parentWorkflow.RegisteredOperations.Tasks.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void ShouldAddWhenDefiningParallelCompositeWorkflow()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition().Do(c => "red");

            var builder = new ParallelSplitBuilder<string>(_taskList);
            builder.AddOperation(innerWorkflow);

            Assert.That(builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Test]
        public void ShouldAddWhenDefiningParallelCompositeWorkflowWithConstraint()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition().Do(c => "red");

            var builder = new ParallelSplitBuilder<string>(_taskList);
            builder.AddOperation(innerWorkflow, If.IsTrue(true));

            Assert.That(builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }
    }
}