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
	public class StrictTransitionGateway : ITransitionGateway
	{
		/// <summary>
		/// allows only what is found in the return value of WhiteList
		/// </summary>
		public virtual IEnumerable<ITransition> AllowTransitions(IList<ITransition> transitions)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// The delegate that, when executed, produces a list of allowed transitions
		/// </summary>
		public virtual Func<IEnumerable<ITransition>> WhiteList { get; set; }
	}
}
