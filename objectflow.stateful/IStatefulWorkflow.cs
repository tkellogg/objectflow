using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// A workflow that is able to pause at states and be resumed later.
	/// </summary>
	/// <typeparam name="T">The object that will be stepped through the workflow</typeparam>
	public interface IStatefulWorkflow<T> : IWorkflow<T>, IStateObserver<T>
		where T : class, IStatefulObject
	{
		/// <summary>
		/// Gets an identifier that describes the workflow. This can be a string,
		/// number, Guid, or any other object that provides a meaningful implementation
		/// of the <c>.Equals(object)</c> method.
		/// </summary>
		object WorkflowId { get; }

		/// <summary>
		/// Signal that the workflow should pause here and then resume later
		/// at this same point. This can be used to persist the state of the 
		/// object and resumed later when <c>Continue</c> is called.
		/// </summary>
		/// <param name="breakPointId">The identifier that describes where the workflow
		/// yielded so that it can return to this point later.</param>
		/// <returns></returns>
		IStatefulWorkflow<T> Yield(object breakPointId);

		#region overriden from base class, but returning IStatefulWorkflow

		/// <summary>
		/// Registers an instance of the specified type in the workflow
		/// </summary>
		new IStatefulWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>;

		/// <summary>
		/// Registers an instance of the specified type in the workflow
		/// </summary>
		new IStatefulWorkflow<T> Do<TOperation>(ICheckConstraint constraint) where TOperation : BasicOperation<T>;

		/// <summary>
		/// Adds operations into the workflow definition
		/// </summary>
		/// <param name="operation">The operation to add</param>
		new IStatefulWorkflow<T> Do(IOperation<T> operation);

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="function">The function to add</param>
		new IStatefulWorkflow<T> Do(Func<T, T> function);

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="function">The funciton to add</param>
		/// <param name="constraint">constraint that determines if the operation is executed</param>
		new IStatefulWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint);

		/// <summary>
		/// Adds an operation into the execution path 
		/// </summary>
		/// <param name="operation">operatio to add</param>
		/// <param name="constraint">constraint that determines if the operation is executed</param>
		new IStatefulWorkflow<T> Do(IOperation<T> operation, ICheckConstraint constraint);

		/// <summary>
		/// Adds a sub-workflow into the execution path
		/// </summary>
		/// <param name="workflow">The function to add</param>
		new IStatefulWorkflow<T> Do(IWorkflow<T> workflow);

		/// <summary>
		/// Adds a sub-workflow into the execution path
		/// </summary>
		/// <param name="workflow">The funciton to add</param>
		/// <param name="constraint">constraint that determines if the workflow is executed</param>
		new IStatefulWorkflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint);
		#endregion

		#region Similar extensions of IWorkflow

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="function">The function to add</param>
		IStatefulWorkflow<T> Do(Action<T> function);

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="function">The function to add</param>
		/// <param name="branch">Branch point to initialize</param>
		IStatefulWorkflow<T> Do(Action<T> function, IDeclaredOperation branch);

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="function">The function to add</param>
		/// <param name="constraint">constraint that determines if the operation is executed</param>
		IStatefulWorkflow<T> Do(Action<T> function, ICheckConstraint constraint);

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="function">The function to add</param>
		/// <param name="constraint">constraint that determines if the operation is executed</param>
		/// <param name="defineAs">Branch point to initialize</param>
		IStatefulWorkflow<T> Do(Action<T> function, ICheckConstraint constraint, IDeclaredOperation defineAs);

		#region Overloads of `Do(Action<...>)` and `Start(T, ...)`

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="body">The function to add</param>
		IStatefulWorkflow<T> Do(Action<T, IDictionary<string, object>> body);

		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <param name="body">The function to add</param>
		IStatefulWorkflow<T> Do(Func<T, IDictionary<string, object>, T> body);

		/// <summary>
		/// Starts the workflow segment with the given parameters
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		T StartWithParams(T subject, IDictionary<string, object> parameters);

		/// <summary>
		/// Starts the workflow segment with the given parameters
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="asParameters"></param>
		/// <returns></returns>
		T StartWithParams(T subject, object asParameters);

		#endregion

		/// <summary>
		/// Declare a point that you may wish to branch to later
		/// </summary>
		/// <param name="defineAs"></param>
		/// <returns></returns>
		IStatefulWorkflow<T> Define(IDeclaredOperation defineAs);

		/// <summary>
		/// If the condition is true
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		Rainbow.ObjectFlow.Stateful.Framework.IBranchingExpression<T> When(Predicate<T> condition);

		/// <summary>
		/// If the condition is true
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		Rainbow.ObjectFlow.Stateful.Framework.IBranchingExpression<T> When(Func<T, IDictionary<string, object>, bool> condition);

		/// <summary>
		/// If the condition is true
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		Rainbow.ObjectFlow.Stateful.Framework.IBranchingExpression<T> Unless(Predicate<T> condition);

		/// <summary>
		/// If the condition is true
		/// </summary>
		/// <param name="condition">if true, execute the specified action</param>
		Rainbow.ObjectFlow.Stateful.Framework.IBranchingExpression<T> Unless(Func<T, IDictionary<string, object>, bool> condition);

		#endregion

		/// <summary>
		/// <para>
		/// Get an enumeration of only possible transitions of this workflow. Usage
		/// of this method before workflow definition is finished is meaningless, so
		/// refrain from calling this until the workflow is entirely defined to a 
		/// point that you could call <c>.Start()</c>
		/// </para>
		/// <para>
		/// If an ITransitionGateway instance has not been supplied to this object 
		/// (i.e. in the constructor), this property may return null or throw an 
		/// exception. This enumeration is unordered, no code should rely on the 
		/// ordering being the same between versions or even between calls.
		/// </para>
		/// </summary>
		IEnumerable<ITransition> PossibleTransitions { get; }
	}
}
