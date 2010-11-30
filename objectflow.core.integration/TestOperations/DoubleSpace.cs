using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.tests.TestOperations
{
    public class DoubleSpace : BasicOperation<Colour>
    {
        public override Colour Execute(Colour colour)
        {
            string name = string.Empty;
            char[] chars = colour.Name.ToCharArray();
            foreach (var c in chars)
            {
                name = name + c + " ";
            }

            colour.Name = name.Trim();

            SetSuccessResult(GetSuccessResult());

            return colour;
        }

        public virtual bool GetSuccessResult()
        {
            return true;
        }
    }
}