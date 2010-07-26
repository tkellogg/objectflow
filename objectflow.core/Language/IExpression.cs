namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IExpression : IHideObjectMembers
    {
        /// <summary>
        /// Adds behaviours to the associated policy
        /// </summary>
        IWith With { get; }
    }
}