using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace Objectflow.core.tests.Syntax
{
    [TestFixture, Explicit]
    public class SimpleConcurrency
    {
        private Workflow<string> _workflow;

        [SetUp]
        public void BeforeEachTest()
        {
            _workflow = new Workflow<string>();
        }

        [Test]
        public void ParallelOperations()
        {
            _workflow
                .Do((b) => "a1", If.IsTrue(true))
                .And.Do((b) => "a2", If.IsTrue(true))
                .And.Do((b) => "a3", If.IsTrue(true))
                .Then().Do((b) => "b")
                .Then().Do((b) => "c");

            _workflow.Start();
        }

        [Test]
        public void ParallelOperationsShorthand()
        {
            _workflow
                .Do((b) => "a1", If.IsTrue(true))
                .And.Do((b) => "a2", If.IsTrue(true))
                .Do((b) => "a3", If.IsTrue(true))
                .Then().Do((b) => "b")
                .Do((b) => "c");

            _workflow.Start();
        }

        [Test]
        public void ConcurrencyWithStaticCreation()
        {
            Workflow<string>.Definition()
                .Do(a => { Console.WriteLine("t1"); return a; })
                .And.Do(b => { Console.WriteLine("T2"); return b; })
                .And.Do(c => { Console.WriteLine("T3"); return c; })
                .Then().Do(d => { Console.WriteLine("Finished"); return d; })
                .Start();
        }
    }
}
