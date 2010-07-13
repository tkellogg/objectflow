using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IWith : IHideObjectMembers
    {
        IInterval Interval { get; }
    }
}