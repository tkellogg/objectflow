using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IExpression : IHideObjectMembers
    {
        IWith With { get; }
    }
}