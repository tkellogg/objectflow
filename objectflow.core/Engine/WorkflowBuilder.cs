using System;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
	/// <summary>
	/// Assembles a chain of operations
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public abstract class WorkflowBuilder<T> where T : class
    {
		/// <summary>
		/// 
		/// </summary>
        internal protected readonly TaskList<T> taskList;

        internal ParallelInvoker<T> ParallelOperations;

		/// <summary>
		/// tasks to be executed
		/// </summary>
        public TaskList<T> Tasks { get { return taskList; } }

        internal WorkflowBuilder(TaskList<T> taskList)
        {
            this.taskList = taskList;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operation"></param>
        public abstract void AddOperation(IOperation<T> operation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="constraint"></param>
        public abstract void AddOperation(IOperation<T> operation, ICheckConstraint constraint);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <param name="function"></param>
		public abstract void AddOperation<T1, T2, T3>(Action<T, T1, T2, T3> function);
		
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="function"></param>
		public abstract void AddOperation<T1, T2>(Action<T, T1, T2> function);
		
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="function"></param>
		public abstract void AddOperation<T1>(Action<T, T1> function);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		public abstract void AddOperation(Func<T, T> function);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		/// <param name="name"></param>
        public abstract void AddOperation(Func<T, T> function, IDeclaredOperation name);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		/// <param name="constraint"></param>
        public abstract void AddOperation(Func<T, T> function, ICheckConstraint constraint);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		/// <param name="constraint"></param>
		/// <param name="name"></param>
        public abstract void AddOperation(Func<T, T> function, ICheckConstraint constraint, IDeclaredOperation name);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operation"></param>
        public abstract void AddOperation(IWorkflow<T> operation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workflow"></param>
		/// <param name="constraint"></param>
        public abstract void AddOperation(IWorkflow<T> workflow, ICheckConstraint constraint);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TOperation"></typeparam>
        public abstract void AddOperation<TOperation>();

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TOperation"></typeparam>
		/// <param name="constraint"></param>
        public abstract void AddOperation<TOperation>(ICheckConstraint constraint);
    }
}