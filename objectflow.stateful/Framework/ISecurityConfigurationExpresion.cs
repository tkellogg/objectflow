using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	/// <summary>
	/// </summary>
	public interface ISecurityConfigurationExpresion<T>
		where T : class, IStatefulObject
	{
		/// <summary>
		/// </summary>
		IConfigurationExpression<T> UsingService(ITransitionGateway gateway);

		/// <summary>
		/// </summary>
		ISecurityConfigurationExpresion<T> AsStrict { get; }
		/// <summary>
		/// </summary>
		ISecurityConfigurationExpresion<T> AsRelaxed { get; }

		/// <summary>
		/// </summary>
		IConfigurationExpression<T> UsingMethod(Func<IEnumerable<ITransition>> transitionList);
	}
}
