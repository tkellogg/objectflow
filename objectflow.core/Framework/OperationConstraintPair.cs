using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    internal class OperationConstraintPair<T>
    {
        private CheckConstraint _constraint;
        private Command<T> _command;

        public OperationConstraintPair(Command<T> command, ICheckContraint constraint)
        {
            _command = command;
            _constraint = constraint as CheckConstraint;
        }

        public OperationConstraintPair(Command<T> command)
        {
            _command = command;
            _constraint = null;
        }

        public CheckConstraint Constraint
        {
            get { return _constraint; }
        }

        public Command<T> Command
        {
            get { return _command; }
        }
    }
}