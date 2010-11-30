using System;
using System.Diagnostics;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.Core.Tests.Syntax
{
    [TestFixture]
    public class FunctionalSyntax
    {
        private IWorkflow<string> _pipe;

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

        [Test]
        public void FunctionsWithResults()
        {
            _pipe.Do("First", () => true);
            _pipe.Do("Second", ()=>false, If.Successfull("First"));
                      
            _pipe.Start("Richard ");

        }

        private string MyMethod(string colour)
        {
            colour = "orange";
            return colour;
        }

        private bool MyFunction()
        {
            bool y;
            return true;
        }
    }
}
