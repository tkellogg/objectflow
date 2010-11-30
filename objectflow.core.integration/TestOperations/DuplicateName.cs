using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.tests.TestOperations
{
    public class DuplicateName : BasicOperation<Colour>
    {
        public override Colour Execute(Colour colour)
        {
            colour.Name += colour.Name;

            SetSuccessResult(true);
            return colour;
        }
    }
}