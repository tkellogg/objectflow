using System;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace Objectflow.core.tests.Testability
{
    public class WhenUnitTestingWorkflow: Specification
    {
        private IWorkflow<Colour> _workflow;

        [Scenario]
        public void Given_a_workflow()
        {
            var workflow = MockRepository.GenerateMock<Workflow<Colour>>();
            _workflow = workflow.Do<DuplicateName>();
        }

        public void When_starting_workflow()
        {
            var result = _workflow.Start();
            Console.WriteLine(result.Name);
        }

        [Observation]
        public void Should_be_able_to_verify_start()
        {
            _workflow.Expect((s) => s.Start()).Return(new Colour("red"));

            When_starting_workflow();
            _workflow.VerifyAllExpectations();
        }
    }
}
    

