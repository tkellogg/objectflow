﻿using System;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;
using Rainbow.ObjectFlow.Policies;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Constraints;
using System.Collections;
using System.Diagnostics;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Pipelines are composed of generic IOperations.  A pipeline
    /// controls the workflow whereas the IOperation encapsalates logic.
    /// </summary>
    public class Workflow<T> : IWorkflow<T>, IDefine<T>, ICompose<T>, IMerge<T>, IExecuteWorkflow<T> where T : class
    {
		/// <summary>
		/// Builder that this workflow is currently using
		/// </summary>
		public WorkflowBuilder<T> WorkflowBuilder { get; private set; }
		internal readonly Dispatcher<T> _workflowEngine;
		private ParallelBuilderActivator<T> _parallelBuilder;
		private SequentialBuilderActivator<T> _sequentialBuilder;

        /// <summary>
        /// default constructor
        /// </summary>
        public Workflow()
			:this(new TaskList<T>())
        {
        }

		internal Workflow(TaskList<T> taskList)
			:this(new Dispatcher<T>(), new SequentialBuilderActivator<T>(taskList), new ParallelBuilderActivator<T>(new TaskList<T>()))
		{
		}

		internal Workflow(Dispatcher<T> dispatcher, SequentialBuilderActivator<T> sequentialBuilder, ParallelBuilderActivator<T> parallelBuilder)
		{
			WorkflowBuilder = sequentialBuilder.Activate();
			_workflowEngine = dispatcher;
			_sequentialBuilder = sequentialBuilder;
			_parallelBuilder = parallelBuilder;
		}

        /// <summary>
        /// Create a workflow with a default error handler class.
        /// </summary>
        /// <param name="errorHandler"></param>
        public Workflow(IErrorHandler<T> errorHandler)
            :this()
        {
            _workflowEngine.ErrorHandler = errorHandler;
        }

        /// <summary>
        /// Joins one operation onto another to run in parallel
        /// <remarks>
        /// Although data will be passed to operations or functions running in parallel 
        /// these operations will not affect the data passed to subsequent operations.  This was to reduce the 
        /// complexity of parallel operations in Version 1.x and this behaviour may be extended to 
        /// pass state in the future.
        /// </remarks>
        /// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ICompose<T> And
        {
            get
            {
                if (IsSequentialBuilder(WorkflowBuilder))
                {
                    OperationDuplex<T>[] array = GetArrayFromCurrentTasks();
					WorkflowBuilder = _parallelBuilder.Activate();
                    SetParallelOperationsFromList(array);
                }

                return this;
            }
        }

        private void SetParallelOperationsFromList(OperationDuplex<T>[] array)
        {
            foreach (var duplex in array)
            {
                WorkflowBuilder.ParallelOperations.Add(duplex);
            }
        }

        private OperationDuplex<T>[] GetArrayFromCurrentTasks()
        {
            var array = new OperationDuplex<T>[WorkflowBuilder.Tasks.Tasks.Count];
            WorkflowBuilder.Tasks.Tasks.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Gets or sets the context of the workflow.
        /// </summary>
        /// <value>The context for the workflow operations.</value>
        public T Context
        {
            get { return _workflowEngine.Context; }
            set { _workflowEngine.Context = value; }
        }

        internal TaskList<T> RegisteredOperations
        {
            get { return WorkflowBuilder.Tasks; }
        }

        /// <summary>
        /// Adds an operation into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to execute as a generic of IOperation</param>
        /// <remarks>Operations must inherit from the BasicOperation class</remarks>        
        public virtual IWorkflow<T> Do(IOperation<T> operation)
        {
            Check.IsNotNull(operation, "operation");
            Check.IsInstanceOf<BasicOperation<T>>(operation, "operation");

            WorkflowBuilder.AddOperation(operation);

            return this;
        }

        /// <summary>
        /// Adds an operation into the workflow definition given the constraint.  The constraint runs if
        /// the constraint evaluates to true.
        /// </summary>
        /// <param name="operation">The operation to execute as a generic of IOperation</param>
        /// <param name="constraint">The constraint to evaluate</param>
        /// <remarks>Operations must inherit from the BasicOperation class</remarks>
        public virtual IWorkflow<T> Do(IOperation<T> operation, ICheckConstraint constraint)
        {
            Check.IsNotNull(operation, "operation");
            Check.IsNotNull(constraint, "constraint");
            Check.IsInstanceOf<BasicOperation<T>>(operation, "operation");

            WorkflowBuilder.AddOperation(operation, constraint);

            return this;
        }

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        /// <param name="branchPoint"></param>
        public virtual IWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint, IDeclaredOperation branchPoint)
        {
            Check.IsNotNull(function, "function");
            Check.IsNotNull(constraint, "constraint");

            WorkflowBuilder.AddOperation(function, constraint, branchPoint);
            return this;
        }

        /// <summary>
        /// Adds a function into the workflow definition
        /// </summary>
        /// <param name="function">Function to add</param>
        public IWorkflow<T> Do(Func<T, T> function)
        {
            Check.IsNotNull(function, "function");

            WorkflowBuilder.AddOperation(function);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="branch"></param>
        /// <returns></returns>
        public IWorkflow<T> Do(Func<T,T> function, IDeclaredOperation branch)
        {
            Check.IsNotNull(function, "function");

            WorkflowBuilder.AddOperation(function, branch);
            return this;
        }

        /// <summary>
        /// Adds a function into the workflow definition
        /// </summary>
        /// <param name="function">Function to add</param>
        /// <param name="constraint">constraint defines if function will be executed</param>
        public IWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint)
        {
            Check.IsNotNull(function, "function");
            Check.IsNotNull(constraint, "constraint");

            WorkflowBuilder.AddOperation(function, constraint);
            return this;
        }

        /// <summary>
        /// Begins the execution of a workflow
        /// </summary>
        /// <returns>The data after being transformed by the Operation objects</returns>
        /// <remarks>
        /// The concrete implementation is responsible for definng how the data is passed to the pipeline.
        /// A common implementation is to use a constructor and an operation that returns the data with no transformations
        /// </remarks>
        public virtual T Start()
        {
			try {
				Then();
				_workflowEngine.Execute(WorkflowBuilder.Tasks.Tasks);
			}
			catch (EarlyExitException) { }
            return Context;
        }

        /// <summary>
        /// Runs the workflow definition
        /// </summary>
        /// <param name="data">Data to transform</param>
        /// <returns>Result of the workflow</returns>
        public virtual T Start(T data)
        {
			try {
				Then();
				_workflowEngine.Execute(WorkflowBuilder.Tasks.GenerateTaskList(), data);
			}
			catch (EarlyExitException) { }
            return Context;
        }

        /// <summary>
        /// Subsequent Operations and functions will execute after the previous one has completed.
        /// </summary>
        public virtual IMerge<T> Then()
        {
            if (!IsSequentialBuilder(WorkflowBuilder))
            {
                WorkflowBuilder.Tasks.Tasks.Add(new OperationDuplex<T>(WorkflowBuilder.ParallelOperations));
                OperationDuplex<T>[] array = GetArrayFromCurrentTasks();

				WorkflowBuilder = _sequentialBuilder.Activate();
				SetCurrentTasksFromList(array);
            }

            return this;
        }

        private void SetCurrentTasksFromList(OperationDuplex<T>[] array)
        {
            foreach (var duplex in array)
            {
                WorkflowBuilder.Tasks.Tasks.Add(duplex);
            }
        }

        private static bool IsSequentialBuilder(WorkflowBuilder<T> builder)
        {
            return null == builder || (typeof(SequentialBuilder<T>) == builder.GetType());
        }

        /// <summary>
        /// Instantiates a workflow object
        /// </summary>
        public static IDefine<T> Definition()
        {
            return new Workflow<T>();
        }

        /// <summary>
        /// Adds a sub-workflow into the workflow definition
        /// </summary>
        /// <param name="workflow">Workflow to add</param>
        public IWorkflow<T> Do(IWorkflow<T> workflow)
        {
            Check.IsNotNull(workflow, "workflow");
            WorkflowBuilder.AddOperation(workflow);

            return this;
        }

        /// <summary>
        /// Adds a sub-workflow into the workflow definition
        /// </summary>
        /// <param name="workflow">Workflow to add</param>
        /// <param name="constraint">pre-condition for execution</param>
        public IWorkflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            Check.IsNotNull(workflow, "workflow");
            Check.IsNotNull(constraint, "constraint");

            WorkflowBuilder.AddOperation(workflow, constraint);

            return this;
        }

        /// <summary>
        /// Allows an operation to be attempted again
        /// </summary>
        public IRetryPolicy Retry()
        {
            IRetryPolicy policy = new Retry(this);
            if (WorkflowBuilder.Tasks.Tasks.Count > 0)
            {
                WorkflowBuilder.Tasks.Tasks[WorkflowBuilder.Tasks.Tasks.Count - 1].Command.Policies.Add(policy);
            }

            return policy;
        }

        /// <summary>
        /// Does the operation again.  Unlike Retry, this does not check on success or failure
        /// </summary>
        /// <returns></returns>
        public IRepeat Repeat()
        {
            IRepeat policy= new Repeat(this);
            if (WorkflowBuilder.Tasks.Tasks.Count > 0)
            {
                WorkflowBuilder.Tasks.Tasks[WorkflowBuilder.Tasks.Tasks.Count - 1].Command.Policies.Add(policy);
            }

            return policy;
        }

        /// <summary>
        /// Registers an instance of the type specified in the workflow
        /// </summary>
        /// <typeparam name="TOperation">Type that inherits from BasicOperation of T</typeparam>
        public IWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>
        {
            WorkflowBuilder.AddOperation<TOperation>();

            return this;
        }

        /// <summary>
        /// Registers an instance of the type specified in the workflow
        /// </summary>
        /// <typeparam name="TOperation">Type that inherits from BasicOperation of T</typeparam>
        /// <param name="constraint">The constraint to evaluate</param>
        public IWorkflow<T> Do<TOperation>(ICheckConstraint constraint) where TOperation : BasicOperation<T>
        {
            WorkflowBuilder.AddOperation<TOperation>(constraint);

            return this;
        }
    }
}