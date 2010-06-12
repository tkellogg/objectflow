using System;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class ParallelSplitBuilder<T> : WorkflowBuilder<T> where T : class
    {
        private readonly Workflow<T> _workflow;

        public ParallelSplitBuilder(Workflow<T> workflow)
        {
            ParallelOperations = new ParallelInvoker<T>();
            _workflow = workflow;

            if (_workflow.RegisteredOperations != null && _workflow.RegisteredOperations.Count != 0)
            {
                OperationConstraintPair<T> leftOver =
                    _workflow.RegisteredOperations[_workflow.RegisteredOperations.Count - 1];
                _workflow.RegisteredOperations.RemoveAt(_workflow.RegisteredOperations.Count - 1);
                ParallelOperations.Add(leftOver);
            }
        }

        public override void AddOperation(IOperation<T> operation)
        {
            var operationPair = new OperationConstraintPair<T>(new OperationInvoker<T>(operation as BasicOperation<T>));
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(IOperation<T> operation, ICheckContraint constraint)
        {
            var operationPair = new OperationConstraintPair<T>(new OperationInvoker<T>(operation as BasicOperation<T>), constraint);
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function)
        {
            var operationPair = new OperationConstraintPair<T>(new FunctionInvoker<T>(function));
            ParallelOperations.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function, ICheckContraint constraint)
        {
            var operationPair = new OperationConstraintPair<T>(new FunctionInvoker<T>(function), constraint);
            ParallelOperations.Add(operationPair);
        }
    }
}