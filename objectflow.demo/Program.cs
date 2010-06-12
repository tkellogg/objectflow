using System;
using Rainbow.Demo.Objectflow.Domain;
using Rainbow.Demo.Objectflow.Domain.Commands;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.Demo.Objectflow.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var colourPipe = new Workflow<Colour>();
            var doubleSpace = new DoubleSpace();
            var doublespace2 = new DoubleSpace();

            colourPipe
                .Do(doubleSpace)
                .Do(doublespace2, If.Not.Successfull(doubleSpace))
                .Do((c) =>
                             {
                                 Console.WriteLine(c.Name); return c;
                             });

            var result = colourPipe.Start(new Colour("green"));

            Console.WriteLine("\r\nPress any key");
            Console.ReadKey();
        }
    }
}