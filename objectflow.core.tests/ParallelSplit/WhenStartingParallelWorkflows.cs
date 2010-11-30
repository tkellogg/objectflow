using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.ParallelSplit
{
    [TestFixture]
    public class WhenStartingParallelWorkflows
    {
        private Mock<Workflow<string>> _workflow;

        [SetUp]
        public void Given()
        {
            ServiceLocator<string>.Reset();
            _workflow = new Mock<Workflow<string>>();
        }

        [Test]
        public void ShouldImplicitlyCloseParallelDefinitionOnStart()
        {
            _workflow.Setup(m => m.Then());

            _workflow.Object.Do((a) => "red").And.Do((b) => "orange");
            _workflow.Object.Start();
            _workflow.VerifyAll();
        }
    }
}