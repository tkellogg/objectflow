using System;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Engine
{
    internal class OperationInvoker<T> : MethodInvoker<T>
    {
        private readonly BasicOperation<T> _operation;
        
        public BasicOperation<T> Operation 
        {
            get
            {
                return _operation;
            }
        }

        public OperationInvoker(BasicOperation<T> operation)
        {
            Check.IsNotNull(operation, "operation");
            _operation = operation;
        }

        public override T Execute(T data)
        {
            T result = data;

            try
            {
                 result = _operation.Execute(data);
            }
            catch (Exception)
            {
                _operation.SetSuccessResult(false);
            }

            return result;
        }
   }
}