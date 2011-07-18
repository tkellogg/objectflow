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
		/// use a transition gateway other than one of the default implementations (strict or relaxed)
		/// </summary>
		IConfigurationExpression<T> UsingCustomService(ITransitionGateway gateway);

		/// <summary>
		/// specify that we will be denying transitions on a white list basis
		/// </summary>
		ISecurityConfigurationExpresion<T> AsStrict { get; }

		/// <summary>
		/// specify that we will be denying transitions on a black list basis
		/// </summary>
		ISecurityConfigurationExpresion<T> AsRelaxed { get; }

		/// <summary>
		/// Use a method to generate the list of transitions that will be either white or black listed
		/// depending on if `AsRelaxed` or `AsStrict` is used
		/// </summary>
		/// <param name="transitionList">a method that returns generates the list</param>
		IConfigurationExpression<T> UsingMethod(Func<IEnumerable<ITransition>> transitionList);

		/// <summary>
		/// Use a list provider to generate the allowed/denied transitions for the current user
		/// </summary>
		/// <param name="listProvider"></param>
		/// <returns></returns>
		IConfigurationExpression<T> UsingListProvider(ITransitionListProvider listProvider);

	}
}
