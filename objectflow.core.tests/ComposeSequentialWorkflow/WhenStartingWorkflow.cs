using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Moq;

namespace Objectflow.core.tests.ComposeSequentialWorkflow
{
    [TestFixture]
    public class WhenStartingWorkflow
    {
        private Workflow<string> _workflow;
        private Mock<WorkflowEngine<string>> _engine;
        private DefaultKernel _container;
        private TaskList<string> _taskList;

        [SetUp]
        public void Given()
        {
            _engine = new Mock<WorkflowEngine<string>>();

            _container = new DefaultKernel();
            ServiceLocator<string>.SetInstance(_container);
            _taskList = new TaskList<string>();
            _container.Register(Component.For<TaskList<string>>().Instance(_taskList));
            _container.Register(Component.For<SequentialBuilder<string>>().Instance(new SequentialBuilder<string>(_taskList)));
            _container.Register(Component.For<WorkflowEngine<string>>().Instance(_engine.Object));
            _workflow = new Workflow<string>();
        }

        [Test]
        public void ShouldUseExecutionEngine()
        {
            var operation =
                new OperationDuplex<string>(new FunctionInvoker<string>(r => "1"));

            _taskList.Tasks.Add(operation);
            _engine.Setup(e => e.Execute(_taskList.Tasks)).Returns("1");

            _workflow.Start();

            _engine.VerifyAll();
        }
    }


}