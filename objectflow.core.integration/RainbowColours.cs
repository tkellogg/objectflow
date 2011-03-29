using System;
using objectflow.core.tests.integration.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;

namespace objectflow.core.tests.integration
{
    public class RainbowColours : AsAWorkflow<Colour> 
    {
        public RainbowColours(IDefine<Colour> workflow) : base(workflow)
        {
        }

        [Obsolete("Use Configure() to configure the method and pass the definer into the constructor")]
        public override void Configure(IDefine<Colour> workflow)
        {
        }

        public override IWorkflow<Colour> Configure()
        {
            return Configurer
                .Do<DuplicateName>()
                .Do<DoubleSpace>();
        }
    }
}