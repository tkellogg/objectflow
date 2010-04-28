
namespace objectflow.tests.TestDomain
{
    /// <summary>
    /// Domain class for the pipeline demo
    /// </summary>
    public class Colour
    {
        public Colour(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}