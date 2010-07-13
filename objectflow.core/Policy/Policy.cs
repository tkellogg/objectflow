using System;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Interfaces;

#pragma warning disable 1591
namespace Rainbow.ObjectFlow.Framework
{
    public abstract class Policy : IHideObjectMembers
    {
        protected object Invoker;

        internal void SetParent<T>(MethodInvoker<T> method)
        {
            Invoker = method;
        }

        internal abstract T Execute<T>(T current);
    }
}