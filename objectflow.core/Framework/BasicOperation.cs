using System.Collections.Generic;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Implements common functionality for Operation objects.  
    /// All operations must inherit from BasicOperation.
    /// </summary>
    /// <typeparam name="T">Type of object this operation will work on.
    /// </typeparam>
    /// <example>
    /// 
    /// </example>
    public abstract class BasicOperation<T> : IOperation<T>
    {
        /// <summary>
        /// Adds an operation into the execution plan
        /// </summary>
        /// <param name="operations"></param>
        /// <returns></returns>
        public abstract IEnumerable<T> Execute(IEnumerable<T> operations);

        protected BasicOperation()
        {
            SuccessResult = false;
        }

        /// <summary>
        /// returns true if the operation succeeded, false otherwise.  Use the SetSuccessResult virtual method to set this property.
        /// </summary>
        public bool SuccessResult { get; private set; }

        /// <summary>
        /// This method is called when an operation succeeds without throwing an exception.  
        /// Override this method to define custom success criteria.
        /// </summary>
        /// <param name="succeeded">The value to set the SuccessResult property to.</param>
        /// <returns>Returns SuccessResult property after this method has set it.  This should be the same as the value passed to it.</returns>
        /// <example>
        /// Override example
        /// </example>
        public virtual bool SetSuccessResult(bool succeeded)
        {
            SuccessResult = succeeded;
            return SuccessResult;
        }
    }
}