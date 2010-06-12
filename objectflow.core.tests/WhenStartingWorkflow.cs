using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rhino.Mocks;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenStartingWorkflow
    {
        private MockRepository _mocker;
        private Workflow<string> _workflow;
        private WorkflowEngine<string> _engine;

        [SetUp]
        public void Given()
        {
            _mocker = new MockRepository();
            _engine = _mocker.DynamicMock<WorkflowEngine<string>>();
            _workflow = new Workflow<string>(_engine);
        }

        [Test]
        public void ShouldUseExecutionEngine()
        {
            var operations =
                new List<OperationConstraintPair<string>>() { (new OperationConstraintPair<string>(new FunctionInvoker<string>(new Func<string, string>((b) => "1")))) };
            Expect.Call(_engine.Execute(operations)).Return("1");

            _mocker.ReplayAll();
            _workflow.RegisteredOperations = operations;

            var result = _workflow.Start();

            Assert.That(result, Is.EqualTo("1"));
            _mocker.VerifyAll();
        }
    }
}
