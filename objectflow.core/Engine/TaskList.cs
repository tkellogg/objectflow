using System.Collections.Generic;
using System.Collections;
using Rainbow.ObjectFlow.Constraints;

namespace Rainbow.ObjectFlow.Engine
{
	/// <summary>
	/// List of tasks accumulated to be executed in order
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class TaskList<T> 
    {
        internal IList<OperationDuplex<T>> Tasks;

		/// <summary>
		/// 
		/// </summary>
        public TaskList()
        {
            Tasks = new List<OperationDuplex<T>>();
        }

        /// <summary>
        /// Gets a generator over Tasks and adjusts the task path as constraints
        /// fail or pass if there are branch conditions included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable GenerateTaskList()
        {
            IEnumerable tasks = Tasks;
            var enumerator = tasks.GetEnumerator();
            for (; enumerator.MoveNext(); )
            {
                var task = (OperationDuplex<T>)enumerator.Current;
                // TODO: this smells like object incest. Interface maybe??
                if (task.Constraint is BranchCondition)
                {
                    var constraint = (BranchCondition)task.Constraint;
                    var cond = new BranchCondition(constraint, () =>
                    {
                        var enumerable = constraint.BranchPoint.GetRemainingOperations();
                        if (enumerable != null)
                            enumerator = enumerable.GetEnumerator();
                    });
                    task = new OperationDuplex<T>(task.Command, cond);
                }
                yield return task;
            }
        }

		/// <summary>
		/// Set parameters for the first parameterized operation
		/// </summary>
		/// <param name="parameters"></param>
		public void SetParameters(IDictionary<string, object> parameters)
		{
			foreach (var task in Tasks)
			{
				if (task is ParameterizedOperationDuplex<T>)
				{
					var t = (ParameterizedOperationDuplex<T>)task;
					((ParameterizedFunctionInvoker<T>)t.Command).SetParameters(parameters);
					return;
				}
			}
		}
    }
}