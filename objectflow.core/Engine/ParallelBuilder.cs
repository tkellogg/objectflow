using System;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class ParallelSplitBuilder<T> : WorkflowBuilder<T> where T : class
    {
        public ParallelSplitBuilder(TaskList<T> taskList) : base(taskList)
        {
            ParallelOperations = new ParallelInvoker<T>();

            if (null != _taskList.Tasks && 0 != _taskList.Tasks.Count)
            {
                OperationDuplex<T> leftOver =
                    _taskList.Tasks[_taskList.Tasks.Count - 1];
                _taskList.Tasks.RemoveAt(_taskList.Tasks.Count - 1);
                ParallelOperations.Add(leftOver);
            }

        }

        public override void AddOperation(IOperation<T> operation)
        {
            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation as BasicOperation<T>));
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(IOperation<T> operation, ICheckConstraint constraint)
        {
            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation as BasicOperation<T>), constraint);
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function)
        {
            var operationPair = new OperationDuplex<T>(new FunctionInvoker<T>(function));
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function, IDeclaredOperation name)
        {
            throw new Exception();
        }

        public override void AddOperation(Func<T, T> function, ICheckConstraint constraint)
        {
            var operationPair = new OperationDuplex<T>(new FunctionInvoker<T>(function), constraint);
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(IWorkflow<T> workflow)
        {
            var operationPair = new OperationDuplex<T>(new WorkflowInvoker<T>(workflow));
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            var operationPair = new OperationDuplex<T>(new WorkflowInvoker<T>(workflow), constraint);
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation<TOperation>()
        {
            var operation = Activator.CreateInstance<TOperation>();
            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation as BasicOperation<T>));
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation<TOperation>(ICheckConstraint constraint)
        {
            var operation = Activator.CreateInstance<TOperation>() as BasicOperation<T>;
            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation), constraint);
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function, ICheckConstraint constraint, IDeclaredOperation name)
        {
            throw new NotImplementedException();
        }
    }
}