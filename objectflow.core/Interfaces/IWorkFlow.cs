using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Interface for the workflow pipeline
    ///</summary>
    public interface IWorkflow<T> : IDefine<T>, IExecuteWorkflow<T> where T : class
    {
        ///<summary>
        /// Chained operations will be executed concurrently
        ///</summary>
        ICompose<T> And { get; }

        /// <summary>
        /// Gets or sets the context used by operations and functions in the workflow
        /// </summary>
        T Context { get; set; }

        ///<summary>
        /// Merges concurrent operations.  By default engine waits for all parallel operations to finish before executing subsequent operations sequentially
        ///</summary>
        /// <remarks>The default is to wait for all concurrent operatins to finish before continuing with sequential the following sequential operations</remarks>
        IMerge<T> Then();

        ///<summary>
        /// Attempt the operation again if it does not finish successfully
        ///</summary>
        /// <remarks>Currently ignores any operations that do not inherit from BasicOperation</remarks>
        IRetryPolicy Retry();

        /// <summary>
        /// Do the operation again
        /// </summary>
        IRepeat Repeat();
    }
}