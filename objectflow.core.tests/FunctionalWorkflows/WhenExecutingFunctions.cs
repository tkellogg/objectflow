using System;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Policies;
using Rhino.Mocks;

namespace Objectflow.core.tests.FunctionalWorkflows
{
    [TestFixture]
    public class WhenExecutingFunctions
    {
        private MockRepository _mock;
        private Workflow<string> _pipe;

        [SetUp]
        public void When()
        {
            ServiceLocator<string>.Reset();
            _pipe = new Workflow<string>();
            _mock = new MockRepository();
        }

        [Test]
        public void ShouldInvokeFunction()
        {
            var method = _mock.DynamicMock<Func<string, string>>();

            Expect.Call(method.Invoke("string parameter")).IgnoreArguments().Return("true");
            _mock.ReplayAll();

            _pipe.Do(method);
            _pipe.Start();

            method.VerifyAllExpectations();
        }

        [Test]
        public void ShouldChainResult()
        {
            var method = new Func<string, string>(Method);

            _pipe
                .Do(method)
                .Do(new Func<string, string>((b) => { _result = b += "yellow"; return b; }));

            string result = _pipe.Start();

            Assert.That(result, Is.EqualTo("rainbow: yellow"));
        }

        [Test]
        public void ShouldSeedData()
        {
            _pipe.Do(new Func<string, string>((s) => { _result = s += "red"; return s; }));
            var result = _pipe.Start("Rainbow: ");

            Assert.That(_result, Is.EqualTo("Rainbow: red"), "result");
        }

        [Test]
        public void ShouldNotExecuteIfConstraintEvaluatesFalse()
        {
            var workflow = Workflow<string>.Definition() as IWorkflow<string>;
            Assert.That(workflow, Is.Not.Null);
            workflow.Do(() => false)
                .Do(() => { workflow.Context += "Red"; return true; }, If.Successfull("Failure"));
           
            var result = workflow.Start();
            Assert.That(result, Is.Null);
        }       

        [Test]
        public void ShouldExecutePoliciesForContextBoundFunctions()
        {
            var workflow = new Workflow<string>();
            workflow.Do(() => false);
            var policy = new TestPolicy();
            workflow.RegisteredOperations.Tasks[0].Command.Policies.Add(policy);

            workflow.Start();
            Assert.That(policy.called, Is.GreaterThan(0), "policy wasn't invoked");
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

        public string Method(string param)
        {
            _result = "rainbow: ";
            return _result;
        }
    }
}