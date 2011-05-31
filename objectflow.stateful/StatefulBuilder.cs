using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// Class responsible for constructing stateful interactions using
	/// regular workflows
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class StatefulBuilder<T>
		where T : class, IStatefulObject
	{
		private List<IWorkflow<T>> _subflows = new List<IWorkflow<T>>();
		private ITransitionRule<T> _transitionRule;
		private IErrorHandler<T> _faultHandler;
        private object _nextKey;
        /// <summary>Used for transitions to know what key we're building from</summary>
		private object _currentKey;
		/// <summary>Index for </summary>
		private Dictionary<object, int> _subflow_idx
							= new Dictionary<object, int>();
		private List<ITransition> _transitions;

		/// <summary>Accumulates all refs that refer to this, and stores workflows where
		/// they might go.</summary>
		private Dictionary<IDeclaredOperation, List<object>> _undefinedForwardRefs
			= new Dictionary<IDeclaredOperation, List<object>>();

		/// <summary>a single ref maps to the first yield. Although, if there
		/// are branches before that first Yield, this will map to multiple paths</summary>
		private Dictionary<IDeclaredOperation, object> _definedRefs
			= new Dictionary<IDeclaredOperation, object>();

		/// <summary>Need to hold onto these for all eternity</summary>
		private Dictionary<IDeclaredOperation, object> _allDefinedRefs
			= new Dictionary<IDeclaredOperation, object>();

		private List<IDeclaredOperation> currentFlowDefs = new List<IDeclaredOperation>();

		private List<Action<object>> _deferredStates = new List<Action<object>>();

		private object _workflowId;

		public StatefulBuilder(object workflowId, ITransitionRule<T> transitionRule, IErrorHandler<T> faultHandler)
		{
			_workflowId = workflowId;
			_faultHandler = faultHandler;
			Current = new Workflow<T>(_faultHandler);
			_transitionRule = transitionRule;
			_transitions = new List<ITransition>();
		}

		public IWorkflow<T> Current { get; private set; }

		public void AddYield(object identifier)
		{
			ProcessDeferredStates(identifier);
			if (IsFirst)
				Current.Do(x => { _transitionRule.Begin(x, identifier); return x; });
			else
				Current.Do(x => { _transitionRule.Transition(x, identifier); return x; });

			AddFlow(identifier, Current);
			Current = new Workflow<T>(_faultHandler);
		}

		private void AddFlow(object key, IWorkflow<T> flow)
		{
			if (!_subflow_idx.ContainsKey(key))
			{
				if (_nextKey != null)
					_subflow_idx[_nextKey] = _subflows.Count;
				_subflows.Add(flow);
				AddTransition(_nextKey, key);
				_currentKey = _nextKey;
				_nextKey = key;
			}
		}

		/// <summary>
		/// indicates that this is the first workflow segment to be defined
		/// </summary>
		public bool IsFirst
		{
			get { return _subflows.Count == 0; }
		}

		public IEnumerable<ITransition> Transitions
		{
			get
			{
				EndDefinitionPhase();
				return _transitions; 
			}
		}

		public void EndDefinitionPhase()
		{
			ProcessDeferredStates(null);
			if (_nextKey != null)
			{
				if (!_subflow_idx.ContainsKey(_nextKey))
				{
					Current.Do(x => { _transitionRule.End(x); return x; });
					_subflow_idx[_nextKey] = _subflows.Count;
					_subflows.Add(Current);
					AddRemainingTransitions();
				}
			}
			else
				AddTransition(null, null);

			// free up some memory that won't be required anymore
			_undefinedForwardRefs = null;
			_definedRefs = null;
		}


		/// <summary>The first flow of the workflow</summary>
		public IWorkflow<T> First
		{
			get
			{
				if (_subflows.Count == 0)
					throw new InvalidOperationException("No workflow steps have been defined");
				return _subflows[0];
			}
		}

		public IWorkflow<T> GetCurrentFlowForObject(T data)
		{
			if (!data.IsAliveInWorkflow(_workflowId))
				return First;
			else
			{
				var state = data.GetStateId(_workflowId);
				if (_subflow_idx.ContainsKey(state))
					return _subflows[_subflow_idx[state]];
				else
					throw new InvalidOperationException("Object is not passing through workflow " + _workflowId);
			}
		}

		private void AddTransition(object from, object to)
		{
			if (_transitions != null)
			{
				var t = new Transition(_workflowId, from, to);
				if (!_transitions.Contains(t))
					_transitions.Add(t);
			}
		}

		private void AddTransition(object from, object to, IDeclaredOperation op)
		{
			AddTransition(from, to);
			if (_transitions != null)
				_allDefinedRefs[op] = to;
		}

		private void AddRemainingTransitions()
		{
			AddTransition(_nextKey, null);
		}

		/// <summary>
		/// Called when making decisions
		/// </summary>
		/// <param name="branchPoint"></param>
		public void AnalyzeTransitionPaths(IDeclaredOperation branchPoint)
		{
			if (_definedRefs.ContainsKey(branchPoint))
			{
				AddTransition(_nextKey, _definedRefs[branchPoint], branchPoint);
			}
			else
			{
				if (!_undefinedForwardRefs.ContainsKey(branchPoint))
					_undefinedForwardRefs[branchPoint] = new List<object>();
				_undefinedForwardRefs[branchPoint].Add(_nextKey);
			}
		}

		/// <summary>
		/// Called when .Define()'ing points
		/// </summary>
		/// <param name="branchPoint"></param>
		public void CreateDecisionPath(IDeclaredOperation branchPoint)
		{
			if (_definedRefs.ContainsKey(branchPoint))
				throw new InvalidOperationException("branch point is already defined. "
						+ "Cannot multiply define branch points");

			_definedRefs[branchPoint] = null;	// placeholder
			_deferredStates.Add(x => _definedRefs[branchPoint] = x);  // The real deal

			if (_undefinedForwardRefs.ContainsKey(branchPoint))
			{
				foreach (var key in _undefinedForwardRefs[branchPoint])
				{
					var placeholder = key;
					_deferredStates.Add(x => AddTransition(placeholder, x, branchPoint));
				}
				_undefinedForwardRefs.Remove(branchPoint);
			}
		}

		/// <summary>
		/// <para>
		/// This whole concept of deferring actions is just because we need to log the ENDING
		/// state ID as the destination of a transition. We usually don't know the ending 
		/// value when we're in the middle of processing, but we generally know what needs
		/// to happen (just waiting on the state ID). So we just defer...
		/// </para>
		/// <para>
		/// This whole class is complicated and this concept doesn't help
		/// </para>
		/// </summary>
		/// <param name="stateId"></param>
		private void ProcessDeferredStates(object stateId)
		{
			foreach (var action in _deferredStates)
				action(stateId);

			_deferredStates.Clear();
		}

		public object GetDestinationState(IDeclaredOperation to)
		{
			return _allDefinedRefs[to];
		}

		private IErrorHandler<T> _errorHandler;
		public IErrorHandler<T> ErrorHandler 
		{
			get { return _errorHandler; }
			set 
			{
				_errorHandler = value;
				foreach (var wf in this._subflows)
				{
					if (wf is Workflow<T>)
						((Workflow<T>)wf).ErrorHandler = value;
				}
			}
		}
	}
}
