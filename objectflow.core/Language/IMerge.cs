using System;
using Rainbow.ObjectFlow.Framework;
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

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        /// <typeparam name="TOperation">Type that inherits from BasicOperation of T</typeparam>
        IWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>;

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        /// <typeparam name="TOperation">Type that inherits from BasicOperation of T</typeparam>
        /// <param name="constraint">The constraint to evaluate</param>
        IWorkflow<T> Do<TOperation>(ICheckConstraint constraint) where TOperation : BasicOperation<T>;

    }
}