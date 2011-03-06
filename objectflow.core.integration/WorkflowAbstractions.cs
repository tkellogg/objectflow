using objectflow.core.tests.integration.TestOperations;
using Rainbow.ObjectFlow.Framework;

namespace objectflow.core.tests.integration
{    
    public class WorkflowAbstractions : Specification
    {
        [Observation]
        public void WorkflowContainerClass()
        {
            var rainbowWorkflow = new RainbowColours();
            rainbowWorkflow.Configure(Workflow<Colour>.Definition());
            var result = rainbowWorkflow.Start(new Colour("Red"));

            result.Name.ShouldBe("R e d R e d");
        }
    }
}