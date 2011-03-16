using Castle.Core;
using Castle.MicroKernel;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Interfaces;
using Castle.MicroKernel.Registration;
using System;
using System.IO;

namespace Rainbow.ObjectFlow.Container
{

#pragma warning disable 1591
    internal static class ServiceLocator<T> 
        where T : class
    {
        /** 
         * TODO: this seems strange that we create a new container, with all transient types
         * for each type that a workflow might use. Memory hog??  How about we refactor this
         * into a non-generic base class and just do registrations in the generic child class?
         * Honestly, the method should have been generic, not the class.
         */
        private static IKernel _container;

        private static void EnsureCastleContainer()
        {
            if (null == _container)
            {
                _container = new DefaultKernel();
                _container.AddComponent<SequentialBuilder<T>>(LifestyleType.Transient);
                _container.AddComponent<ParallelSplitBuilder<T>>(LifestyleType.Transient);
                _container.AddComponent<TaskList<T>>(LifestyleType.Transient);
                _container.AddComponent<Dispatcher<T>>(LifestyleType.Transient);
            }
        }

        private static U GetFromContainerishLookingThing<U>()
        {
            object ret = null;
            if (typeof(U) == typeof(SequentialBuilder<T>))
                ret = new SequentialBuilder<T>(GetFromContainerishLookingThing<TaskList<T>>());
            else if (typeof(U) == typeof(ParallelSplitBuilder<T>))
                ret = new ParallelSplitBuilder<T>(GetFromContainerishLookingThing<TaskList<T>>());
            else if (typeof(U) == typeof(TaskList<T>))
                ret = new TaskList<T>();
            else if (typeof(U) == typeof(Dispatcher<T>))
                ret = new Dispatcher<T>();
            else throw new InvalidCastException(string.Format("I don't know how to resolve type {0}", typeof(U)));
            return (U)ret;
        }

        /// <summary>
        /// Resolve an instance of type U. If castle isn't available, use our ugly
        /// and inflexible internal "container".
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public static U Resolve<U>()
        {
            /*
             * Note: All castle code is turned off for Release. This is simply just to 
             * keep requirements simple. The only reason I can see for keeping castle
             * around is to make unit tests easy.
             */
#if DEBUG
            EnsureCastleContainer();
            return _container.Resolve<U>();
#else
            return GetFromContainerishLookingThing<U>();
#endif
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
