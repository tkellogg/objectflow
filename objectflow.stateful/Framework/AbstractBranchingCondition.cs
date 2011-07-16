using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	abstract class AbstractBranchingCondition<T> : IBranchingExpression<T>
		where T : class, IStatefulObject
	{
		protected StatefulWorkflow<T> _workflow;
		private IWorkflow<T> _current;
		private StatefulBuilder<T> _builder;
		private ITransitionRule<T> _transitionRule;

		public AbstractBranchingCondition(StatefulWorkflow<T> workflow, IWorkflow<T> current, 
			StatefulBuilder<T> builder, ITransitionRule<T> transitionRule)
		{
			this._workflow = workflow;
			this._current = current;
			this._builder = builder;
			this._transitionRule = transitionRule;
		}

		protected abstract void DoWhenConditionIsTrue(Action<T> callback);

		public IStatefulWorkflow<T> Break()
		{
			DoWhenConditionIsTrue(x => { throw new EarlyExitException(); });

			return _workflow;
		}

		public IStatefulWorkflow<T> BreakWithStatus(object status)
		{
			_builder.AddBreakWithStatus(status);
			DoWhenConditionIsTrue(x =>
			{
				_workflow.CheckThatTransitionIsAllowed(x.GetStateId(_workflow.WorkflowId), status);
				_transitionRule.Transition(x, status);
				throw new EarlyExitException();
			});
			return _workflow;
		}

		public IStatefulWorkflow<T> BranchTo(Interfaces.IDeclaredOperation location)
		{
			return BranchTo(location, true);
		}

		private IStatefulWorkflow<T> BranchTo(Interfaces.IDeclaredOperation location, bool whenTrue)
		{
			_builder.AnalyzeTransitionPaths(location);
			bool branchConditionPassed = false;
			DoWhenConditionIsTrue(x => 
			{
				_workflow.CheckThatTransitionIsAllowed(x, location);
				branchConditionPassed = true;
			});

			if (whenTrue)
				_builder.Current.Do(x => x, Helpers.If.IsTrue(() => !branchConditionPassed, location));
			else
				_builder.Current.Do(x => x, Helpers.If.IsTrue(() => branchConditionPassed, location));
			
			return _workflow;
		}

		/**
		 * TODO: Consider adding checks for branch points and yields inside these inner
		 * code blocks to make sure not anything can branch inside. Probably the condition
		 * has to be true for anything branching inside
		 */

		public IStatefulWorkflow<T> Do(Action<IStatefulWorkflow<T>> innerCodeBlock)
		{
			var branchPoint = Declare.Step();
			BranchTo(branchPoint, false);
			innerCodeBlock(_workflow);
			_workflow.Define(branchPoint);
			return _workflow;
		}

	}
}
