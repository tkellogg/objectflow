using System.Collections.Generic;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal abstract class MethodInvoker<T>
    {
        public IList<IPolicy> Policies;
        
        public abstract T Execute(T data);

        protected MethodInvoker()
        {
            Policies = new List<IPolicy>();
        }

    }
}