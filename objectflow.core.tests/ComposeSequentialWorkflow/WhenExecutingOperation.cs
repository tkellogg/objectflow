using Moq;
using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;

namespace Objectflow.core.tests.ComposeSequentialWorkflow
{
    [TestFixture]
    public class WhenExecutingOperation
    {
        private Workflow<Colour> _flow;

        [SetUp]
        public void Given()
        {
            ServiceLocator<Colour>.Reset();
            _flow = new Workflow<Colour>();
        }

        [Test]
        public void ShouldNotSetResultBeforeExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            _flow.Do(doublespace);

            Assert.That(doublespace.SuccessResult, Is.False);
        }

        [Test]
        public void ShouldSetResultAfterExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            var flow = new Workflow<Colour>();
            flow.Do(doublespace);
            flow.Start(new Colour("Red"));
            
            Assert.That(doublespace.SuccessResult, Is.True);
        }

        [Test]
        public void ShouldBeAbleToOverrideDefaultSuccessResult()
        {
            var operation = new Mock<DoubleSpace>();
            operation.Setup(op => op.GetSuccessResult()).Returns(false);

            _flow.Do(operation.Object);
            _flow.Start();

            Assert.That(operation.Object.SuccessResult, Is.False);
        }
    }
}