using System;
using System.Diagnostics;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;

namespace objectflow.core.tests.Syntax
{
    [TestFixture]
    public class FunctionalSyntax
    {
        private Pipeline<string> _pipe;

        [SetUp]
        public void BeforeEachTest()
        {
            _pipe = new Pipeline<string>();
            _pipe.Execute(new PipelineMemoryLoader<string>("The colours of the rainbow"));
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
                .Execute(new Func<string, string>(MyMethod))
                .Execute((b) =>
                {
                    b += " rainbow";
                    return (b);
                })
                .Execute(new Func<string, string>(MyMethod));

            _pipe.Start();

        }

        private string MyMethod(string colour)
        {
            colour = "orange";
            return colour;
        }
    }
}
