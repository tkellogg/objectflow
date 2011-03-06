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
        /// <summary>Index for </summary>
        private Dictionary<object, IWorkflow<T>> subflows = 
                                            new Dictionary<object, IWorkflow<T>>();

        /// <summary>An ordered list of keys</summary>
        private List<object> keys = new List<object>();

        private IWorkflow<T> current = new Workflow<T>();
        private IWorkflow<T> first;

        /// <summary>
        /// Creates a workflow that has persistable states and is identified by
        /// <c>workflowId</c>.
        /// </summary>
        /// <param name="workflowId">Persistable value that represents this workflow.
        /// </param>
        public StatefulWorkflow(object workflowId)
        {
            WorkflowId = workflowId;
        }

        /// <summary>
        /// Creates a new workflow that has persistable states. Usage of this 
        /// constructure asserts that an object that might pass through this workflow
        /// will pass through only this workflow and no other stateful workflow.
        /// Because of this restriction, it is recommended that you use 
        /// <see cref="StatefulWorkflow(object)"/> instead.
        /// </summary>
        public StatefulWorkflow() :this(null) {}

        #region new IStatefulWorkflow<T> Members

        /// <summary>
        /// Gets an identifier that describes the workflow. This can be a string,
        /// number, Guid, or any other object that provides a meaningful implementation
        /// of the <c>.Equals(object)</c> method.
        /// </summary>
        public virtual object WorkflowId { get; private set; }

        /// <summary>
        /// Continue the workflow where the given object left off.
        /// </summary>
        /// <returns></returns>
        public virtual IStatefulWorkflow<T> Yield(object breakPointId)
        {
            if (first == null)
                first = current;
            else {
                keys.Add(breakPointId);
                subflows[breakPointId] = current;
                current = new Workflow<T>();
            }
            return this;
        }

        /// <summary>
        /// Continues execution of the workflow where <c>obj</c> last left off 
        /// after <c>Yield</c> was called.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual T Continue(T data)
        {

            return data;
        }

        /// <summary>
        /// Indicates that <c>obj</c> is currently passing through this workflow
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool IsMidlife(T data)
        {
            return data != null && data.GetStateId(WorkflowId) != null;
        }

        /// <summary>
        /// Start the workflow
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override T Start(T data)
        {
            return base.Start(data);
        }

        #endregion

        #region Members that hide IWorkflow<T> Members

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
