using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StatefulWorkflow<T> : Workflow<T>, IStatefulWorkflow<T>
        where T : class, IStatefulObject
    {
        #region IStatefulWorkflow<T> Members

        /// <summary>
        /// Continue the workflow where the given object left off.
        /// </summary>
        /// <returns></returns>
        public IStatefulWorkflow<T> Yield(object breakPointId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStatefulWorkflow<T> Members

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        public new IStatefulWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>
        {
            return (IStatefulWorkflow<T>)base.Do<TOperation>();
        }

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do<TOperation>(ICheckConstraint constraint)
        {
            return (IStatefulWorkflow<T>)base.Do<TOperation>(constraint);
        }

        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        public new IStatefulWorkflow<T> Do(IOperation<T> operation)
        {
            return (IStatefulWorkflow<T>)base.Do(operation);
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(Func<T, T> function)
        {
            return (IStatefulWorkflow<T>)base.Do(function);
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(Func<T, T> function, ICheckConstraint constraint)
        {
            return (IStatefulWorkflow<T>)base.Do(function, constraint);
        }

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(IOperation<T> operation, ICheckConstraint constraint)
        {
            return (IStatefulWorkflow<T>)base.Do(operation, constraint);
        }

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The function to add</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(IWorkflow<T> workflow)
        {
            return (IStatefulWorkflow<T>)base.Do(workflow);
        }
        
        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the workflow is executed</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            return (IStatefulWorkflow<T>)base.Do(workflow, constraint);
        }

        #endregion
    }
}
