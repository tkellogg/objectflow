using System;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal abstract class WorkflowBuilder<T> where T : class
    {
        public ParallelInvoker<T> ParallelOperations;

        public abstract void AddOperation(IOperation<T> operation);

        public abstract void AddOperation(IOperation<T> operation, ICheckContraint constraint);

        public abstract void AddOperation(Func<T, T> function);

        public abstract void AddOperation(Func<T, T> function, ICheckContraint constraint);
    }
}