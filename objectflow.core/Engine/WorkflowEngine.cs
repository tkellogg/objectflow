using System.Collections;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class WorkflowEngine<T>
    {
        internal static Stack<ICheckConstraint> CallStack { get; set; }

        static WorkflowEngine()
        {
            CallStack = new Stack<ICheckConstraint>();
        }

        public virtual T Execute(IEnumerable operations)
        {
            return Execute(operations, default(T));
        }

        public virtual T Execute(IEnumerable operations, T data)
        {
            T current = data;

            foreach (OperationConstraintPair<T> operationPair in operations)
            {
                current = Execute(operationPair, current);
            }

            return current;
        }

        public virtual T Execute(OperationConstraintPair<T> operationPair)
        {
            return Execute(operationPair, default(T));
        }

        public virtual T Execute(OperationConstraintPair<T> operationPair, T current)
        {
            if (ConstraintResult(operationPair.Constraint as Condition))
            {
                current = operationPair.Command.Execute(current);
            }

            return current;
        }

        private static bool ConstraintResult(ICheckConstraint constraint)
        {
            return constraint == null || constraint.Matches();
        }
    }
}