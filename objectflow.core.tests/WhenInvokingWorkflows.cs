using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenInvokingWorkflows
    {
        [Test]
        public void ShouldInvokeWorkflow()
        {
            var _mock = new MockRepository();
            var innerWorkflow = _mock.DynamicMock<IWorkflow<string>>();
            innerWorkflow.Expect<IWorkflow<string>>((foo) => foo.Start("dd")).Return(null);
            _mock.ReplayAll();

            var invoker = new WorkflowInvoker<string>(innerWorkflow);
            invoker.Execute("dd");

            _mock.VerifyAll();
        }
    }
}
