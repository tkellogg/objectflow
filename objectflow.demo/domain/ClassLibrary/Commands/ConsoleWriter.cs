using System;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
{
    public class ConsoleWriter : BasicOperation<Colour>
    {
        public override IEnumerable<Colour> Execute(IEnumerable<Colour> input)
        {
            foreach (var member in input)
            {
                Console.WriteLine(string.Format("Member: {0}", member.Name));
            }

            return input;
        }
    }
}