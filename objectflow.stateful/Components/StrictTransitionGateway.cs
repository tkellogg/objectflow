using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Components
{
	/// <summary>
	/// transition gateway that operates on a whitelist basis. It blocks everything that
	/// it wasn't explicitly allowed.
	/// </summary>
	public class StrictTransitionGateway : AbstractTransitionGateway, ITransitionGateway
	{

		/// <summary>
		/// Creates a new strict gateway using the given workflow ID
		/// </summary>
		public StrictTransitionGateway(object workflowId)
			:base(workflowId)
		{
		}

		/// <summary>
		/// allows only what is found in the return value of WhiteList
		/// </summary>
		public virtual IEnumerable<ITransition> AllowTransitions(IList<ITransition> transitions)
		{
			return base.TransitionsInCommonWithList(transitions);
		}
	}
}
