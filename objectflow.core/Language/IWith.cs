namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IWith : IHideObjectMembers
    {
        /// <summary>
        /// Defines time
        /// </summary>
        IInterval Interval { get; }
    }
}