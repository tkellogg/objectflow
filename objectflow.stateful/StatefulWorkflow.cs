using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Stateful.Framework;
using System.ComponentModel;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class StatefulWorkflow<T> : Workflow<T>, IStatefulWorkflow<T>
		where T : class, IStatefulObject
	{
		private ITransitionGateway _gateway;

		private StatefulBuilder<T> _builder;
		private ITransitionRule<T> _transitionRule;

		/// <summary>
		/// Handles errors during workflow execution. You can set
		/// <code>ErrorHandler.Strict = true</code> or extend it for more specific
		/// functionality.
		/// </summary>
		public override IErrorHandler<T> ErrorHandler {
			get { return _errorHandler; }
			set 
			{
				_errorHandler = value;
				if (_builder != null)
					_builder.ErrorHandler = value;
			} 
		}

		private IErrorHandler<T> _errorHandler;


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
			ErrorHandler = new StatefulErrorHandler<T>();
			this._gateway = gateway;
			_builder = new StatefulBuilder<T>(workflowId, transitionRule, ErrorHandler);
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
			return StartWithParams(data, null);
		}

		/// <summary>
		/// Start the workflow with given parameters
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public T StartWithParams(T subject, IDictionary<string, object> parameters)
		{
			SaveStartState(subject);
			_builder.EndDefinitionPhase();
			CheckThatTransitionIsAllowed(subject.GetStateId(this.WorkflowId));
			var workflow = _builder.GetCurrentFlowForObject(subject);
			if (parameters != null && parameters.Count > 0)
				workflow.WorkflowBuilder.Tasks.SetParameters(parameters);
			try
			{
				return workflow.Start(subject);
			}
			finally
			{
				ReleaseSavedState(subject);
			}
		}

		/// <summary>
		/// Start the workflow with the given anonymous object as parameters
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="asParameters"></param>
		/// <returns></returns>
		public T StartWithParams(T subject, object asParameters)
		{
			return StartWithParams(subject, asParameters.ToDictionary());
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
		/// 
		/// </summary>
		/// <param name="function"></param>
		/// <returns></returns>
		public virtual IStatefulWorkflow<T> Do(Action<T, IDictionary<string, object>> function)
		{
			_builder.Current.WorkflowBuilder.AddOperation(function);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		/// <returns></returns>
		public virtual IStatefulWorkflow<T> Do(Func<T, IDictionary<string, object>, T> function)
		{
			_builder.Current.WorkflowBuilder.AddOperation((x, opts) => function(x, opts));
			return this;
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
		[Obsolete("Constraints are not necessary. Use .When(...).Do(x => {}) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[Obsolete("Constraints are not necessary. Use .When(...).Do(x => {}) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual IStatefulWorkflow<T> Do(Action<T> function, ICheckConstraint constraint, IDeclaredOperation defineAs)
		{
			return Do(x =>
			{
				function(x);
				return x;
			}, constraint, defineAs);
		}

		#region Convenience methods for the fluent interface

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

		/// <summary>
		/// If the condition is true
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		public virtual IBranchingExpression<T> When(Predicate<T> condition)
		{
			return new BranchingExpression<T>(this, condition, _builder.Current, _builder, _transitionRule);
		}

		/// <summary>
		/// If the condition is true
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		public virtual IBranchingExpression<T> When(Func<T, IDictionary<string, object>, bool> condition)
		{
			return new AdvancedBranchingExpression<T>(this, condition, _builder.Current, _builder, _transitionRule);
		}

		/// <summary>
		/// If the condition is false
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		public virtual IBranchingExpression<T> Unless(Predicate<T> condition)
		{
			return new BranchingExpression<T>(this, x => !condition(x), _builder.Current, _builder, _transitionRule);
		}

		/// <summary>
		/// If the condition is false
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		public virtual IBranchingExpression<T> Unless(Func<T, IDictionary<string, object>, bool> condition)
		{
			return new AdvancedBranchingExpression<T>(this, (x, opts) => !condition(x, opts), _builder.Current, _builder, _transitionRule);
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
		[Obsolete("Constraints are not necessary. Use .When(...).Do(x => {}) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[Obsolete("Constraints are not necessary. Use .When(...).Do(x => {}) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[Obsolete("Constraints are not necessary. Use .When(...).Do(x => {}) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[Obsolete("Constraints are not necessary. Use .When(...).Do(x => {}) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[Obsolete("Constraints are not necessary. Use .When(...).Do(x => {}) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		/// <summary>
		/// Check the security gateway to ensure that any potential transitions through the branch point
		/// are allowed
		/// </summary>
		internal virtual void CheckThatTransitionIsAllowed(T entity, IDeclaredOperation to)
		{
			if (_gateway != null)
			{
				object from = GetOriginalState(entity);
				object toState = _builder.GetDestinationState(to);
				CheckThatTransitionIsAllowed(from, toState);
			}
		}

		internal void CheckThatTransitionIsAllowed(object from, object toState)
		{
			if (_gateway != null)
			{
				var list = PossibleTransitions.Where(x => object.Equals(x.From, from)
								&& object.Equals(x.To, toState)).ToList();
				if (!_gateway.AllowTransitions(list).Any())
					throw new UnallowedTransitionException("No transitions allowed from states: {0} to {1}",
						from, toState);
			}
		}


		#region Retain starting state

		private List<KeyValuePair<T, object>> _startStates = new List<KeyValuePair<T, object>>();
		
		private object GetOriginalState(T entity)
		{
			var i = GetIndexOfStoredEntity(entity);
			if (i.HasValue)
				return _startStates[i.Value].Value;
			else return null;
		}

		private void SaveStartState(T entity)
		{
			ReleaseSavedState(entity);
			_startStates.Add(new KeyValuePair<T, object>(entity, entity.GetStateId(WorkflowId)));
		}

		private int? GetIndexOfStoredEntity(T entity) 
		{
			for (int i = 0; i < _startStates.Count; i++)
			{
				if (object.ReferenceEquals(_startStates[i].Key, entity))
					return i;
			}
			return null;
		}

		/// <summary>
		/// Release our reference to subject to prevent memory leaks
		/// </summary>
		private void ReleaseSavedState(T entity)
		{
			var i = GetIndexOfStoredEntity(entity);
			if (i.HasValue)
				_startStates.RemoveAt(i.Value);
		}

		#endregion

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
