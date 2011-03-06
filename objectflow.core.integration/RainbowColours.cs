using System;
using objectflow.core.tests.integration.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;

namespace objectflow.core.tests.integration
{
    public class RainbowColours : AsAWorkflow<Colour> 
    {
        public override void Configure(IDefine<Colour> workflow)
        {
            // Define your workflow here
            var concreteConfigurer = workflow;

            concreteConfigurer
                .Do<DuplicateName>()
                .Do<DoubleSpace>();

            Workflow = concreteConfigurer as IWorkflow<Colour>;
        }
    }
}