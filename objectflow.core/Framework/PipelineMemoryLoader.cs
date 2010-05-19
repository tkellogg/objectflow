using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Default in-memort loader for pipeline returns the data it was instantiated with.
    /// The pipeline requires a loader to get data into pipeline.  
    /// This could be a text file, database or other external resource.
    /// <remarks>
    /// A loader Operation must be the first operation a Pipeline is ocnfigured to execute.
    /// </remarks>
    /// </summary>
    /// <typeparam name="T">Type of object the operation will contain.</typeparam>
    public class PipelineMemoryLoader<T> : BasicOperation<T>
    {
        private readonly IEnumerable<T> _team = new List<T>();

        /// <summary>
        /// Instantietes the class with the input data for the pipeline.
        /// </summary>
        /// <param name="inputData">The data the operation will work on</param>
        public PipelineMemoryLoader(IEnumerable<T> inputData)
        {
            _team = inputData;
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="inputData">The data the operation will work on</param>
        /// <returns>The transformed data</returns>
        public override IEnumerable<T> Execute(IEnumerable<T> inputData)
        {
            return _team;
        }
    }
}