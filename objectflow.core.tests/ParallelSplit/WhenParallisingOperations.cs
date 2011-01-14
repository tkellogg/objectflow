using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rhino.Mocks;

namespace Objectflow.core.tests.ParallelSplit
{
    public class WhenParallisingOperations:Specification
    {
        private Workflow<string> _workflow;
        private MockRepository _mocker;
        private Dispatcher<string> _engine;

        [Scenario]
        public void Given()
        {
            _mocker = new MockRepository();
            _engine = _mocker.DynamicMock<Dispatcher<string>>();
            _workflow = new Workflow<string>();
        }

        [Observation]
        public void ShouldUseExecutonEngine()
        {
            _workflow.Do(a => a += ", yellow").And.Do(b => b += ", orange").Then();
            var func1 = ((ParallelInvoker<string>)_workflow.RegisteredOperations.Tasks[0].Command).RegisteredOperations[0];
            var func2 = ((ParallelInvoker<string>)_workflow.RegisteredOperations.Tasks[0].Command).RegisteredOperations[1];

            var ps = new ParallelInvoker<string>(_engine);

            ps.RegisteredOperations.Add(func1);
            ps.RegisteredOperations.Add(func2);

            Expect.Call(_engine.Execute(func1, "Red")).Return("Red, yellow").IgnoreArguments().Repeat.Twice();
            _mocker.ReplayAll();

            var result = ps.Execute("Red");
            _mocker.VerifyAll();
        }
    }
}