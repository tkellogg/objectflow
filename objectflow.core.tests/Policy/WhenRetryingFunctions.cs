using System;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.Policy
{
    public class WhenRetryingFunctions:Specification
    {
        private Mock<FunctionInvoker<string>> _function;

        [Scenario]
        public void Given()
        {
            _function = new Mock<FunctionInvoker<string>>();
            _function.Setup((f) => f.Execute("Red")).Throws(new Exception());
            _function.Object.Policies.Add(new Retry(null)
            {
                Count = 2
            });
        }

        [Observation]
        public void ShouldExecuteFunction()
        {
            var engine = new Dispatcher<string>();
            engine.Execute(new OperationDuplex<string>(_function.Object), "Red");
            
            _function.Verify((f)=>f.Execute("Red"), Times.Exactly(3));            
        }
    }
}
