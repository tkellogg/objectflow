using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	internal class BranchingExpression<T> : IBranchingExpression<T>
		where T : class, IStatefulObject
	{
		private Predicate<T> condition;
		private IStatefulWorkflow<T> workflow;

		public BranchingExpression(IStatefulWorkflow<T> workflow, Predicate<T> condition)
		{
			this.workflow = workflow;
			this.condition = condition;
		}

		public IStatefulWorkflow<T> Break()
		{
			workflow.When(condition);
			return workflow;
		}

		public IStatefulWorkflow<T> BranchTo(Interfaces.IDeclaredOperation location)
		{
			workflow.When(condition, location);
			return workflow;
		}

	}
}
