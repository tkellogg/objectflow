using NUnit.Framework;
using Objectflow.tests.TestDomain;
using Objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rhino.Mocks;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenExecutingOperation
    {
        private Workflow<Colour> _flow;
        private WorkflowMemoryLoader<Colour> _defaultLoader;

        [SetUp]
        public void BeforeEachTest()
        {
            _defaultLoader = new WorkflowMemoryLoader<Colour>(new Colour("Red"));
            _flow = new Workflow<Colour>();
            _flow.Do(_defaultLoader);
        }

        [Test]
        public void ShouldNotSetResultBeforeExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            _flow.Do(_defaultLoader)
                .Do(doublespace);

            Assert.That(doublespace.SuccessResult, Is.False);
        }

        [Test]
        public void ShouldSetResultAfterExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            var flow = new Workflow<Colour>();
            flow.Do(_defaultLoader)
                .Do(doublespace);
            flow.Start();
            Assert.That(doublespace.SuccessResult, Is.True);
        }

        [Test]
        public void ShouldBeAbleToOverrideDefaultSuccessResult()
        {
            var operation = MockRepository.GenerateMock<DoubleSpace>();
            operation.Expect(op => op.GetSuccessResult()).Return(false);

            _flow.Do(operation);
            _flow.Start();

            Assert.That(operation.SuccessResult, Is.False);
        }

        [Test]
        public void DefaultLoaderEvaluatesAsSuccessfulAfterExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();

            Assert.That(_defaultLoader.SuccessResult, Is.EqualTo(false), "Before operation");

            _flow.Start();

            Assert.That(_defaultLoader.SuccessResult, Is.True, "after operation");
        }
    }
}
