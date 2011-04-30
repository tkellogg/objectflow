using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Engine
{

	internal abstract class BuilderActivator<T, TBuilder>
		where T : class
		where TBuilder : WorkflowBuilder<T>
	{
		private TaskList<T> _taskList;

		[Obsolete("This is only used by Moq", true)]
		public BuilderActivator()
		{
		}

		public BuilderActivator(TaskList<T> taskList)
		{
			_taskList = taskList;
		}

		public virtual TBuilder Activate()
		{
			var ctor = typeof(TBuilder).GetConstructor(new[]{typeof(TaskList<T>)});
			return (TBuilder)ctor.Invoke(new[] { _taskList });
		}
	}

	/// <summary>
	/// Creates instances of the sequential builder
	/// </summary>
	internal class SequentialBuilderActivator<T> : BuilderActivator<T, SequentialBuilder<T>>
		where T:class
	{
		public SequentialBuilderActivator(TaskList<T> taskList) : base(taskList) { }
	}

	internal class ParallelBuilderActivator<T> : BuilderActivator<T, ParallelSplitBuilder<T>>
		where T : class
	{
		public ParallelBuilderActivator(TaskList<T> taskList) : base(taskList) { }
	}
}
