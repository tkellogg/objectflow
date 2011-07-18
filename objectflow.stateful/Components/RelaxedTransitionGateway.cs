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
	public class RelaxedTransitionGateway : ITransitionGateway
	{
		/// <summary>
		/// allows only what is found in the return value of WhiteList
		/// </summary>
		public IEnumerable<ITransition> AllowTransitions(IList<ITransition> transitions)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// The delegate that, when executed, produces a list of unallowed transitions
		/// </summary>
		public virtual Func<IEnumerable<ITransition>> BlackList { get; set; }
	}
}
