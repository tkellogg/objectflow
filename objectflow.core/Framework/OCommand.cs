
namespace Rainbow.ObjectFlow.Framework
{
    internal class OCommand<T> : Command<T>
    {
        private readonly BasicOperation<T> _operation;

        public OCommand(BasicOperation<T> operation)
        {
            Check.IsNotNull(operation, "operation");
            _operation = operation;
        }

        public override T Execute(T data)
        {
            return _operation.Execute(data);
        }
    }
}