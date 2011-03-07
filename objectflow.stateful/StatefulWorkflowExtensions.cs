using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Extension methods used to work with stateful workflows
    /// and stateful objects
    /// </summary>
    public static class StatefulWorkflowExtensions
    {
        /// <summary>
        /// Indicates that this object is currently passing through the specific workflow
        /// </summary>
        /// <param name="self"></param>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        public static bool IsAliveInWorkflow(this IStatefulObject self, object workflowId)
        {
            return self.GetStateId(workflowId) != null;
        }

        /// <summary>
        /// Indicates that this object is currently passing through the specific workflow
        /// </summary>
        /// <param name="self"></param>
        /// <param name="workflow"></param>
        /// <returns></returns>
        public static bool IsAliveInWorkflow<T>(this IStatefulObject self, IStatefulWorkflow<T> workflow)
            where T : class, IStatefulObject
        {
            return self.GetStateId(workflow.WorkflowId) != null;
        }
    }
}
