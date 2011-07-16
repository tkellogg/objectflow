using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	class FailureExpression<T> : IFailureExpression<T>
		where T : class, IStatefulObject
	{
		private AbstractBranchingCondition<T> _condition;

		public FailureExpression(AbstractBranchingCondition<T> condition)
		{
			_condition = condition;
		}

		public void With(string message, params object[] format)
		{
			_condition.Do(wf => 
			{
				string fmt = string.Format(message, format);
				wf.Do(x => { throw new WorkflowActionFailedException(fmt); });
			});
		}

		public void With(Func<T, string> builder)
		{
			_condition.Do(wf =>
			{
				wf.Do(x => { throw new WorkflowActionFailedException(builder(x)); });
			});
		}

		public void With(Func<T, IDictionary<string, object>, string> builder)
		{
			_condition.Do(wf =>
			{
				wf.Do((x, opts) => { throw new WorkflowActionFailedException(builder(x, opts)); });
			});
		}

		public void With<TException>(TException exception) where TException : Exception
		{
			_condition.Do(wf =>
			{
				wf.Do(x => { throw exception; });
			});
		}

		public void With<TException>(Func<T, TException> builder) where TException : Exception
		{
			_condition.Do(wf =>
			{
				wf.Do(x => { throw builder(x); });
			});
		}

		public void With<TException>(Func<T, IDictionary<string, object>, TException> builder) where TException : Exception
		{
			_condition.Do(wf =>
			{
				wf.Do((x, opts) => { throw builder(x, opts); });
			});
		}

	}
}
