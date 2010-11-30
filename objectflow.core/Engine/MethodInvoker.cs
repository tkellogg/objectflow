using System.Collections.Generic;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal abstract class MethodInvoker<T>
    {
        public IList<IPolicy> Policies;
        
        /// <summary>
        /// Determines if the method uses the context property to hold state instead of returning it
        /// <remarks>This is used by generic function of bool</remarks>
        /// </summary>
        public bool IsContextBound = false;

        public abstract T Execute(T data);

        protected MethodInvoker()
        {
            Policies = new List<IPolicy>();
        }

    }
}