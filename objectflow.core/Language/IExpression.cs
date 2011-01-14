namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IExpression : IHideObjectMembers
    {
        /// <summary>
        /// Adds behaviours to the associated policy
        /// </summary>
        IWith With { get; }

        ///<summary>
        /// Merges concurrent operations.  By default engine waits for all parallel operations to finish before executing subsequent operations sequentially
        ///</summary>
        /// <remarks>The default is to wait for all concurrent operatins to finish before continuing with sequential the following sequential operations</remarks>
        IMerge<T> Then<T>() where T:class ;
    }
}