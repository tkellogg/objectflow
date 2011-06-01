using System;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Engine
{
    internal class SequentialBuilder<T> : WorkflowBuilder<T> where T : class
    {

        public SequentialBuilder(TaskList<T> taskList) : base(taskList)
        {
        }

        public override void AddOperation(IOperation<T> operation)
        {
            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation as BasicOperation<T>));
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation(IOperation<T> operation, ICheckConstraint constraint)
        {
            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation as BasicOperation<T>), constraint);            
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function)
        {
            var operationPair = new OperationDuplex<T>(new FunctionInvoker<T>(function));
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function, IDeclaredOperation name)
        {
            InitializeDeclaredOperation(name);
            var operationPair = new OperationDuplex<T>(new FunctionInvoker<T>(function), null);
            taskList.Tasks.Add(operationPair);
        }

        private void InitializeDeclaredOperation(IDeclaredOperation name)
        {
            if (name == null)
                throw new ArgumentException("argument hasn't been initialized");
            name.SetTasks((System.Collections.IList)taskList.Tasks);
        }

        public override void AddOperation(Func<T, T> function, ICheckConstraint constraint)
        {
            var operationPair = new OperationDuplex<T>(new FunctionInvoker<T>(function), constraint);
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation(Func<T, T> function, ICheckConstraint constraint, IDeclaredOperation name)
        {
            InitializeDeclaredOperation(name);
            var operationPair = new OperationDuplex<T>(new FunctionInvoker<T>(function), constraint);
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation(IWorkflow<T> workflow)
        {
            var operationPair = new OperationDuplex<T>(new WorkflowInvoker<T>(workflow));
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation(IWorkflow<T> workflow, ICheckConstraint constraint)
        {
            var operationPair = new OperationDuplex<T>(new WorkflowInvoker<T>(workflow), constraint);
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation<TType>()
        {
            var operation = Activator.CreateInstance<TType>();
            
            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation as BasicOperation<T>));
            taskList.Tasks.Add(operationPair);
        }

        public override void AddOperation<TOperation>(ICheckConstraint constraint)
        {
            var operation = Activator.CreateInstance<TOperation>();

            var operationPair = new OperationDuplex<T>(new OperationInvoker<T>(operation as BasicOperation<T>), constraint);
            taskList.Tasks.Add(operationPair);
        }

		public override void AddOperation(Action<T, IDictionary<string, object>> function)
		{
			var op = new ParameterizedOperationDuplex<T>(new ParameterizedFunctionInvoker<T>(function));
			taskList.Tasks.Add(op);
		}
	}
}