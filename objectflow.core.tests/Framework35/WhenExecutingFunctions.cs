using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rhino.Mocks;

namespace objectflow.core.tests.Framework35
{
    [TestFixture]
    public class WhenExecutingFunctions
    {
        private MockRepository _mock;
        private Pipeline<string> _pipe;

        [SetUp]
        public void When()
        {
            _pipe = new Pipeline<string>();
            _mock = new MockRepository();
        }

        [Test]
        public void ShouldInvokeFunction()
        {
            var method = _mock.DynamicMock<Func<string, string>>();

            Expect.Call(method.Invoke("string parameter")).IgnoreArguments().Return("true");
            _mock.ReplayAll();

            _pipe.Execute(method);
            _pipe.Start();

            method.VerifyAllExpectations();
        }

        [Test]
        public void ShouldChainResult()
        {
            var method = new Func<string, string>(Method);

            _pipe
                .Execute(method)
                .Execute(new Func<string, string>((b) => { _result = b += "yellow"; return b; }));

            string result = _pipe.Start();

            Assert.That(result, Is.EqualTo("rainbow: yellow"));

        }

        [Test]
        public void ShouldSeedData()
        {
            _pipe.Execute(new Func<string, string>((s) => { _result = s += "red"; return s; }));
            var result = _pipe.Start("Rainbow: ");

            Assert.That(_result, Is.EqualTo("Rainbow: red"), "result");
        }

        private string _result;
        public string Method(string param)
        {
            _result = "rainbow: ";
            return _result;
        }
    }
}
