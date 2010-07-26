using System;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Default in-memory loader for pipeline returns the data it was instantiated with.
    /// The pipeline requires a loader to get data into pipeline.  
    /// This could be a text file, database or other external resource.
    /// <remarks>
    /// A loader Operation must be the first operation a Pipeline is ocnfigured to execute.
    /// </remarks>
    /// </summary>
    /// <typeparam name="T">Type of object the operation will contain.</typeparam>    
    [Obsolete("Use Start(T) method of workflow.  This is included as an example implementation of the abstract BasicOperation class.")]
    public class WorkflowMemoryLoader<T> : BasicOperation<T>
    {
        private readonly T _state;

        /// <summary>
        /// Instantietes the class with the input data for the pipeline.
        /// </summary>
        /// <param name="initialState">The data the operation will work on</param>
        public WorkflowMemoryLoader(T initialState)
        {
            _state = initialState;
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="state">The data the operation will work on</param>
        /// <returns>The transformed data</returns>
        public override T Execute(T state)
        {
            SetSuccessResult(true);
            return _state;
        }
    }
}