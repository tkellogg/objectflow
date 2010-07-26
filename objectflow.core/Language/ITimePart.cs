namespace Rainbow.ObjectFlow.Language
{
#pragma warning disable 1591
    public interface ITimePart : IHideObjectMembers
    {
        void Milliseconds(int millseconds);
        void Seconds(int seconds);
        void Minutes(int minutes);
    }
}