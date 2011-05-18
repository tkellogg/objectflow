using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Engine;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StatefulWorkflow<T> : Workflow<T>, IStatefulWorkflow<T>
        where T : class, IStatefulObject
    {
        private ITransitionGateway _gateway;

		private StatefulBuilder<T> _builder;
		private IErrorHandler<T> _faultHandler;
		private ITransitionRule<T> _transitionRule;

		#region Constructors

		/// <summary>
		/// Creates a workflow that has persistable states identified by
		/// <c>workflowId</c>.
		/// </summary>
		/// <param name="workflowId">Persistable value that represents this workflow.
		/// </param>
		public StatefulWorkflow(object workflowId)
			:this(workflowId, null)
		{
		}

		/// <summary>
		/// Creates a workflow that has persistable states identified by
		/// <c>workflowId</c> and provided by a transition security layer
		/// </summary>
		/// <param name="workflowId"></param>
		/// <param name="gateway">The security gateway that has the ability to diallow
		/// a transition from ever happening.</param>
		public StatefulWorkflow(object workflowId, ITransitionGateway gateway)
			:this(workflowId, gateway, new DefaultTransitionRule<T>(workflowId))
		{}

		/// <summary>
		/// Creates a workflow that has persistable states identified by
		/// <c>workflowId</c> and provided by a transition security layer
		/// </summary>
		/// <param name="workflowId"></param>
		/// <param name="gateway">The security gateway that has the ability to diallow
		/// a transition from ever happening.</param>
		/// <param name="transitionRule">transition rule used to guide objects through the workflow</param>
		public StatefulWorkflow(object workflowId, ITransitionGateway gateway, ITransitionRule<T> transitionRule)
		{
			WorkflowId = workflowId;
			_faultHandler = new StatefulErrorHandler<T>();
			this._gateway = gateway;
			_builder = new StatefulBuilder<T>(workflowId, transitionRule, _faultHandler);
			_transitionRule = transitionRule;
		}

		/// <summary>
		/// Creates a new workflow that has persistable states. Usage of this 
		/// constructure asserts that an object that might pass through this workflow
		/// will pass through only this workflow and no other stateful workflow.
		/// Because of this restriction, it is recommended that you use 
		/// <c>StatefulWorkflow(object)</c> instead.
		/// </summary>
		public StatefulWorkflow() : this(null) { }

		#endregion

		
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
			_builder.AddYield(stateId);
            return this;
        }

        /// <summary>
        /// Start the workflow
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override T Start(T data)
        {
			return StartWithParams(data);
		}

		/// <summary>
		/// Start the workflow with given parameters
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public T StartWithParams(T subject, params object[] parameters)
		{
			_builder.EndDefinitionPhase();
			CheckThatTransitionIsAllowed(subject.GetStateId(this.WorkflowId));
			var workflow = _builder.GetCurrentFlowForObject(subject);
			if (parameters.Length > 0)
				workflow.WorkflowBuilder.Tasks.SetParameters(parameters);
			return workflow.Start(subject);
		}

		/// <summary>
		/// Start the workflow with extra arguments
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <param name="subject">Object being operated on</param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="arg3"></param>
		public virtual T Start<T1, T2, T3>(T subject, T1 arg1, T2 arg2, T3 arg3)
		{
			return StartWithParams(subject, arg1, arg2, arg3);
		}

		/// <summary>
		/// Start the workflow with extra arguments
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="subject">Object being operated on</param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		public virtual T Start<T1, T2>(T subject, T1 arg1, T2 arg2)
		{
			return StartWithParams(subject, arg1, arg2);
		}

		/// <summary>
		/// Start the workflow with extra arguments
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="subject">Object being operated on</param>
		/// <param name="arg1"></param>
		public virtual T Start<T1>(T subject, T1 arg1)
		{
			return StartWithParams(subject, arg1);
		}


        /// <summary>
        /// Begins the execution of a workflow
        /// </summary>
        /// <returns>The data after being transformed by the Operation objects</returns>
        public override T Start()
        {
			var ret = _builder.First.Start();
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
			_builder.Current.WorkflowBuilder.AddOperation(body);
			return this;
		}

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <typeparam name="T2">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		public virtual IStatefulWorkflow<T> Do<T1, T2>(Action<T, T1, T2> body)
		{
			_builder.Current.WorkflowBuilder.AddOperation(body);
			return this;
		}
		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		public virtual IStatefulWorkflow<T> Do<T1>(Action<T, T1> body)
		{
			_builder.Current.WorkflowBuilder.AddOperation(body);
			return this;
		}

        #region Convenience methods for the fluent interface

        /// <summary>
        /// When function returns true, branch to the specified operation
        /// </summary>
        /// <param name="function"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        public virtual IStatefulWorkflow<T> When(Func<T, bool> function, IDeclaredOperation otherwise)
        {
            _builder.AnalyzeTransitionPaths(otherwise);
            bool failedCheck = false;
            _builder.Current.Do(x =>
            {
                CheckThatTransitionIsAllowed(x.GetStateId(this.WorkflowId), otherwise);
                if (!function(x))
                    failedCheck = true;
                return x;
            });
            _builder.Current.Do(x => x, If.IsTrue(() => !failedCheck, otherwise));
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
            _builder.Current.Do(x => x, defineAs);
            _builder.CreateDecisionPath(defineAs);
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
            _builder.Current.Do<TOperation>();
            return this;
        }

        /// <summary>
        /// Registers an instance of the specified type in the workflow
        /// </summary>
        public new IStatefulWorkflow<T> Do<TOperation>(ICheckConstraint constraint)
            where TOperation : BasicOperation<T>
        {
            _builder.Current.Do<TOperation>(constraint);
            return this;
        }

        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        public new IStatefulWorkflow<T> Do(IOperation<T> operation)
        {
            _builder.Current.Do(operation);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        public new IStatefulWorkflow<T> Do(Func<T, T> function)
        {
            _builder.Current.Do(function);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <param name="branch"></param>
        public new IStatefulWorkflow<T> Do(Func<T, T> function, IDeclaredOperation branch)
        {
            _builder.Current.Do(function, branch);
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
            _builder.Current.Do(function, constraint, branch);
            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        public new IStatefulWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint)
        {
            _builder.Current.Do(function, constraint);
            return this;
        }

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        public new IStatefulWorkflow<T> Do(IOperation<T> operation, ICheckConstraint constraint)
        {
            _builder.Current.Do(operation, constraint);
            return this;
        }

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The function to add</param>
        public new IStatefulWorkflow<T> Do(IWorkflow<T> workflow)
        {
            _builder.Current.Do(workflow);
            return this;
        }
        
        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the workflow is executed</param>
        public new IStatefulWorkflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            _builder.Current.Do(workflow, constraint);
            return this;
        }

        #endregion

        #region Transitions

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
			get { return _builder.Transitions; }
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
                object toState = _builder.GetDestinationState(to);
                var list = PossibleTransitions.Where(x => object.Equals(x.From, from)
                    && object.Equals(x.To, toState)).ToList();
                if (!_gateway.AllowTransitions(list).Any())
                    throw new UnallowedTransitionException("No transitions allowed from states: {0} to {1}", 
                        from, toState);
            }
        }

        #endregion


		#region IStateObserver<T> Members

		/// <summary>
		/// If <see cref="IsInWorkflow"/> is false, this indicates that the entity has been
		/// in a workflow at least once. This returns null if this information can't be determined.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool? HasBeenInWorkflow(T entity)
		{
			return _transitionRule.HasBeenInWorkflow(entity);
		}

		/// <summary>
		/// True if the entity is currently passing through the workflow
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool IsInWorkflow(T entity)
		{
			return _transitionRule.IsInWorkflow(entity);
		}

		#endregion
	}
}
