using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	/// <summary>
	/// Describes the interface for configuring a workflow
	/// </summary>
	public interface IConfigurationExpression<T>
		where T : class, IStatefulObject
	{
		/// <summary>
		/// Setup the transition rule
		/// </summary>
		IConfigurationExpression<T> TransitionRule(ITransitionRule<T> transitionRule);

		/// <summary>
		/// Setup the 
		/// </summary>
		IConfigurationExpression<T> ErrorHandler(IErrorHandler<T> errorHandler);
		
		/// <summary>
		/// Configure the security gateway using a complete custom provider
		/// </summary>
		ISecurityConfigurationExpresion<T> Security { get; }
	}
}
