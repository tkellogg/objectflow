using Rainbow.ObjectFlow.Engine;

namespace Rainbow.ObjectFlow.Interfaces
{
    internal interface IWorkflowBuilders<T> where T : class
    {
        WorkflowBuilder<T> GetSequentialBuilder(IWorkflow<T> workflow);

        WorkflowBuilder<T> GetParallelBuilder(IWorkflow<T> workflow);
    }
}