using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StatefulWorkflow<T> : Workflow<T>, IStatefulWorkflow<T>
        where T : class, IStatefulObject
    {
        private IErrorHandler<T> _faultHandler;
        private IWorkflow<T> _current;
        private ITransitionGateway _gateway;
        private List<ITransition> _transitions;

        /// <summary>Index for </summary>
        private Dictionary<object, int> subflow_idx
                            = new Dictionary<object, int>();
        private List<IWorkflow<T>> subflows = new List<IWorkflow<T>>();
        private object nextKey;
        /// <summary>Used for transitions to know what key we're building from</summary>
        private object currentKey;
        
        private void AddFlow(object key, IWorkflow<T> flow)
        {
            if (!subflow_idx.ContainsKey(key))
            {
                if (nextKey != null)
                    subflow_idx[nextKey] = subflows.Count;
                subflows.Add(flow);
                AddTransition(nextKey, key);
                currentKey = nextKey;
                nextKey = key;
            }
        }

        private void EndDefinitionPhase()
        {
            if (nextKey != null)
            {
                if (!subflow_idx.ContainsKey(nextKey))
                {
                    _current.Do(x => { x.SetStateId(WorkflowId, null); return x; });
                    subflow_idx[nextKey] = subflows.Count;
                    subflows.Add(_current);
                    AddRemainingTransitions();
                }
            }
            else
                AddTransition(null, null);
            
            // free up some memory that won't be required anymore
            undefinedForwardRefs = null;
            definedRefs = null;
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

        /// <summary>
        /// Creates a workflow that has persistable states identified by
        /// <c>workflowId</c>.
        /// </summary>
        /// <param name="workflowId">Persistable value that represents this workflow.
        /// </param>
        public StatefulWorkflow(object workflowId)
        {
            WorkflowId = workflowId;
            _faultHandler = new ErrorHandler<T>();
            _current = new Workflow<T>(_faultHandler);
        }

        /// <summary>
        /// Creates a workflow that has persistable states identified by
        /// <c>workflowId</c> and provided by a transition security layer
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="gateway">The security gateway that has the ability to diallow
        /// a transition from ever happening.</param>
        public StatefulWorkflow(object workflowId, ITransitionGateway gateway)
        {
            WorkflowId = workflowId;
            _faultHandler = new StatefulErrorHandler<T>();
            _current = new Workflow<T>(_faultHandler);
            this._gateway = gateway;
            this._transitions = new List<ITransition>();
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
            _current.Do(x => { x.SetStateId(WorkflowId, stateId); return x; });
            AddFlow(stateId, _current);
            _current = new Workflow<T>(_faultHandler);
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
            CheckThatTransitionIsAllowed(data.GetStateId(this.WorkflowId));
            var ret = GetCurrentFlowForObject(data).Start(data);
            return ret;
        }

        /// <summary>
        /// Begins the execution of a workflow
        /// </summary>
        /// <returns>The data after being transformed by the Operation objects</returns>
        public override T Start()
        {
            var ret = First.Start();
            return ret;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        public virtual IStatefulWorkflow<T> Do(Action<T> function)
        {
            return Do(x =>
            {
                function(x);
                return x;
            });
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <param name="branch">Branch point to initialize</param>
        public virtual IStatefulWorkflow<T> Do(Action<T> function, IDeclaredOperation branch)
        {
            return Do(x =>
            {
                function(x);
                return x;
            }, branch);
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        public virtual IStatefulWorkflow<T> Do(Action<T> function, ICheckConstraint constraint)
        {
            return Do(x =>
            {
                function(x);
                return x;
            }, constraint);
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        /// <param name="defineAs">Branch point to initialize</param>
        public virtual IStatefulWorkflow<T> Do(Action<T> function, ICheckConstraint constraint, IDeclaredOperation defineAs)
        {
            return Do(x =>
            {
                function(x);
                return x;
            }, constraint, defineAs);
        }


		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <typeparam name="T2">Extra parameter used from start</typeparam>
		/// <typeparam name="T3">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		public virtual IStatefulWorkflow<T> Do<T1, T2, T3>(Action<T, T1, T2, T3> body)
		{
			return null;
		}

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <typeparam name="T2">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		public virtual IStatefulWorkflow<T> Do<T1, T2>(Action<T, T1, T2> body);
		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		public virtual IStatefulWorkflow<T> Do<T1>(Action<T, T1> body);

        #region Convenience methods for the fluent interface

        /// <summary>
        /// When function returns true, branch to the specified operation
        /// </summary>
        /// <param name="function"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        public virtual IStatefulWorkflow<T> When(Func<T, bool> function, IDeclaredOperation otherwise)
        {
            AnalyzeTransitionPaths(otherwise);
            bool failedCheck = false;
            _current.Do(x =>
            {
                CheckThatTransitionIsAllowed(x.GetStateId(this.WorkflowId), otherwise);
                if (!function(x))
                    failedCheck = true;
                return x;
            });
            _current.Do(x => x, If.IsTrue(() => !failedCheck, otherwise));
            return this;
        }

        /// <summary>
        /// When function returns false, branch to the specified operation
        /// </summary>
        /// <param name="function"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        public virtual IStatefulWorkflow<T> Unless(Func<T, bool> function, IDeclaredOperation otherwise)
        {
            return When(x => !function(x), otherwise);
        }

        /// <summary>
        /// Declare a point that you may wish to branch to later
        /// </summary>
        /// <param name="defineAs"></param>
        /// <returns></returns>
        public virtual IStatefulWorkflow<T> Define(IDeclaredOperation defineAs)
        {
            _current.Do(x => x, defineAs);
            CreateDecisionPath(defineAs);
            return this;
        }

        #endregion

        #endregion

        #region Members that hide IWorkflow<T> Members

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        public new IStatefulWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>
        {
            _current.Do<TOperation>();
            return this;
        }

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        public new IStatefulWorkflow<T> Do<TOperation>(ICheckConstraint constraint)
            where TOperation : BasicOperation<T>
        {
            _current.Do<TOperation>(constraint);
            return this;
        }

        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        public new IStatefulWorkflow<T> Do(IOperation<T> operation)
        {
            _current.Do(operation);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        public new IStatefulWorkflow<T> Do(Func<T, T> function)
        {
            _current.Do(function);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <param name="branch"></param>
        public new IStatefulWorkflow<T> Do(Func<T, T> function, IDeclaredOperation branch)
        {
            _current.Do(function, branch);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <param name="constraint"></param>
        /// <param name="branch"></param>
        public new IStatefulWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint, IDeclaredOperation branch)
        {
            _current.Do(function, constraint, branch);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        public new IStatefulWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint)
        {
            _current.Do(function, constraint);
            return this;
        }

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        public new IStatefulWorkflow<T> Do(IOperation<T> operation, ICheckConstraint constraint)
        {
            _current.Do(operation, constraint);
            return this;
        }

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The function to add</param>
        public new IStatefulWorkflow<T> Do(IWorkflow<T> workflow)
        {
            _current.Do(workflow);
            return this;
        }
        
        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the workflow is executed</param>
        public new IStatefulWorkflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            _current.Do(workflow, constraint);
            return this;
        }

        #endregion

        #region Transitions

        /// <summary>Accumulates all refs that refer to this, and stores workflows where
        /// they might go.</summary>
        private Dictionary<IDeclaredOperation, List<object>> undefinedForwardRefs
            = new Dictionary<IDeclaredOperation, List<object>>();

        /// <summary>a single ref maps to the first yield. Although, if there
        /// are branches before that first Yield, this will map to multiple paths</summary>
        private Dictionary<IDeclaredOperation, object> definedRefs
            = new Dictionary<IDeclaredOperation, object>();

        /// <summary>Need to hold onto these for all eternity</summary>
        private Dictionary<IDeclaredOperation, object> allDefinedRefs
            = new Dictionary<IDeclaredOperation, object>();

        private List<IDeclaredOperation> currentFlowDefs = new List<IDeclaredOperation>();

        /// <summary>
        /// Called when making decisions
        /// </summary>
        /// <param name="branchPoint"></param>
        private void AnalyzeTransitionPaths(IDeclaredOperation branchPoint)
        {
            if (definedRefs.ContainsKey(branchPoint))
            {
                AddTransition(nextKey, definedRefs[branchPoint], branchPoint);
            }
            else
            {
                if (!undefinedForwardRefs.ContainsKey(branchPoint))
                    undefinedForwardRefs[branchPoint] = new List<object>();
                undefinedForwardRefs[branchPoint].Add(nextKey);
            }
        }

        /// <summary>
        /// Called when .Define()'ing points
        /// </summary>
        /// <param name="branchPoint"></param>
        private void CreateDecisionPath(IDeclaredOperation branchPoint)
        {
            if (definedRefs.ContainsKey(branchPoint))
                throw new InvalidOperationException("branch point is already defined. "
                        + "Cannot multiply define branch points");
            definedRefs[branchPoint] = nextKey;
            if (undefinedForwardRefs.ContainsKey(branchPoint))
            {
                foreach (var key in undefinedForwardRefs[branchPoint])
                {
                    AddTransition(nextKey, key, branchPoint);
                }
                undefinedForwardRefs.Remove(branchPoint);
            }
        }

        private void AddTransition(object from, object to)
        {
            if (_transitions != null)
            {
                var t = new Transition(WorkflowId, from, to);
                if (!_transitions.Contains(t))
                    _transitions.Add(t);
            }
        }

        private void AddTransition(object from, object to, IDeclaredOperation op)
        {
            AddTransition(from, to);
            if (_transitions != null)
                allDefinedRefs[op] = to;
        }

        private void AddRemainingTransitions()
        {
            AddTransition(nextKey, null);
        }

        /// <summary>
        /// <para>
        /// Get an enumeration of only possible transitions of this workflow. Usage
        /// of this method before workflow definition is finished is meaningless, so
        /// refrain from calling this until the workflow is entirely defined to a 
        /// point that you could call <c>.Start()</c>
        /// </para>
        /// <para>
        /// If an ITransitionGateway instance has not been supplied to this object 
        /// (i.e. in the constructor), this property will throw an exception.
        /// </para>
        /// </summary>
        public virtual IEnumerable<ITransition> PossibleTransitions
        {
            get {
                EndDefinitionPhase();
                return _transitions; 
            }
        }

        private void CheckThatTransitionIsAllowed(object from)
        {
            if (_gateway != null)
            {
                var list = PossibleTransitions.Where(x => object.Equals(x.From, from)).ToList();
                if (!_gateway.AllowTransitions(list).Any())
                    throw new UnallowedTransitionException("No transitions allowed from state: {0}", from);
            }
        }

        private void CheckThatTransitionIsAllowed(object from, IDeclaredOperation to)
        {
            if (_gateway != null)
            {
                object toState = allDefinedRefs[to];
                var list = PossibleTransitions.Where(x => object.Equals(x.From, from)
                    && object.Equals(x.To, toState)).ToList();
                if (!_gateway.AllowTransitions(list).Any())
                    throw new UnallowedTransitionException("No transitions allowed from states: {0} to {1}", 
                        from, toState);
            }
        }

        #endregion

    }
}
