using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	internal class BranchingExpression<T> : IBranchingExpression<T>
		where T : class, IStatefulObject
	{
		private Predicate<T> condition;
		private IStatefulWorkflow<T> workflow;
		private Func<T, IDictionary<string, object>, bool> advancedCondition;

		public BranchingExpression(IStatefulWorkflow<T> workflow, Predicate<T> condition)
		{
			this.workflow = workflow;
			this.condition = condition;
		}

		public BranchingExpression(IStatefulWorkflow<T> workflow, Func<T, IDictionary<string, object>, bool> condition)
		{
			this.workflow = workflow;
			this.advancedCondition = condition;
		}

		public IStatefulWorkflow<T> Break()
		{
			if (condition == null)
				workflow.Unless(condition);
			else
				workflow.Do((x, dict) => 
				{
					if (advancedCondition(x, dict))
						throw new EarlyExitException();
				});

			return workflow;
		}

		public IStatefulWorkflow<T> BranchTo(Interfaces.IDeclaredOperation location)
		{
			if (condition != null)
				workflow.Unless(condition, location);
			else
				workflow.Unless(advancedCondition, location);

			return workflow;
		}

	}
}
