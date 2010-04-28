using System.Collections.Generic;
using Rainbow.Demo.Objectflow.Domain.Commands;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain
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