using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Framework
{
    ///<summary>
    /// Provides fiunctionality common to all workflows and interfaces to configure and start workflows
    ///</summary>
    ///<typeparam name="T">Type the workflow transforms</typeparam>
    public abstract class AsAWorkflow<T> : IConfigureSequence<T>, IExecuteWorkflow<T> where T : class
    {
        protected IWorkflow<T> Workflow;

        ///<summary>
        /// Allows a workflow operations and policies to be added
        ///</summary>
        ///<param name="workflow"></param>
        public abstract void Configure(IDefine<T> workflow);

        ///<summary>
        /// Start a worfklow
        ///</summary>
        ///<returns>Transformed data of T</returns>
        public virtual T Start()
        {
            Check.IsNotNull(Workflow, "Workflow");
            return Workflow.Start();
        }

        ///<summary>
        /// Start workflow
        ///</summary>
        ///<param name="seedData">Data that the first operation works on.</param>
        ///<returns>Transformed data of T</returns>
        public virtual T Start(T seedData)
        {
            Check.IsNotNull(Workflow, "Workflow");
            Check.IsNotNull(seedData, "seedData");
            return Workflow.Start(seedData);            
        }
    }
}