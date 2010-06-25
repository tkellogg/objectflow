using System;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Engine
{
    internal class OperationInvoker<T> : MethodInvoker<T>
    {
        private readonly BasicOperation<T> _operation;

        public OperationInvoker(BasicOperation<T> operation)
        {
            // TODO: throw exception if null passed in.
            Check.IsNotNull(operation, "operation");
            _operation = operation;
        }

        public override T Execute(T data)
        {
            try
            {
                T result = _operation.Execute(data);
                return result;
            }
            catch (Exception)
            {
                _operation.SetSuccessResult(false);
                return data;
            }
        }
    }
}