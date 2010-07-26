using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace Objectflow.core.tests.Concurrency
{
    [TestFixture]
    public class ThenSpec
    {
        private Workflow<string> _workflow;
        private MockRepository _mocker;
        private IWorkflowBuilders<string> _factory;

        [SetUp]
        public void BeforeEachTest()
        {
            _mocker = new MockRepository();
            _factory = _mocker.DynamicMock<IWorkflowBuilders<string>>();
            _workflow = new Workflow<string>(_factory);
        }

        [Test]
        public void ShouldResetBuilderToLinear()
        {
            Expect.Call(_factory.GetParallelBuilder(_workflow)).Return(new ParallelSplitBuilder<string>(_workflow));
            Expect.Call(_factory.GetSequentialBuilder(_workflow)).Return(new SequentialBuilder<string>(_workflow));
            _mocker.ReplayAll();

            var pipe = _workflow.And.Do(c => "red").Then();
            _mocker.VerifyAll();
        }

        [Test]
        public void ShouldReturnWorkflowFromThen()
        {
            var pipe = _workflow.Then();

            Assert.That(pipe, Is.Not.Null, "object not instantiated");
            Assert.That(pipe.Equals(_workflow), "workflow instance");
        }

        [Test]
        public void ShouldAddSequenceToRegisteredOperationsOnThen()
        {
            _workflow = new Workflow<string>();

            _workflow.Do((a) => "1").And.Do((b) => "2").Then();

            Assert.That(_workflow.RegisteredOperations.Count, Is.EqualTo(1), "incorrect operations");
            Assert.That(_workflow.RegisteredOperations[0].Command, Is.InstanceOf(typeof(ParallelInvoker<string>)), "instance type");
        }

        [Test]
        public void ShouldAddMultipleOperationsToSequence()
        {
            _workflow = new Workflow<string>();
            _workflow.Do((a) => "1").And.Do((b) => "2").And.Do((b) => "2").Then();

            Assert.That(_workflow.RegisteredOperations.Count, Is.EqualTo(1), "operaton count");
            Assert.That(_workflow.RegisteredOperations[0].Command, Is.InstanceOf(typeof(ParallelInvoker<string>)), "Object type");
            Assert.That(((ParallelInvoker<string>)_workflow.RegisteredOperations[0].Command).RegisteredOperations.Count, Is.EqualTo(3), "RegisteredOperations");
        }
    }
}
