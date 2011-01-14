using Rainbow.ObjectFlow.Framework;

namespace objectflow.core.tests.integration.TestOperations
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