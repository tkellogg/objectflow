using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    internal class OperationConstraintPair<T>
    {
        public BasicOperation<T> Operation;
        public CheckConstraint Constraint;
        public long LineNumber;

        public OperationConstraintPair(BasicOperation<T> operation, ICheckContraint constraint)
        {
            Operation = operation;
            Constraint = constraint as CheckConstraint;
        }
    }
}