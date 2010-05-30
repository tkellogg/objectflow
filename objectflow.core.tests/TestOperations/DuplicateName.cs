using objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;

namespace objectflow.tests.TestOperations
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