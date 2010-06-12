
namespace Rainbow.ObjectFlow.Engine
{
    internal abstract class MethodInvoker<T>
    {
        public abstract T Execute(T data);
    }
}