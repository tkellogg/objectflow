using System;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Engine
{
    internal class FunctionInvoker<T> : MethodInvoker<T>
    {
        private readonly Func<T, T> _function;

        /// <summary>
        /// Used by mock framework
        /// </summary>
        internal FunctionInvoker()
        {

        }
        public FunctionInvoker(Func<T, T> function)
        {
            Check.IsNotNull(function, function.Method.Name);
            _function = function;
        }

        public override T Execute(T data)
        {
            return _function.Invoke(data);
        }

        /// <exception cref="System.MemberAccessException">Thrown when accessed with late binding</exception>
        public override int GetHashCode()
        {
            return _function.GetHashCode();
        }
    }
}