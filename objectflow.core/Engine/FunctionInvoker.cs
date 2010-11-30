using System;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Engine
{
    internal class FunctionInvoker<T> : MethodInvoker<T>
    {
        private readonly Func<T, T> _function;
        private readonly Func<bool> _boolFunction;

        public FunctionInvoker(Func<T, T> function)
        {
            Check.IsNotNull(function, function.Method.Name);
            _function = function;
        }

        public FunctionInvoker(Func<bool> function)
        {
            Check.IsNotNull(function, function.Method.Name);
            _boolFunction = function;
            IsContextBound = true;    
        }

        public override T Execute(T data)
        {
            return _function.Invoke(data);
        }

        public virtual bool Execute()
        {
            return _boolFunction.Invoke();
        }

        public override int GetHashCode()
        {
            return IsContextBound ? _boolFunction.GetHashCode() : _function.GetHashCode();
        }
    }
}