using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Engine
{
	/// <summary>
	/// When we were using the ServiceLocator, it created a new WorkflowBuilder 
	/// whenever Resolve was called
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TBuilder"></typeparam>
	internal abstract class BuilderActivator<T, TBuilder>
		where T : class
		where TBuilder : WorkflowBuilder<T>
	{
		private TaskList<T> _taskList;

		public BuilderActivator() { }

		public BuilderActivator(TaskList<T> taskList)
		{
			_taskList = taskList;
		}

		public virtual TBuilder Activate()
		{
			var ctor = typeof(TBuilder).GetConstructor(new[]{typeof(TaskList<T>)});
			var ret = (TBuilder)ctor.Invoke(new[] { _taskList });
			// Apparently we need a new task list each time this is created. The only reason
			// we take a task list in the ctor is because some unit tests require a reference
			_taskList = new TaskList<T>();
			return ret;
		}
	}

	/// <summary>
	/// Creates instances of the sequential builder
	/// </summary>
	internal class SequentialBuilderActivator<T> : BuilderActivator<T, SequentialBuilder<T>>
		where T:class
	{
		public SequentialBuilderActivator(TaskList<T> taskList) : base(taskList) { }
		[Obsolete("This is only used by Moq", true)]
		public SequentialBuilderActivator() : base() { }
	}

	internal class ParallelBuilderActivator<T> : BuilderActivator<T, ParallelSplitBuilder<T>>
		where T : class
	{
		public ParallelBuilderActivator(TaskList<T> taskList) : base(taskList) { }
		[Obsolete("This is only used by Moq", true)]
		public ParallelBuilderActivator() : base() { }
	}
}
