using System;
using Castle.MicroKernel;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;
using Rainbow.ObjectFlow.Policies;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Pipelines are composed of generic IOperations.  A pipeline
    /// controls the workflow whereas the IOperation encapsalates logic.
    /// </summary>
    public class Workflow<T> : IWorkflow<T>, IDefine<T>, ICompose<T>, IMerge<T> where T : class
    {
        private WorkflowBuilder<T> _workflowBuilder;
        private readonly WorkflowEngine<T> _workflowEngine;
        private readonly IKernel _container;

        /// <summary>
        /// default constructor
        /// </summary>
        public Workflow()
        {
            _container = ServiceLocator<T>.Get();
            _workflowEngine = _container.Resolve<WorkflowEngine<T>>();            
            _workflowBuilder = _container.Resolve<SequentialBuilder<T>>();
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
        public ICompose<T> And
        {
            get
            {
                if (IsSequentialBuilder(_workflowBuilder))
                {
                    OperationDuplex<T>[] array = GetArrayFromCurrentTasks();
                    _workflowBuilder = _container.Resolve<ParallelSplitBuilder<T>>();
                    SetParallelOperationsFromList(array);
                }

                return this;
            }
        }

        private void SetParallelOperationsFromList(OperationDuplex<T>[] array)
        {
            foreach (var duplex in array)
            {
                _workflowBuilder.ParallelOperations.Add(duplex);
            }
        }

        private OperationDuplex<T>[] GetArrayFromCurrentTasks()
        {
            var array = new OperationDuplex<T>[_workflowBuilder.TaskList.Tasks.Count];
            _workflowBuilder.TaskList.Tasks.CopyTo(array, 0);
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
            get { return _workflowBuilder.TaskList; }
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

            _workflowBuilder.AddOperation(operation);

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

            _workflowBuilder.AddOperation(operation, constraint);

            return this;
        }

        /// <summary>
        /// Adds a function into the workflow definition
        /// </summary>
        /// <param name="function">Function to add</param>
        public IWorkflow<T> Do(Func<T, T> function)
        {
            Check.IsNotNull(function, "function");

            _workflowBuilder.AddOperation(function);
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

            _workflowBuilder.AddOperation(function, constraint);
            return this;
        }

        /// <summary>
        /// Adds a function that returns its' success result into the execution path
        /// <remarks>
        /// The function returns the success result as a bool (True for success) to enable functions to be used in the evaluation of future contraints
        /// </remarks>
        /// </summary>
        /// <param name="function">The function to add</param>
        public IWorkflow<T> Do(Func<bool> function)
        {
            Check.IsNotNull(function, "function");

            _workflowBuilder.AddOperation(function);
            return this;
        }

        /// <summary>
        /// Adds a function that returns its' success result into the execution path
        /// <remarks>
        /// The function returns the success result as a bool (True for success) to enable functions to be used in the evaluation of future contraints
        /// </remarks>
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <param name="constraint">The condition whose evaluation determines if the workflow is executed</param>
        public IWorkflow<T> Do(Func<bool> function, ICheckConstraint constraint)
        {
            Check.IsNotNull(function, "function");
            Check.IsNotNull(constraint, "constraint");

            _workflowBuilder.AddOperation(function, constraint);

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
        public T Start()
        {
            Then();
            _workflowEngine.Execute( _workflowBuilder.TaskList.Tasks);
            return Context;
        }

        /// <summary>
        /// Runs the workflow definition
        /// </summary>
        /// <param name="data">Data to transform</param>
        /// <returns>Result of the workflow</returns>
        public virtual T Start(T data)
        {
            Then();
            _workflowEngine.Execute(_workflowBuilder.TaskList.Tasks, data);
            return Context;
        }

        /// <summary>
        /// Subsequent Operations and functions will execute after the previous one has completed.
        /// </summary>
        public virtual IMerge<T> Then()
        {
            if (!IsSequentialBuilder(_workflowBuilder))
            {
                 _workflowBuilder.TaskList.Tasks.Add(new OperationDuplex<T>(_workflowBuilder.ParallelOperations));
                 OperationDuplex<T>[] array = GetArrayFromCurrentTasks();

                _workflowBuilder = _container.Resolve<SequentialBuilder<T>>();
                SetCurrentTasksFromList(array);
            }

            return this;
        }

        private void SetCurrentTasksFromList(OperationDuplex<T>[] array)
        {
            foreach (var duplex in array)
            {
                _workflowBuilder.TaskList.Tasks.Add(duplex);
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
            _workflowBuilder.AddOperation(workflow);

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

            _workflowBuilder.AddOperation(workflow, constraint);

            return this;
        }

        /// <summary>
        /// Allows an operation to be attempted again
        /// </summary>
        public IRetryPolicy Retry()
        {
            IRetryPolicy policy = new Retry();
            if (_workflowBuilder.TaskList.Tasks.Count > 0)
            {
                _workflowBuilder.TaskList.Tasks[_workflowBuilder.TaskList.Tasks.Count - 1].Command.Policies.Add(policy);
            }

            return policy;
        }

        /// <summary>
        /// Registers an instance of the type specified in the workflow
        /// </summary>
        /// <typeparam name="TOperation">Type that inherits from BasicOperation of T</typeparam>
        public IWorkflow<T> Do<TOperation>() where TOperation : BasicOperation<T>
        {
            _workflowBuilder.AddOperation<TOperation>();

            return this;
        }

        /// <summary>
        /// Registers an instance of the type specified in the workflow
        /// </summary>
        /// <typeparam name="TOperation">Type that inherits from BasicOperation of T</typeparam>
        /// <param name="constraint">The constraint to evaluate</param>
        public IWorkflow<T> Do<TOperation>(ICheckConstraint constraint) where TOperation : BasicOperation<T>
        {
            _workflowBuilder.AddOperation<TOperation>(constraint);

            return this;
        }
    }
}