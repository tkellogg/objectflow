using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Components
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractTransitionGateway
	{
		/// <summary></summary>
		protected object _workflowId;

		/// <summary></summary>
		protected static ITransition[] EMPTY_TRANSITIONS = new ITransition[] { };

		/// <summary>
		/// Creates a new strict gateway using the given workflow ID
		/// </summary>
		public AbstractTransitionGateway(object workflowId)
		{
			_workflowId = workflowId;
		}
		

		/// <summary>
		/// The delegate that, when executed, produces a list of unallowed transitions
		/// </summary>
		public virtual Func<IEnumerable<ITransition>> TransitionList { get; set; }

		/// <summary></summary>
		protected IEnumerable<ITransition> TransitionsInCommonWithList(IList<ITransition> transitions)
		{
			var list = TransitionList();
			list = list ?? EMPTY_TRANSITIONS;

			var result = from a in transitions.AsQueryable()
						 join b in list.AsQueryable() on a.From equals b.From
						 where object.Equals(a.To, b.To) && object.Equals(_workflowId, a.WorkflowId)
						 select a;

			return result;
		}
	}
}
