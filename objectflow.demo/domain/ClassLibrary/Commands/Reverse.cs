using System.Collections.Generic;
using System.Linq;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
{
    public class Reverse : BasicOperation<Colour>
    {
        public override IEnumerable<Colour> Execute(IEnumerable<Colour> operations)
        {
            return operations.Reverse();
        }
    }
}