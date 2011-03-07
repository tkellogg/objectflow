using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StatefulWorkflow<T> : Workflow<T>, IStatefulWorkflow<T>
        where T : class, IStatefulObject
    {
        /// <summary>Index for </summary>
        private Dictionary<object, int> subflow_idx
                            = new Dictionary<object, int>();
        private List<IWorkflow<T>> subflows = new List<IWorkflow<T>>();
        private object nextKey;
        
        private void AddFlow(object key, IWorkflow<T> flow)
        {
            if (!subflow_idx.ContainsKey(key))
            {
                if (nextKey != null)
                    subflow_idx[nextKey] = subflows.Count;
                subflows.Add(flow);
                nextKey = key;
            }
        }

        private void EndDefinitionPhase()
        {
            if (nextKey != null && !subflow_idx.ContainsKey(nextKey))
            {
                subflow_idx[nextKey] = subflows.Count;
                subflows.Add(current);
            }
        }

        /// <summary>The first flow of the workflow</summary>
        protected IWorkflow<T> First
        {
            get
            {
                if (subflows.Count == 0)
                    throw new InvalidOperationException("No workflow steps have been defined");
                return subflows[0];
            }
        }

        private IWorkflow<T> GetCurrentFlowForObject(T data)
        {
            if (!data.IsAliveInWorkflow(this))
                return First;
            else
            {
                var state = data.GetStateId(this.WorkflowId);
                if (subflow_idx.ContainsKey(state))
                    return subflows[subflow_idx[state]];
                else
                    throw new InvalidOperationException("Object is not passing through workflow " + this.WorkflowId);
            }
        }

        private object GetKeyFromIndex(int index)
        {
            return (from x in subflow_idx
                    where x.Value == index
                    select x.Key).FirstOrDefault();
        }

        private IWorkflow<T> current = new Workflow<T>();

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
        /// <c>StatefulWorkflow(object)</c> instead.
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
        /// <param name="stateId">Define the state ID that an object will be given as
        /// it leaves this step.</param>
        /// <returns></returns>
        public virtual IStatefulWorkflow<T> Yield(object stateId)
        {
            AddFlow(stateId, current);
            current = new Workflow<T>();
            return this;
        }

        /// <summary>
        /// Start the workflow
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override T Start(T data)
        {
            EndDefinitionPhase();
            int oldIndex = 0;
            if (data.IsAliveInWorkflow(this))
                oldIndex = subflow_idx[data.GetStateId(this.WorkflowId)];
            var ret = GetCurrentFlowForObject(data).Start(data);
            SetStateIndex(oldIndex, ret);
            return ret;
        }

        private void SetStateIndex(int oldIndex, T ret)
        {
            EndDefinitionPhase();
            if (ret != null && subflows.Count >= oldIndex)
                ret.SetStateId(this.WorkflowId, GetKeyFromIndex(oldIndex + 1));
            else
                ret.SetStateId(this.WorkflowId, null);
        }

        /// <summary>
        /// Begins the execution of a workflow
        /// </summary>
        /// <returns>The data after being transformed by the Operation objects</returns>
        public override T Start()
        {
            var ret = First.Start();
            SetStateIndex(0, ret);
            return ret;
        }

        #endregion

        #region Members that hide IWorkflow<T> Members

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        public new IStatefulWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>
        {
            current.Do<TOperation>();
            return this;
        }

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do<TOperation>(ICheckConstraint constraint)
        {
            current.Do<TOperation>(constraint);
            return this;
        }

        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        public new IStatefulWorkflow<T> Do(IOperation<T> operation)
        {
            current.Do(operation);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(Func<T, T> function)
        {
            current.Do(function);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(Func<T, T> function, ICheckConstraint constraint)
        {
            current.Do(function, constraint);
            return this;
        }

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(IOperation<T> operation, ICheckConstraint constraint)
        {
            current.Do(operation, constraint);
            return this;
        }

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The function to add</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(IWorkflow<T> workflow)
        {
            current.Do(workflow);
            return this;
        }
        
        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the workflow is executed</param>
        IStatefulWorkflow<T> IStatefulWorkflow<T>.Do(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            current.Do(workflow, constraint);
            return this;
        }

        #endregion
    }
}
