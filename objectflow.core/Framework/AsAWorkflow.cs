using System;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Framework
{
    ///<summary>
    /// Provides functionality common to all workflows and interfaces to configure and start workflows
    ///</summary>
    ///<typeparam name="T">Type the workflow transforms</typeparam>
    public abstract class AsAWorkflow<T> : IConfigureSequence<T>, IExecuteWorkflow<T> where T : class
    {
        protected IWorkflow<T> Workflow;
        protected IDefine<T> Configurer;

        /// <summary>
        /// Configures the workflow with a workflow configuration object
        /// </summary>
        /// <param name="workflow">Workflow configuration obejct</param>
        protected AsAWorkflow(IDefine<T> workflow)
        {
            Check.IsNotNull(workflow, "workfow definer");
            Configurer = workflow;
        }

        ///<summary>
        /// Allows a workflow operations and policies to be added
        ///</summary>
        ///<param name="workflow"></param>        
        [Obsolete("Use Configure() to configure the method and pass the definer into the constructor")]
        public abstract void Configure(IDefine<T> workflow);

        /// <summary>
        /// Allows a workflow operations and policies to be added
        /// </summary>
        /// <returns></returns>
        public abstract IWorkflow<T> Configure();

        ///<summary>
        /// Start a worfklow
        ///</summary>
        ///<returns>Transformed data of T</returns>
        public virtual T Start()
        {
            if (null == Workflow)
                Workflow = Configure();

            return Workflow.Start();
        }

        ///<summary>
        /// Start workflow
        ///</summary>
        ///<param name="seedData">Data that the first operation works on.</param>
        ///<returns>Transformed data of T</returns>
        public virtual T Start(T seedData)
        {
            if (null == Workflow)
                Workflow = Configure();
            
            return Workflow.Start(seedData);
        }
    }
}