using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// A workflow that is able to pause at states and be resumed later.
    /// </summary>
    /// <typeparam name="T">The object that will be stepped through the workflow</typeparam>
    public interface IStatefulWorkflow<T> : IWorkflow<T>
        where T : class, IStatefulObject
    {
        /// <summary>
        /// Signal that the workflow should pause here and then resume later
        /// at this same point
        /// </summary>
        /// <param name="breakPointId">The identifier that describes where the workflow
        /// yielded so that it can return to this point later.</param>
        /// <returns></returns>
        IStatefulWorkflow<T> Yield(object breakPointId);

        #region overriden from base class, but returning IStatefulWorkflow

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        new IStatefulWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>;

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        new IStatefulWorkflow<T> Do<TOperation>(ICheckConstraint constraint) where TOperation : BasicOperation<T>;

        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        new IStatefulWorkflow<T> Do(IOperation<T> operation);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        new IStatefulWorkflow<T> Do(Func<T, T> function);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        new IStatefulWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint);

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        new IStatefulWorkflow<T> Do(IOperation<T> operation, ICheckConstraint constraint);

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The function to add</param>
        new IStatefulWorkflow<T> Do(IWorkflow<T> workflow);

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the workflow is executed</param>
        new IStatefulWorkflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint);
        #endregion
    }
}
