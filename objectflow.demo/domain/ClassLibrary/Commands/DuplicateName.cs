using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
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