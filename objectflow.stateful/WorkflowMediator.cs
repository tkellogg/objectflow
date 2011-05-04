using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Stateful;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Describes a workflow process or sub-process that an object can go through. Actual
    /// processes will descend from this class.
    /// </summary>
    public abstract class WorkflowMediator<T> : Rainbow.ObjectFlow.Stateful.IWorkflowMediator<T>
        where T : class, IStatefulObject
    {
        /// <summary>
        /// A reference to the workflow. Call <see cref="InitializeWorkflowIfNecessary"/> if this is null
        /// </summary>
        protected IStatefulWorkflow<T> _workflow;

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
		/// <param name="parameters">Optional parameters for this workflow segment</param>
		/// <returns></returns>
		public virtual T Start(T initializer, params object[] parameters)
		{
			InitializeWorkflowIfNecessary();
			var beginning = initializer.GetStateId(_workflow.WorkflowId);

			T ret = initializer;
			if (Validate(initializer))
			{
				if (parameters.Length == 0)
					ret = _workflow.Start(initializer);
				else
					ret = _workflow.StartWithParams(initializer, parameters);
			}

			object ending = (ret != null)? ret.GetStateId(_workflow.WorkflowId) : null;
			OnFinished(ret, beginning, ending);

			return ret;
		}

		/// <summary>
		/// Called when a segment finishes executing. This is where you should implement
		/// persistence.
		/// </summary>
		/// <param name="subject">The object exiting the workflow segment</param>
		/// <param name="from">beginning workflow state</param>
		/// <param name="to">ending workflow state</param>
		protected virtual void OnFinished(T subject, object from, object to)
		{
		}

        /// <summary>
        /// Lazy initializes the workflow. Always call this before you need to reference <see cref="_workflow"/>
        /// </summary>
        protected virtual void InitializeWorkflowIfNecessary()
        {
            if (_workflow == null)
                _workflow = Define();
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
			else
				return GetPossibleTransitions(@object.GetStateId(_workflow.WorkflowId));
        }

		/// <summary>
		/// Allows other applications to query the workflow for transitions that are allowed
		/// and won't be denied. This makes it possible to consolidate all workflow logic
		/// and keep UI separate. 
		/// </summary>
		/// <param name="fromState">state that the object of interest is currently at</param>
		public virtual IEnumerable<ITransition> GetPossibleTransitions(object fromState)
		{
			InitializeWorkflowIfNecessary();
			var empty = new ITransition[0];
			var enumerable = _workflow.PossibleTransitions;
			if (enumerable == null)
				return empty;
			else return enumerable.Where(x =>
				object.Equals(x.From, fromState));
		}

        /// <summary>
        /// This method indicates that the user wants @object to transition into the state marked
        /// by <code>toHint</code>. The implementation should take the necessary steps to direct
        /// the object through the correct workflow steps. 
        /// </summary>
        /// <param name="object"></param>
        /// <param name="toHint"></param>
        public void TransitionTo(T @object, object toHint)
        {
            InitializeWorkflowIfNecessary();
            InitializeHintsIfNecessary();
            var from = CoerceNull(@object.GetStateId(_workflow.WorkflowId));
            toHint = CoerceNull(toHint);
            if (hints.ContainsKey(from) && hints[from].ContainsKey(toHint)
                    && hints[from][toHint] != null)
                hints[from][toHint](@object);
            Start(@object);
        }

        private void InitializeHintsIfNecessary()
        {
            if (hints == null)
            {
                hints = new Dictionary<object, Dictionary<object, Action<T>>>();
                DefineHints();
            }
        }

        private const string NULL_VALUE_EXPR = "Rainbow.ObjectFlow.Stateful.NullValueExpression";
        private object CoerceNull(object value)
        {
            return value == null ? NULL_VALUE_EXPR : value;
        }

        private Dictionary<object, Dictionary<object, Action<T>>> hints;

        /// <summary>
        /// Define all hints here using SetHint and SetHints 
        /// </summary>
        protected virtual void DefineHints() { }

        /// <summary>
        /// Setup a hint path so that when an object with state <c>from</c> requests to go into
        /// state <c>to</c>, we can use the provided action (<c>fn</c>) to put the object into
        /// a state consistent with the requested path.
        /// </summary>
        /// <param name="from">The starting point of the path</param>
        /// <param name="to">The desired outcome</param>
        /// <param name="fn">A block of code that sets the necessary properties to put the object
        /// in a state where it will follow the requested path.</param>
        protected void SetHint(object from, object to, Action<T> fn)
        {
            if (hints == null)
                throw new InvalidOperationException("SetHint should not be called from within Define(), instead "
                    + "override DefineHints() and setup your hints there");
            if (!hints.ContainsKey(CoerceNull(from)))
                hints[CoerceNull(from)] = new Dictionary<object, Action<T>>();
            hints[CoerceNull(from)][CoerceNull(to)] = fn;
        }

        /// <summary>
        /// Setup a hint path so that when an object with state <c>from</c> requests to go into
        /// state <c>to</c>, we can use the provided action (<c>fn</c>) to put the object into
        /// a state consistent with the requested path.
        /// </summary>
        /// <param name="from">The starting point of the path</param>
        /// <param name="to">A list of the desired outcomes</param>
        /// <param name="fn">A block of code that sets the necessary properties to put the object
        /// in a state where it will follow the requested path.</param>
        protected void SetHints(object from, IEnumerable<object> to, Action<T> fn)
        {
            foreach (var item in to)
                SetHint(from, item, fn);
        }

        /// <summary>
        /// Setup a hint path so that when an object with state <c>from</c> requests to go into
        /// state <c>to</c>, we can use the provided action (<c>fn</c>) to put the object into
        /// a state consistent with the requested path.
        /// </summary>
        /// <param name="from">A list of the starting points of the path</param>
        /// <param name="to">The desired outcome</param>
        /// <param name="fn">A block of code that sets the necessary properties to put the object
        /// in a state where it will follow the requested path.</param>
        protected void SetHints(IEnumerable<object> from, object to, Action<T> fn)
        {
            foreach (var item in from)
                SetHint(item, to, fn);
        }
    }
}
