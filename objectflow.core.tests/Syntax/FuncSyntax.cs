using System;
using System.Diagnostics;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.Core.Tests.Syntax
{
    [TestFixture]
    public class FunctionalSyntax
    {
        private Workflow<string> _pipe;

        [SetUp]
        public void BeforeEachTest()
        {
            _pipe = new Workflow<string>();
            _pipe.Do(new WorkflowMemoryLoader<string>("The colours of the rainbow"));
        }

        [Test]
        public void UsingFunctions()
        {
            Func<bool> method = new Func<bool>(() => true);
            bool result = method();
            Debug.WriteLine(result);
            Assert.That(result, Is.True);
        }

        [Test]
        public void FuncPipeline()
        {
            _pipe
                .Do(new Func<string, string>(MyMethod))
                .Do((b) =>
                {
                    b += " rainbow";
                    return b;
                })
                .Do(new Func<string, string>(MyMethod));

            _pipe.Start();
        }

        private string MyMethod(string colour)
        {
            colour = "orange";
            return colour;
        }
    }
}
