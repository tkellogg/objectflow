using System.Collections.Generic;
using objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;

namespace objectflow.tests.TestDomain
{
    /// <summary>
    /// Concrete pipeline for TeamMember type.  This class specifies the operations that should occur and 
    /// the order they occur in to transform the data encapsualted in TeamMembers
    /// </summary>
    public class ColourPipeline : Pipeline<Colour>
    {
        public ColourPipeline()
        {
        }

        public ColourPipeline(IEnumerable<Colour> team)
        {
            Execute(new LoadList(team));
        }
    }
}