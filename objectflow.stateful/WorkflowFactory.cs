using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Rainbow.ObjectFlow.Stateful;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Describes a workflow process or sub-process that an object can go through. Actual
    /// processes will descend from this class.
    /// </summary>
    public abstract class WorkflowFactory<T> : Rainbow.ObjectFlow.Stateful.IWorkflowFactory<T>
        where T : class, IStatefulObject
    {
        private IStatefulWorkflow<T> workflow;

        /// <summary>
        /// This is where you define your workflow
        /// </summary>
        /// <remarks>We'll pass this a DP to record the steps that must happen and proxy
        /// the steps to an actual WorkFlow object. This way we have a full ordering built
        /// up in memory so we can easily jump steps and start part-way through."/></remarks>
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
        public virtual T Start(T initializer)
        {
            InitializeWorkflowIfNecessary();
            if (Validate(initializer))
                return workflow.Start(initializer);
            else return initializer;
        }

        private void InitializeWorkflowIfNecessary()
        {
            if (workflow == null)
                workflow = Define();
        }

        /// <summary>
        /// Calculate whether or not the current role can make the transition from one state
        /// to the next. This looks up in the transition table
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual bool CanDoTransition(object from, object to)
        {
            return true;
        }

        /// <summary>
        /// Allows other applications to query the workflow for transitions that are allowed
        /// and won't be denied. This makes it possible to consolidate all workflow logic
        /// and keep UI separate. 
        /// </summary>
        /// <param name="object">The stateful object that wants to know what it can do.</param>
        /// <returns></returns>
        public virtual IEnumerable<ITransition> GetPossibleTransitions(T @object)
        {
            InitializeWorkflowIfNecessary();
            var empty = new ITransition[0];
            if (@object == null)
                return empty;
            var enumerable = workflow.PossibleTransitions;
            if (enumerable == null)
                return empty;
            else return enumerable.Where(x => 
                object.Equals(x.From, @object.GetStateId(workflow.WorkflowId)));
        }
    }
}
