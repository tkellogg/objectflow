using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	abstract class AbstractBranchingCondition<T> : IBranchingExpression<T>
		where T : class, IStatefulObject
	{
		protected StatefulWorkflow<T> _workflow;
		private IWorkflow<T> _current;
		private StatefulBuilder<T> _builder;

		public AbstractBranchingCondition(StatefulWorkflow<T> workflow, IWorkflow<T> current, 
			StatefulBuilder<T> builder)
		{
			this._workflow = workflow;
			this._current = current;
			this._builder = builder;
		}

		protected abstract void DoWhenConditionIsTrue(Action<T> callback);

		public IStatefulWorkflow<T> Break()
		{
			DoWhenConditionIsTrue(x => { throw new EarlyExitException(); });

			return _workflow;
		}

		public IStatefulWorkflow<T> BranchTo(Interfaces.IDeclaredOperation location)
		{
			_builder.AnalyzeTransitionPaths(location);
			bool branchConditionPassed = false;
			DoWhenConditionIsTrue(x => 
			{
				_workflow.CheckThatTransitionIsAllowed(x, location);
				branchConditionPassed = true;
			});
			_builder.Current.Do(x => x, Helpers.If.IsTrue(() => branchConditionPassed, location));
			return _workflow;
		}

		public IStatefulWorkflow<T> Do(Action<IStatefulWorkflow<T>> workflowSteps)
		{
			throw new NotImplementedException();
		}
	}
}
