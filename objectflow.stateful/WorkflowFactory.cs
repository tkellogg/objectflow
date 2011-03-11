using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Describes a workflow process or sub-process that an object can go through. Actual
    /// processes will descend from this class.
    /// </summary>
    public abstract class WorkflowFactory<T> : IWorkflowFactory<T>
        where T : class, IStatefulObject
    {
        private IStatefulWorkflow<T> workflow;

        /// <summary>
        /// This is where you define your workflow
        /// </summary>
        /// <remarks>We'll pass this a DP to record the steps that must happen and proxy
        /// the steps to an actual WorkFlow object. This way we have a full ordering built
        /// up in memory so we can easily jump steps and start part-way through.</remarks>
        protected abstract IStatefulWorkflow<T> Define();

        /// <summary>
        /// Pre-process hook so that you can put global checks on all entry points for this 
        /// workflow. If validation fails, this will silently exit the workflow
        /// </summary>
        /// <param name="object"></param>
        /// <returns><c>false</c> if validation fails</returns>
        protected virtual bool Validate(T @object)
        {
            return true;
        }

        /// <summary>
        /// For resuming a workflow that has already begun or starting a workflow on a new
        /// object.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public virtual T Process(T initializer)
        {
            if (workflow == null)
                workflow = Define();
            if (Validate(initializer))
                return workflow.Start(initializer);
            else return initializer;
        }
    }
}
