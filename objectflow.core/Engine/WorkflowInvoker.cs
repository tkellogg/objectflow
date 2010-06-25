using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class WorkflowInvoker<T> : MethodInvoker<T> where T : class
    {
        private IWorkflow<T> _workflow;

        public WorkflowInvoker(IWorkflow<T> workflow)
        {
            _workflow = workflow;
        }

        public override T Execute(T data)
        {
            return _workflow.Start(data);
        }
    }

}
