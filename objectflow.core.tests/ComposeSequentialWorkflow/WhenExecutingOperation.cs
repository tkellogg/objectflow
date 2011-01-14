using Moq;
using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;

namespace Objectflow.core.tests.ComposeSequentialWorkflow
{
    public class WhenExecutingOperation :Specification
    {
        private Workflow<Colour> _flow;

        [Scenario]
        public void Given()
        {
            ServiceLocator<Colour>.Reset();
            _flow = new Workflow<Colour>();
        }

        [Observation]
        public void ShouldNotSetResultBeforeExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            _flow.Do(doublespace);

            Assert.That(doublespace.SuccessResult, Is.False);
        }

        [Observation]
        public void ShouldSetResultAfterExecuting()
        {
            BasicOperation<Colour> doublespace = new DoubleSpace();
            var flow = new Workflow<Colour>();
            flow.Do(doublespace);
            flow.Start(new Colour("Red"));
            
            Assert.That(doublespace.SuccessResult, Is.True);
        }

        [Observation]
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