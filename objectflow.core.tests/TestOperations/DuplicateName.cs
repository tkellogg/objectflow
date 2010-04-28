using System.Collections.Generic;
using objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;

namespace objectflow.tests.TestOperations
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