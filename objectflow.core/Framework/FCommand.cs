using System;

namespace Rainbow.ObjectFlow.Framework
{
    internal class FCommand<T> : Command<T>
    {
        private readonly Func<T, T> _function;

        public FCommand(Func<T, T> function)
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