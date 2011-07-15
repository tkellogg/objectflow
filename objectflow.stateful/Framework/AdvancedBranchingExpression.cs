using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	/// <summary>
	/// Branching expression that uses parameters
	/// </summary>
	class AdvancedBranchingExpression<T> : AbstractBranchingCondition<T>
		where T : class, IStatefulObject
	{
		private Func<T, IDictionary<string, object>, bool> _condition;

		public AdvancedBranchingExpression(StatefulWorkflow<T> workflow, 
			Func<T, IDictionary<string, object>, bool> condition, IWorkflow<T> current, StatefulBuilder<T> builder,
			ITransitionRule<T> transitionRule)
			:base(workflow, current, builder, transitionRule)
		{
			_condition = condition;
		}

		protected override void DoWhenConditionIsTrue(Action<T> callback)
		{
			_workflow.Do((x, opts) =>
			{
				if (_condition(x, opts))
					callback(x);
			});
		}
	}
}
