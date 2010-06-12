using System;
using Rainbow.ObjectFlow.Engine;

namespace Rainbow.ObjectFlow.Framework
{
    internal class FunctionInvoker<T> : MethodInvoker<T>
    {
        private readonly Func<T, T> _function;

        public FunctionInvoker(Func<T, T> function)
        {
            // TODO: throw exception if null passed in.
            Check.IsNotNull(function, function.Method.Name);
            _function = function;
        }

        public override T Execute(T data)
        {
            return _function.Invoke(data);
        }
    }
}