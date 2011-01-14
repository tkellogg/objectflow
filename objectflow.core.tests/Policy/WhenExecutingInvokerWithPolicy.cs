using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.Policy
{
    public class WhenExecutingInvokerWithPolicy : Specification
    {
        private FailOnceOperation _operation;
        private MethodInvoker<string> _invoker;
        private Dispatcher<string> _engine;

        [Observation]
        public void ShouldUsePolicy()
        {

            _engine.Execute(new OperationDuplex<string>(_invoker));
            Assert.That(_operation.ExecuteCount, Is.EqualTo(2));
        }

        [Scenario]
        public void Given()
        {
            _operation = new FailOnceOperation();
            _invoker = new OperationInvoker<string>(_operation);
            _invoker.Policies.Add(new Retry(null));
            _engine = new Dispatcher<string>();
        }
    }
}
