using System;
using Rainbow.ObjectFlow.Stateful.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Stateful.Components;
using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Stateful
{
	public partial class StatefulWorkflow<T> : IConfigurationExpression<T>, ISecurityConfigurationExpresion<T>
	{
		/// <summary>
		/// Configure this workflow
		/// </summary>
		public IStatefulWorkflow<T> Configure(Action<IConfigurationExpression<T>> config)
		{
			return this;
		}

		#region IConfigurationExpression

		IConfigurationExpression<T> IConfigurationExpression<T>.TransitionRule(ITransitionRule<T> transitionRule)
		{
			throw new NotImplementedException();
		}

		IConfigurationExpression<T> IConfigurationExpression<T>.ErrorHandler(IErrorHandler<T> errorHandler)
		{
			throw new NotImplementedException();
		}

		ISecurityConfigurationExpresion<T> IConfigurationExpression<T>.Security
		{
			get { return this; }
		}

		#endregion

		#region ISecurityConfigurationExpresion

		IConfigurationExpression<T> ISecurityConfigurationExpresion<T>.UsingCustomService(ITransitionGateway gateway)
		{
			_gateway = gateway;
			return this;
		}

		ISecurityConfigurationExpresion<T> ISecurityConfigurationExpresion<T>.AsStrict
		{
			get 
			{
				_gateway = new StrictTransitionGateway(WorkflowId);
				return this;
			}
		}

		ISecurityConfigurationExpresion<T> ISecurityConfigurationExpresion<T>.AsRelaxed
		{
			get
			{
				_gateway = new RelaxedTransitionGateway(WorkflowId);
				return this;
			}
		}

		IConfigurationExpression<T> ISecurityConfigurationExpresion<T>.UsingMethod(Func<IEnumerable<ITransition>> transitionList)
		{
			GetAbstractGateway().TransitionList = transitionList;
			return this;
		}

		private AbstractTransitionGateway GetAbstractGateway()
		{
			if (_gateway == null)
				_gateway = new RelaxedTransitionGateway(WorkflowId);

			if (_gateway is AbstractTransitionGateway)
				return (AbstractTransitionGateway)_gateway;
			else
				throw new InvalidOperationException(string.Format("Expected Transition gateway to be either {0} or {1} but was {2}",
					typeof(RelaxedTransitionGateway), typeof(StrictTransitionGateway), _gateway.GetType()));
		}

		IConfigurationExpression<T> ISecurityConfigurationExpresion<T>.UsingListProvider(ITransitionListProvider listProvider)
		{
			GetAbstractGateway().TransitionListProvider = listProvider;
			return this;
		}

		#endregion
	}
}