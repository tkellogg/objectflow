using System;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IMerge<T> : IHideObjectMembers where T: class
    {
        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        /// <returns>this</returns>
        IWorkflow<T> Do(IOperation<T> operation);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <returns>this</returns>
        IWorkflow<T> Do(Func<T, T> function);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint"></param>
        /// <returns>this</returns>
        IWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint);

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that defines if the operation is executed</param>
        /// <returns>this</returns>
        IWorkflow<T> Do(IOperation<T> operation, ICheckConstraint constraint);

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The function to add</param>
        /// <returns>this</returns>
        IWorkflow<T> Do(IWorkflow<T> workflow);

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The funciton to add</param>
        /// <param name="constraint"></param>
        /// <returns>this</returns>
        IWorkflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint);
    }
}