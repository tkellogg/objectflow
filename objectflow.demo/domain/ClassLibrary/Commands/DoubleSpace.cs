using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
{
    public class DoubleSpace : BasicOperation<Colour>
    {
        public override Colour Execute(Colour input)
        {
            string name = string.Empty;
            char[] chars = input.Name.ToCharArray();
            foreach (var c in chars)
            {
                name = name + c + " ";
            }

            input.Name = name.Trim();

            SetSuccessResult(true);

            return input;
        }
    }
}