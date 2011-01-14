namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface ITimePart : IHideObjectMembers
    {
        IExpression Milliseconds(int millseconds);
        //IMerge<T> Milliseconds<T>(int millseconds) where T : class;
        IExpression Seconds(int seconds);
        IExpression Minutes(int minutes);
    }
}