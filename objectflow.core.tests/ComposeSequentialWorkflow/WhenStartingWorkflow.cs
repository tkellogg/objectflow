using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Moq;

namespace Objectflow.core.tests.ComposeSequentialWorkflow
{
    public class WhenStartingWorkflow : Specification
    {
        private Workflow<string> _workflow;
        private Mock<Dispatcher<string>> _engine;
        private DefaultKernel _container;
        private TaskList<string> _taskList;

        [Scenario]
        public void Given()
        {
            _engine = new Mock<Dispatcher<string>>();

            _container = new DefaultKernel();
            ServiceLocator<string>.SetInstance(_container);
            _taskList = new TaskList<string>();
            _container.Register(Component.For<TaskList<string>>().Instance(_taskList));
            _container.Register(Component.For<SequentialBuilder<string>>().Instance(new SequentialBuilder<string>(_taskList)));
            _container.Register(Component.For<Dispatcher<string>>().Instance(_engine.Object));
            _workflow = new Workflow<string>();
        }

        [Observation]
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