using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
{
    /// <summary>
    /// Gets the list of team members the pipeline will work on.
    /// </summary>
    internal class LoadList : BasicOperation<Colour>
    {
        private readonly IEnumerable<Colour> _team = new List<Colour>();

        public LoadList(IEnumerable<Colour> team)
        {
            _team = team;
        }

        public override IEnumerable<Colour> Execute(IEnumerable<Colour> operations)
        {
            return _team;
        }
    }
}