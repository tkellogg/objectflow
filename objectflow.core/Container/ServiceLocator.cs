using Castle.Core;
using Castle.MicroKernel;
using Rainbow.ObjectFlow.Engine;

namespace Rainbow.ObjectFlow.Container
{

#pragma warning disable 1591
    internal static class ServiceLocator<T> where T :class 
    {
        private static IKernel _container;
        public static IKernel Get()
        {
            if (null == _container)
            {
                _container = new DefaultKernel();
                _container.AddComponent<SequentialBuilder<T>>(LifestyleType.Transient);
                _container.AddComponent<ParallelSplitBuilder<T>>(LifestyleType.Transient);
                _container.AddComponent<TaskList<T>>(LifestyleType.Transient);
                _container.AddComponent<Dispatcher<T>>(LifestyleType.Transient);
            }

            return _container;
        }

        public static void SetInstance(IKernel container)
        {
            _container = container;
        }

        public static void Reset()
        {
            _container = null;
        }
    }
}
#pragma warning restore 1591
