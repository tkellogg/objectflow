using System;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.Demo.Objectflow.Domain.Commands
{
    public class ConsoleWriter : BasicOperation<Colour>
    {
        public override Colour Execute(Colour input)
        {
            Console.WriteLine(string.Format("Member: {0}", input.Name));

            return input;
        }
    }
}