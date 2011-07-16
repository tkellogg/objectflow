using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	/// <summary>
	/// Enumerates the ways that an action can fail
	/// </summary>
	public interface IFailureExpression<T>
		where T : class, IStatefulObject
	{
		/// <summary>
		/// Fails with <see cref="WorkflowActionFailedException"/> and a message
		/// </summary>
		void With(string message, params object[] format);

		/// <summary>
		/// Fails with <see cref="WorkflowActionFailedException"/> and a message
		/// </summary>
		void With(Func<T, string> builder);

		/// <summary>
		/// Fails with <see cref="WorkflowActionFailedException"/> and a message
		/// </summary>
		void With(Func<T, IDictionary<string, object>, string> builder);

		/// <summary>
		/// Fails with a custom exception
		/// </summary>
		void With<TException>(Func<T, TException> builder)
			where TException : Exception;

		/// <summary>
		/// Fails with a custom exception
		/// </summary>
		void With<TException>(Func<T, IDictionary<string, object>, TException> builder)
			where TException : Exception;
	}
}
