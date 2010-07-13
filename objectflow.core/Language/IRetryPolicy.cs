using Rainbow.ObjectFlow.Interfaces;

#pragma warning disable 1591
namespace Rainbow.ObjectFlow.Language
{
    public interface IRetryPolicy : IHideObjectMembers, IPolicy
    {
        IExpression Attempts(int number);

        IExpression Once();
        
        IExpression Twice();

    }
}