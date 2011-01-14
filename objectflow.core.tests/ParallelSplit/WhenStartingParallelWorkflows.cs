using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.ParallelSplit
{
    public class WhenStartingParallelWorkflows:Specification
    {
        private Mock<Workflow<string>> _workflow;

        [Scenario]
        public void Given()
        {
            ServiceLocator<string>.Reset();
            _workflow = new Mock<Workflow<string>>();
        }

        [Observation]
        public void ShouldImplicitlyCloseParallelDefinitionOnStart()
        {
            _workflow.Setup(m => m.Then());

            _workflow.Object.Do((a) => "red").And.Do((b) => "orange");
            _workflow.Object.Start();
            _workflow.VerifyAll();
        }
    }
}