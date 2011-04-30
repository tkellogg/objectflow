using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Moq;

namespace Objectflow.core.tests.ComposeSequentialWorkflow
{
    public class WhenStartingWorkflow : Specification
    {
        private Workflow<string> _workflow;
        private Mock<Dispatcher<string>> _engine;
        private TaskList<string> _taskList;

        [Scenario]
        public void Given()
        {
            _engine = new Mock<Dispatcher<string>>();
            _taskList = new TaskList<string>();
            _workflow = new Workflow<string>(_taskList);
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