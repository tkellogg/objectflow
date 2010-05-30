using NUnit.Framework;
using objectflow.tests.TestDomain;
using objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rhino.Mocks;

namespace objectflow.core.tests
{
    [TestFixture]
    public class WhenExecutingOperation
    {
        private Pipeline<Colour> _flow;
        private PipelineMemoryLoader<Colour> _defaultLoader;

        [SetUp]
        public void BeforeEachTest()
        {
            _defaultLoader = new PipelineMemoryLoader<Colour>(new Colour("Red"));
            _flow = new Pipeline<Colour>();
            _flow.Execute(_defaultLoader);
        }

        [Test]
        public void ShouldNotSetResultBeforeExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            _flow.Execute(_defaultLoader)
                .Execute(doublespace);

            Assert.That(doublespace.SuccessResult, Is.False);
        }

        [Test]
        public void ShouldSetResultAfterExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            var flow = new Pipeline<Colour>();
            flow.Execute(_defaultLoader)
                .Execute(doublespace);
            flow.Start();
            Assert.That(doublespace.SuccessResult, Is.True);
        }


        [Test]
        public void ShouldBeAbleToOverrideDefaultSuccessResult()
        {
            var operation = MockRepository.GeneratePartialMock<DoubleSpace>();
            operation.Stub<DoubleSpace>(op => op.GetSuccessResult()).Return(false);

            _flow.Execute(operation);
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
