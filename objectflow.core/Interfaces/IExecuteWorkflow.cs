namespace Rainbow.ObjectFlow.Interfaces
{
    ///<summary>
    /// Defines how to start workflows
    ///</summary>
    ///<typeparam name="T">Type of object to be executed</typeparam>
    public interface IExecuteWorkflow<T>
    {
        ///<summary>
        /// Start a worfklow
        ///</summary>
        ///<returns>Transformed data of T</returns>
        T Start();

        ///<summary>
        /// Start workflow
        ///</summary>
        ///<param name="seedData">Data that the first operation works on.</param>
        ///<returns>Transformed data of T</returns>
        T Start(T seedData);
    }
}