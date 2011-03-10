using System;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal abstract class WorkflowBuilder<T> where T : class
    {
        protected readonly TaskList<T> _taskList;

        public ParallelInvoker<T> ParallelOperations;

        public TaskList<T> TaskList { get { return _taskList; } }

        public WorkflowBuilder(TaskList<T> taskList)
        {
            _taskList = taskList;
        }

        public abstract void AddOperation(IOperation<T> operation);

        public abstract void AddOperation(IOperation<T> operation, ICheckConstraint constraint);

        public abstract void AddOperation(Func<T, T> function);

        public abstract void AddOperation(Func<T, T> function, IDeclaredOperation name);

        public abstract void AddOperation(Func<T, T> function, ICheckConstraint constraint);

        public abstract void AddOperation(Func<T, T> function, ICheckConstraint constraint, IDeclaredOperation name);

        public abstract void AddOperation(IWorkflow<T> operation);

        public abstract void AddOperation(IWorkflow<T> workflow, ICheckConstraint constraint);

        public abstract void AddOperation<TOperation>();

        public abstract void AddOperation<TOperation>(ICheckConstraint constraint);
    }
}