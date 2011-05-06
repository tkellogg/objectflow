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
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <typeparam name="T2">Extra parameter used from start</typeparam>
		/// <typeparam name="T3">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		IStatefulWorkflow<T> Do<T1, T2, T3>(Action<T, T1, T2, T3> body);
		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <typeparam name="T2">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		IStatefulWorkflow<T> Do<T1, T2>(Action<T, T1, T2> body);
		/// <summary>
		/// Adds a function into the execution path
		/// </summary>
		/// <typeparam name="T1">Extra parameter used from start</typeparam>
		/// <param name="body">The function to add</param>
		IStatefulWorkflow<T> Do<T1>(Action<T, T1> body);

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
		T Start<T1, T2, T3>(T subject, T1 arg1, T2 arg2, T3 arg3);
		/// <summary>
		/// Start the workflow with extra arguments
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="subject">Object being operated on</param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		T Start<T1, T2>(T subject, T1 arg1, T2 arg2);
		/// <summary>
		/// Start the workflow with extra arguments
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="subject">Object being operated on</param>
		/// <param name="arg1"></param>
		T Start<T1>(T subject, T1 arg1);

		/// <summary>
		/// Starts the workflow segment with the given parameters
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		T StartWithParams(T subject, params object[] parameters);

		#endregion

        /// <summary>
        /// When function returns true, branch to the specified operation
        /// </summary>
        /// <param name="function"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        IStatefulWorkflow<T> When(Func<T, bool> function, IDeclaredOperation otherwise);
        
        /// <summary>
        /// When function returns false, branch to the specified operation
        /// </summary>
        /// <param name="function"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        IStatefulWorkflow<T> Unless(Func<T, bool> function, IDeclaredOperation otherwise);

        /// <summary>
        /// Declare a point that you may wish to branch to later
        /// </summary>
        /// <param name="defineAs"></param>
        /// <returns></returns>
        IStatefulWorkflow<T> Define(IDeclaredOperation defineAs);

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
