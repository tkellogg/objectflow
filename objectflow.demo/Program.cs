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
            var colourPipe = new Pipeline<Colour>();
            var doubleSpace = new DoubleSpace();
            var doublespace2 = new DoubleSpace();

            colourPipe
                .Execute(doubleSpace)
                .Execute(doublespace2, When.Not.Successfull(doubleSpace))
                .Execute(new ConsoleWriter());

            var result = colourPipe.Start(new Colour("green"));

            Console.WriteLine("\r\nPress any key");
            Console.ReadKey();
        }
    }
}