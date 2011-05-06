using System;
namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// Can be queried for information about where an object
	/// is within a workflow
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IStateObserver<T>
	 where T : class, IStatefulObject
	{
		/// <summary>
		/// If <see cref="IsInWorkflow"/> is false, this indicates that the entity has been
		/// in a workflow at least once. This returns null if this information can't be determined.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool? HasBeenInWorkflow(T entity);

		/// <summary>
		/// True if the entity is currently passing through the workflow
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool IsInWorkflow(T entity);
	}
}
