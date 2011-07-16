using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	/// <summary>
	/// Represents a series of choices that are a result of a condition
	/// </summary>
	public interface IBranchingExpression<T>
		where T : class, IStatefulObject
	{
		/// <summary>
		/// Break out of the workflow execution immediately without setting state
		/// </summary>
		IStatefulWorkflow<T> Break();

		/// <summary>
		/// Break out of the workflow execution immediately, setting the given state
		/// </summary>
		/// <param name="status">The state to give the object as it's exiting</param>
		IStatefulWorkflow<T> BreakWithStatus(object status);

		/// <summary>
		/// Throw a <see cref="WorkflowActionFailedException"/> which causes execution
		/// of the workflow to halt immediately
		/// </summary>
		IStatefulWorkflow<T> Fail();

		/// <summary>
		/// Throw a <see cref="WorkflowActionFailedException"/> with the given message
		/// which causes execution of the workflow to halt immediately
		/// </summary>
		IStatefulWorkflow<T> Fail(Action<IFailureExpression<T>> failure);

		/// <summary>
		/// Go to the specified branch location if the condition is true. Otherwise keep executing
		/// </summary>
		IStatefulWorkflow<T> BranchTo(IDeclaredOperation location);

		/// <summary>
		/// Specify a set of workflow steps that happen conditionally. 
		/// </summary>
		/// <remarks>
		///	lets us get back to more of a block structure. Also a much simpler way of doing 
		///	branching without actually thinking in terms of labels/gotos
		/// </remarks>
		/// <param name="workflowSteps"></param>
		IStatefulWorkflow<T> Do(Action<IStatefulWorkflow<T>> workflowSteps);

	}
}
