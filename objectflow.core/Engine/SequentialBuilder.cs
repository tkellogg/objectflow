using System;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class SequentialBuilder<T> : WorkflowBuilder<T> where T : class
    {
        private readonly Workflow<T> _workflow;

        public SequentialBuilder(Workflow<T> workflow)
        {
            _workflow = workflow;
        }

        public override void AddOperation(IOperation<T> operation)
        {
            var operationPair = new OperationConstraintPair<T>(new OperationInvoker<T>(operation as BasicOperation<T>));
            _workflow.RegisteredOperations.Add(operationPair);
        }

        public override void AddOperation(IOperation<T> operation, ICheckConstraint constraint)
        {
            var operationPair = new OperationConstraintPair<T>(new OperationInvoker<T>(operation as BasicOperation<T>), constraint);
            _workflow.RegisteredOperations.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function)
        {
            var operationPair = new OperationConstraintPair<T>(new FunctionInvoker<T>(function));
            _workflow.RegisteredOperations.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function, ICheckConstraint constraint)
        {
            var operationPair = new OperationConstraintPair<T>(new FunctionInvoker<T>(function), constraint);
            _workflow.RegisteredOperations.Add(operationPair);
        }

        public override void AddOperation(IWorkflow<T> workflow)
        {
            var operationPair = new OperationConstraintPair<T>(new WorkflowInvoker<T>(workflow));
            _workflow.RegisteredOperations.Add(operationPair);
        }

        public override void AddOperation(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            var operationPair = new OperationConstraintPair<T>(new WorkflowInvoker<T>(workflow), constraint);
            _workflow.RegisteredOperations.Add(operationPair);
        }
    }
}