using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// A default transition gateway implementation that only allows a certain 
	/// set of transitions
	/// </summary>
	public class DefaultTransitionGateway : ITransitionGateway
	{
		
			private IList<ITransition> allowedTransitions;

			/// <param name="allowedTransitions">a list of allowed transitions</param>
			public DefaultTransitionGateway(IList<ITransition> allowedTransitions)
			{
				this.allowedTransitions = allowedTransitions;
			}

			/// <inheritdoc />
			public IEnumerable<ITransition> AllowTransitions(IList<ITransition> transitions)
			{
				return from t in transitions.AsQueryable()
					   join a in this.allowedTransitions.AsQueryable()
						on t.From equals a.From
					   select t;
			}

	}
}
