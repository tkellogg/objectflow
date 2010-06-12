using NUnit.Framework;
using Objectflow.tests.TestDomain;
using Objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace Objectflow.tests.Syntax
{
    [TestFixture]
    public class WorkFlowSyntax
    {
        private Workflow<Colour> _pipe;
        private IOperation<Colour> _doublespace = new DoubleSpace();
        private IOperation<Colour> _doublespaceOne = new DoubleSpace();
        private IOperation<Colour> _doubleSpaceTwo = new DoubleSpace();
        private Colour _red;

        [SetUp]
        public void BeforeEachTest()
        {
            _red = new Colour("Red");
            _pipe = new Workflow<Colour>();
            _pipe.Do(new WorkflowMemoryLoader<Colour>(_red));
            _doublespace = new DoubleSpace();
            _doublespaceOne = new DoubleSpace();
            _doubleSpaceTwo = new DoubleSpace();
        }

        [Test]
        public void SingleOperationSyntax()
        {
            _pipe.Do(_doublespace);

            var result = WhenT();

            Assert.That(result, Is.Not.Null, "null");
            Assert.That(result.Name, Is.EqualTo("R e d"), "wrong value");
        }

        [Test]
        public void MultipleOperationSyntax()
        {
            _pipe.Do(_doublespaceOne).Do(_doubleSpaceTwo);

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("R   e   d"));
        }

        [Test]
        public void SimpleBooleanConstraintSyntax()
        {
            _pipe.Do(_doublespaceOne, If.IsTrue(false));

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Red"));
        }

        [Test]
        public void SimpleBooleanConstraintNegationSyntax()
        {
            _pipe.Do(_doublespaceOne, If.Not.IsTrue(false));

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("R e d"));
        }

        [Test]
        public void DecisionConstraintSyntax()
        {
            _pipe
                .Do(_doublespaceOne)
                .Do(_doubleSpaceTwo, If.Successful(_doublespaceOne));

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("R   e   d"));
        }

        [Test]
        public void DecisionConstraintNegationSyntax()
        {
            var failedOperation = MockRepository.GenerateStub<BasicOperation<Colour>>();
            failedOperation.Stub<BasicOperation<Colour>>(
                bo => bo.Execute(null)).IgnoreArguments().Return(_red);
            failedOperation.SetSuccessResult(false);

            _pipe
                .Do(failedOperation)
                .Do(_doubleSpaceTwo, If.Not.Successfull(_doublespaceOne));

            var result = WhenT();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("R e d"));
        }

        [Test]
        public void LamdaDecisionSyntax()
        {
            string z = "red";

            _pipe
                .Do(_doublespaceOne)
                .Do(_doubleSpaceTwo, If.IsTrue(() => z.Contains("blue")));

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("R e d"));
        }

        [Test]
        public void GenericPipelineSyntax()
        {
            var pipe = new Workflow<Colour>();
            pipe.Do(new WorkflowMemoryLoader<Colour>(_red))
                .Do(_doublespace);

            var result = pipe.Start();

            Assert.That(result.ToString(), Is.EqualTo("R e d"));
        }

        private Colour WhenT()
        {
            var result = _pipe.Start();
            return result;
        }
    }
}