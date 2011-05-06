using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// Default ITransitionRule implementation that treats everything not currently 
	/// in a workflow with a null state Id. Descend from this class if all you want 
	/// to change is how objects are handled when they're not in a workflow.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DefaultTransitionRule<T> : Rainbow.ObjectFlow.Stateful.ITransitionRule<T>
        where T : class, IStatefulObject
	{
		private object _workflowId;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workflowId"></param>
		public DefaultTransitionRule(object workflowId)
		{
			_workflowId = workflowId;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="endState"></param>
		public virtual void Begin(T entity, object endState)
		{
			entity.SetStateId(_workflowId, endState);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		public virtual void End(T entity)
		{
			entity.SetStateId(_workflowId, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="endState"></param>
		public virtual void Transition(T entity, object endState)
		{
			entity.SetStateId(_workflowId, endState);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual bool IsInWorkflow(T entity)
		{
			return entity.GetStateId(_workflowId) != null;
		}

		/// <summary>
		/// Returns null unless <see cref="IsInWorkflow"/> is true.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual bool? HasBeenInWorkflow(T entity)
		{
			return IsInWorkflow(entity) ? true as bool? : null;
		}
	}
}
