using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class OperationConstraintPair<T>
    {
        private readonly CheckConstraint _constraint;
        private readonly MethodInvoker<T> _command;

        public OperationConstraintPair(MethodInvoker<T> command, ICheckContraint constraint)
        {
            _command = command;
            _constraint = constraint as CheckConstraint;
        }

        public OperationConstraintPair(MethodInvoker<T> command)
        {
            _command = command;
            _constraint = null;
        }

        public CheckConstraint Constraint
        {
            get { return _constraint; }
        }

        public MethodInvoker<T> Command
        {
            get { return _command; }
        }
    }
}