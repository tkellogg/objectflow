using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
{
    public class DuplicateName : BasicOperation<Colour>
    {
        public override IEnumerable<Colour> Execute(IEnumerable<Colour> operations)
        {
            foreach (Colour member in operations)
            {
                member.Name += member.Name;
            }

            SetSuccessResult(true);
            return operations;
        }
    }
}