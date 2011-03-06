using System;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.FrameworkExtensions
{
    public class WhenStartingWorkflowContainer : Specification
    {
        private RainbowWorkflow _workflow;

        [Scenario]
        public void Given_a_workflow_container()
        {
            _workflow = new RainbowWorkflow();
        }

        [Observation]
        public void Should_Ensure_Operations_Have_Been_Configured()
        {
            _workflow.ShouldThrow<ArgumentNullException>((() => _workflow.Start()));
        }

        [Observation]
        public void Should_ensure_seeded_start_has_been_configured()
        {
            _workflow.ShouldThrow<ArgumentNullException>((() => _workflow.Start(new Colour("Red"))));
        }

        [Observation]
        public void Should_check_data_is_not_null()
        {
            _workflow.Configure(Workflow<Colour>.Definition());
            _workflow.ShouldThrow<ArgumentNullException>((() => _workflow.Start(null)));
        }
    }
}