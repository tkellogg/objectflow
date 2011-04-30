using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace Objectflow.core.tests.FunctionalWorkflows
{
    public class WhenExecutingFunctions:Specification
    {
        private MockRepository _mock;
        private Workflow<string> _pipe;

        [Scenario]
        public void When()
        {
            _pipe = new Workflow<string>();
            _mock = new MockRepository();
        }

        [Observation]
        public void ShouldInvokeFunction()
        {
            var method = _mock.DynamicMock<Func<string, string>>();

            Expect.Call(method.Invoke("string parameter")).IgnoreArguments().Return("true");
            _mock.ReplayAll();

            _pipe.Do(method);
            _pipe.Start();

            method.VerifyAllExpectations();
        }

        [Observation]
        public void ShouldChainResult()
        {
            var method = new Func<string, string>(Method);

            _pipe
                .Do(method)
                .Do(new Func<string, string>((b) => { _result = b += "yellow"; return b; }));

            string result = _pipe.Start();

            Assert.That(result, Is.EqualTo("rainbow: yellow"));
        }

        public class TestPolicy : Rainbow.ObjectFlow.Policies.Policy, IPolicy
        {
            public int called;
            internal override T Execute<T>(T current)
            {
                called++;
                return default(T);
            }
        }

        private string _result;
        private Workflow<string> _workflow;

        public string Method(string param)
        {
            _result = "rainbow: ";
            return _result;
        }
    }
}