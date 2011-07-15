using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	internal class BranchingExpression<T> : AbstractBranchingCondition<T>
		where T : class, IStatefulObject
	{
		private Predicate<T> _condition;

		public BranchingExpression(StatefulWorkflow<T> workflow, Predicate<T> condition,
					IWorkflow<T> current, StatefulBuilder<T> builder, ITransitionRule<T> transitionRule)
			:base(workflow, current, builder, transitionRule)
		{
			_condition = condition;
		}

		protected override void DoWhenConditionIsTrue(Action<T> callback)
		{
			_workflow.Do(x =>
			{
				if (_condition(x))
					callback(x);
			});
		}

	}
}
