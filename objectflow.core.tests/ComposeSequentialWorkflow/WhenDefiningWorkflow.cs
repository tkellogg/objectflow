using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Moq;
using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests.ComposeSequentialWorkflow
{
    [TestFixture]
    public class WhenDefiningWorkflow
    {
        private IOperation<Colour> _doubleSpace;
        private Workflow<Colour> _workflow;
        private IKernel _container;
        private TaskList<Colour> _taskList;

        [SetUp]
        public void Given()
        {
            _taskList = new TaskList<Colour>();
            _container = new DefaultKernel();
            ServiceLocator<Colour>.SetInstance(_container);
            _container.Register(Component.For<TaskList<Colour>>().Instance(_taskList));
            _container.Register(Component.For<SequentialBuilder<Colour>>().Instance(new SequentialBuilder<Colour>(_taskList)));
            _container.Register(Component.For<WorkflowEngine<Colour>>().Instance(new WorkflowEngine<Colour>()));
            _workflow = new Workflow<Colour>();
            _doubleSpace = new DoubleSpace();
        }

        [Test]
        public void ShouldRegisterOperation()
        {
            _workflow.Do(_doubleSpace);

            Assert.That(_taskList.Tasks.Count, Is.EqualTo(1));
            Assert.That(_taskList.Tasks[0].Command.Equals(_doubleSpace));
        }

        [Test]
        public void ShouldCheckForNullOperation()
        {
            const BasicOperation<Colour> operation = null;
            Assert.Throws<ArgumentNullException>(() => _workflow.Do(operation), "thrown exception");
        }

        [Test]
        public void ShouldCheckForNullConstraint()
        {
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workflow.Do(_doubleSpace, null));

            Assert.That(exception.Message, Is.StringContaining("Argument [constraint] cannot be null"));
        }

        [Test]
        public void ShouldCheckForNullOperationWithConstraint()
        {
            const IOperation<Colour> operation = null;
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workflow.Do(operation, If.IsTrue(true)));

            Assert.That(exception.Message, Is.StringContaining("Argument [operation] cannot be null"));
        }

        [Test]
        public void ShouldCheckForNullOperationBeforeNullConstraint()
        {
            const IOperation<Colour> operation = null;
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workflow.Do(operation, null));

            Assert.That(exception.Message, Is.StringContaining("Argument [operation] cannot be null"));
        }

        [Test]
        public void ShouldCheckOperationIsInstanceOfOperationTemplate()
        {
            var operation = new Mock<IOperation<Colour>>();
            Assert.Throws<InvalidCastException>(() => Workflow<Colour>.Definition().Do(operation.Object), "Exception");
        }

        [Test]
        public void ShouldCheckOperationInstanceWithConstraints()
        {
            var operation = new Mock<IOperation<Colour>>();
            Assert.Throws<InvalidCastException>(() => Workflow<Colour>.Definition().Do(operation.Object, If.IsTrue(true)), "Exception");
        }
   }
}