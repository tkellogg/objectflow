
namespace Rainbow.ObjectFlow.Framework
{
    internal abstract class Command<T>
    {
        public abstract T Execute(T data);
    }
}