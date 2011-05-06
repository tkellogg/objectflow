using System;
namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// Extension point to rewrite the logic for how objects transition 
	/// through workflows. This controls how state ID's are set as well
	/// as providing indicators about state
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ITransitionRule<T> : IStateObserver<T>
	 where T : class, IStatefulObject
	{
		/// <summary>
		/// Sets the state ID as it begins the workflow
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="endState">The state the entity is transitioning into</param>
		void Begin(T entity, object endState);

		/// <summary>
		/// Sets the state ID as it exits the workflow
		/// </summary>
		/// <param name="entity"></param>
		void End(T entity);

		/// <summary>
		/// Transition the object between states
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="endState">The state the entity is transitioning into</param>
		void Transition(T entity, object endState);
	}
}
