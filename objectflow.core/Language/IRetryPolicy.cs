using Rainbow.ObjectFlow.Interfaces;

#pragma warning disable 1591
namespace Rainbow.ObjectFlow.Language
{
    public interface IRetryPolicy : IHideObjectMembers, IPolicy
    {
        /// <summary>
        /// Number of times a retry policy is attempted
        /// </summary>
        IExpression Attempts(int times);

        IExpression Once();
        
        IExpression Twice();
    }
}