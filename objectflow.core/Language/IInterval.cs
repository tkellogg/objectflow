namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface IInterval: IHideObjectMembers 
    {
        ITimePart Of { get; }
    }
}