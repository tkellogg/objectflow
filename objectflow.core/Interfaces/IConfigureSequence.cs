using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Interfaces
{
    ///<summary>
    /// Defines how to configure a workflow class
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public interface IConfigureSequence<T> where T : class
    {
        ///<summary>
        /// Allows a workflow operations and policies to be added
        ///</summary>
        ///<param name="workflow"></param>
        void Configure(IDefine<T> workflow);
    }
}