using System;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Pipelines are composed of generic IOperations.  A pipeline
    /// controls the workflow whereas the IOperation encapsalates logic.
    /// </summary>
    /// <typeparam name="T">The type this pipeline contains</typeparam>
    public class Workflow<T> : IWorkflow<T> where T : class
    {
        /// <summary>
        /// Operations that have been added to the workflow definition
        /// </summary>
        internal IList<OperationConstraintPair<T>> RegisteredOperations;

        private WorkflowBuilder<T> _workflowBuilder;
        private IWorkflowBuilders<T> _workflowBuilderFactory;
        private WorkflowEngine<T> _workflowEngine;

        /// <summary>
        /// default constructor
        /// </summary>
        public Workflow()
        {
            Initialise();
        }

        private void Initialise()
        {
            _workflowBuilderFactory = new WorkflowBuilders<T>();
            _workflowBuilder = _workflowBuilderFactory.GetSequentialBuilder(this);
            _workflowEngine = new WorkflowEngine<T>();
            RegisteredOperations = new List<OperationConstraintPair<T>>();
        }

        internal Workflow(IWorkflowBuilders<T> factory)
        {
            Initialise();
            _workflowBuilderFactory = factory;
        }

        internal Workflow(WorkflowEngine<T> engine)
        {
            Initialise();
            _workflowEngine = engine;
        }

        /// <summary>
        /// Joins one operation onto another to run in parallel
        /// <remarks>
        /// Although data will be passed to operations or functions running in parallel 
        /// these operations will not affect the data for future operations.  This was to reduce the 
        /// complexity of parallel operations in Version 1.x and this behaviour may be extended to 
        /// pass state in the future.
        /// </remarks>
        /// </summary>
        public Workflow<T> And
        {
            get
            {
                if (IsSequentialBuilder(_workflowBuilder))
                {
                    _workflowBuilder = _workflowBuilderFactory.GetParallelBuilder(this);
                }

                return this;
            }
        }

        /// <summary>
        /// Adds an operation into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to execute as a generic of IOperation</param>
        /// <returns>this object to enable chaining</returns>
        /// <remarks>Operations must inherit from the BasicOperation class</remarks>        
        public virtual Workflow<T> Do(IOperation<T> operation)
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
        /// <returns>this object to enable chaining</returns>
        /// <remarks>Operations must inherit from the BasicOperation class</remarks>
        public virtual Workflow<T> Do(IOperation<T> operation, ICheckConstraint constraint)
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
        /// <returns>this</returns>
        public Workflow<T> Do(Func<T, T> function)
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
        /// <returns>this</returns>
        public Workflow<T> Do(Func<T, T> function, ICheckConstraint constraint)
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
        public virtual T Start()
        {
            Then();
            return _workflowEngine.Execute(RegisteredOperations);
        }

        /// <summary>
        /// Runs the workflow definition
        /// </summary>
        /// <param name="data">data to transform</param>
        /// <returns>Result of the workflow</returns>
        public virtual T Start(T data)
        {
            Then();
            return _workflowEngine.Execute(RegisteredOperations, data);
        }

        /// <summary>
        /// Subsequent Operations and functions will execute after the previous one has completed.
        /// </summary>
        /// <returns></returns>
        public IWorkflow<T> Then()
        {
            if (!IsSequentialBuilder(_workflowBuilder))
            {
                RegisteredOperations.Add(new OperationConstraintPair<T>(_workflowBuilder.ParallelOperations));
                _workflowBuilder = _workflowBuilderFactory.GetSequentialBuilder(this);
            }

            return this;
        }

        private static bool IsSequentialBuilder(WorkflowBuilder<T> builder)
        {
            return null == builder || (typeof(SequentialBuilder<T>) == builder.GetType());
        }

        /// <summary>
        /// Instantiates a workflow object
        /// </summary>
        /// <returns>Object of IWorkflow of T</returns>
        public static IWorkflow<T> Definition()
        {
            return new Workflow<T>();
        }

        /// <summary>
        /// Adds a sub-workflow into the workflow definition
        /// </summary>
        /// <param name="workflow">Workflow to add</param>
        /// <returns>this</returns>
        public Workflow<T> Do(IWorkflow<T> workflow)
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
        /// <returns>this</returns>
        public Workflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            Check.IsNotNull(workflow, "workflow");
            Check.IsNotNull(constraint, "constraint");

            _workflowBuilder.AddOperation(workflow, constraint);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IRetryPolicy Retry()
        {
            IRetryPolicy policy = new Retry();
            if (RegisteredOperations.Count > 0)
            {
                RegisteredOperations[RegisteredOperations.Count - 1].Command.Policies.Add(policy);
            }

            return policy;
        }
    }
}