using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Engine
{
    internal static class WfExecutionPlan
    {
        internal static IDictionary<int, bool> _callStack;

        static WfExecutionPlan()
        {
            _callStack = new Dictionary<int, bool>();
        }

        public static IDictionary<int, bool> CallStack
        {
            get { return _callStack; }
            set { _callStack = value; }
        }
    }
}
