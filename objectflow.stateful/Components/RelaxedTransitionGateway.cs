using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Components
{
	/// <summary>
	/// transition gateway that operates on a blacklist basis. It blocks everything that
	/// it wasn't explicitly disallowed.
	/// </summary>
	public class RelaxedTransitionGateway : AbstractTransitionGateway, ITransitionGateway
	{

		/// <summary>
		/// Creates a new strict gateway using the given workflow ID
		/// </summary>
		public RelaxedTransitionGateway(object workflowId)
			:base(workflowId)
		{
		}

		/// <summary>
		/// allows only what is found in the return value of WhiteList
		/// </summary>
		public IEnumerable<ITransition> AllowTransitions(IList<ITransition> transitions)
		{
			return transitions.Except(base.TransitionsInCommonWithList(transitions))
				.Where(x => object.Equals(x.WorkflowId, _workflowId));
		}
	}
}
