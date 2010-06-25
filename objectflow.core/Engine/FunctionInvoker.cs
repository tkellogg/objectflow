using System;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Engine
{
    internal class FunctionInvoker<T> : MethodInvoker<T>
    {
        private readonly Func<T, T> _function;

        public FunctionInvoker(Func<T, T> function)
        {
            Check.IsNotNull(function, function.Method.Name);
            _function = function;
        }

        public override T Execute(T data)
        {
            return _function.Invoke(data);
        }
    }
}