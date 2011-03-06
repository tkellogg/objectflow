using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Engine
{
    internal static class WfExecutionPlan
    {
        static WfExecutionPlan()
        {
            CallStack = new List<bool>();
        }

        public static IList<bool> CallStack { get; set; }
    }
}
