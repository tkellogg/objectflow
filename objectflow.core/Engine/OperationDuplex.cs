using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class OperationDuplex<T>
    {
        private readonly ICheckConstraint _constraint;
        private readonly MethodInvoker<T> _command;

        public OperationDuplex(MethodInvoker<T> command, ICheckConstraint constraint)
        {
            _command = command;
            _constraint = constraint;
        }

        public OperationDuplex(MethodInvoker<T> command)
        {
            _command = command;
            _constraint = null;
        }

        public ICheckConstraint Constraint
        {
            get { return _constraint; }
        }

        public MethodInvoker<T> Command
        {
            get { return _command; }
        }
    }
}