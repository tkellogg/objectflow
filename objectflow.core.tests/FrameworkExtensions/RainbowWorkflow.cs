using System.Diagnostics;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;

namespace Objectflow.core.tests.FrameworkExtensions
{
    public class RainbowWorkflow : AsAWorkflow<Colour>
    {
        public override void Configure(IDefine<Colour> workflow)
        {
            workflow.Do<DuplicateName>();

            Workflow = workflow as IWorkflow<Colour>;
            Debug.Assert(workflow!=null, "Could not configure RainbowWorkflow");
        }
    }
}