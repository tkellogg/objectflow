using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
{
    public class DoubleSpace : BasicOperation<Colour>
    {
        public override IEnumerable<Colour> Execute(IEnumerable<Colour> input)
        {
            foreach (Colour member in input)
            {
                string name = string.Empty;
                char[] chars = member.Name.ToCharArray();
                foreach (var c in chars)
                {
                    name = name + c + " ";
                }

                member.Name = name.Trim();
            }

            SetSuccessResult(true);

            return input;
        }
    }
}