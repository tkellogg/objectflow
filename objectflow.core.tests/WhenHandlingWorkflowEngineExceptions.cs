using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenHandlingWorkflowEngineExceptions
    {
        [Test]
        public void ShouldReturnUnsuccessfull()
        {
            var engine = new WorkflowEngine<string>();
            var operation = new FailedOperation();

            var operationWrapper =
                new OperationConstraintPair<string>(new OperationInvoker<string>(operation));

            engine.Execute(operationWrapper);

            Assert.That(operation.SuccessResult, Is.EqualTo(false));
        }
    }
    public class FailedOperation : BasicOperation<string>
    {
        public override string Execute(string data)
        {
            throw new Exception("test exception");
        }
    }
}
