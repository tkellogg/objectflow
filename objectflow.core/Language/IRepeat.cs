using System;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IRepeat : IHideObjectMembers, IPolicy
    {
        IExpression Times(int times);

        IExpression Once();

        IExpression Twice();

        // TODO: Put back in for next release
        //IExpression Until(Func<bool> becomesTrue);
    }
}
