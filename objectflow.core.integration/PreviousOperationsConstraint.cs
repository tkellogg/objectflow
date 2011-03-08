using System;
using System.Threading;
using objectflow.core.tests.integration.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.tests.integration
{
    public class PreviousOperationsConstraint: Specification
    {

        private IWorkflow<Colour> _workflow;

        [Observation]
        public void ShouldNotExecuteAfterFailedFunction()
        {
            _workflow = Workflow<Colour>.Definition()
                .Do(s=> { throw new AbandonedMutexException(); })
                .Do<DoubleSpace>(If.Successfull(Step.Previous));
        }

        [Observation]
        public void ShouldExecuteAfterSuccessfulFunction()
        {
            _workflow = Workflow<Colour>.Definition()
                .Do(s =>
                        {
                            s.Name = "blue";
                            return s;
                        })
                .Do<DoubleSpace>(If.Successfull(Step.Previous));
        }
    }
}