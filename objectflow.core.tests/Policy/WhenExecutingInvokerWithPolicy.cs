using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.Policy
{
    [TestFixture]
    public class WhenExecutingInvokerWithPolicy
    {
        [Test]
        public void ShouldUsePolicy()
        {
            var operation = new FailOnceOperation();
            MethodInvoker<string> function = new OperationInvoker<string>(operation);
            function.Policies.Add(new Retry());
            var engine = new WorkflowEngine<string>();

            engine.Execute(new OperationConstraintPair<string>(function));
            Assert.That(operation.ExecuteCount, Is.EqualTo(2));
        }
    }

    public class FailOnceOperation : BasicOperation<string>
    {
        private static bool _fail = true;
        public int ExecuteCount;

        public override string Execute(string data)
        {
            ExecuteCount++;

            SetSuccessResult(!_fail);
            _fail = !_fail;

            return data;
        }
    }
}
