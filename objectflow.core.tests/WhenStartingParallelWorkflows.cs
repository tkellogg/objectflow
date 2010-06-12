using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rhino.Mocks;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenStartingParallelWorkflows
    {
        private Workflow<string> _workflow;
        private WorkflowBuilders<string> _builders;
        private MockRepository _mocker;
        private WorkflowEngine<string> _engine;

        [SetUp]
        public void Given()
        {
            _engine = new WorkflowEngine<string>();
            _mocker = new MockRepository();
            _builders = _mocker.PartialMock<WorkflowBuilders<string>>(new[] { _engine });
            _workflow = new Workflow<string>(_builders);
        }

        [Test, Ignore]
        public void ShouldImplicitlyCloseParallelDefinitionOnStart()
        {
            _workflow.Do((a) => "red").And.Do((b) => "orange");

            var _pb = _mocker.DynamicMock<SequentialBuilder<string>>(new[] { _engine });
            Expect.Call(_pb.ParallelOperations).Return(new ParallelInvoker<string>());

            _mocker.ReplayAll();

            var f1 = new Func<string, string>(a => "red");
            var f2 = new Func<string, string>(a => "orange");

            _workflow.Start();
        }
    }
}
