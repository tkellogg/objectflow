using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Language;

#pragma warning disable 1591

namespace Rainbow.ObjectFlow.Policies
{
    public abstract class Policy : IHideObjectMembers
    {
        internal object Invoker;

        internal void SetInvoker<T>(MethodInvoker<T> method)
        {
            Invoker = method;
        }

        internal abstract T Execute<T>(T current);
    }
}