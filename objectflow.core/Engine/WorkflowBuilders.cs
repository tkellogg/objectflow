using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Engine
{
    internal class WorkflowBuilders<T> : IWorkflowBuilders<T> where T : class
    {
        public virtual WorkflowBuilder<T> GetSequentialBuilder(IWorkflow<T> workflow)
        {
            return new SequentialBuilder<T>(workflow as Workflow<T>);
        }

        public virtual WorkflowBuilder<T> GetParallelBuilder(IWorkflow<T> workflow)
        {
            return new ParallelSplitBuilder<T>(workflow as Workflow<T>);
        }
    }
}