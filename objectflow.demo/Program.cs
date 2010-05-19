using System;
using System.Collections.Generic;
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
                .Execute(new PipelineMemoryLoader<Colour>(GetColours()))
                .Execute(doubleSpace)
                .Execute(doublespace2, When.Not.Successfull(doubleSpace));

            var results = colourPipe.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Console.WriteLine(result);
            Console.WriteLine("\r\nPress any key");
            Console.ReadKey();
        }

        private static IEnumerable<Colour> GetColours()
        {
            var team = new Colour[]
                           {
                               new Colour("Red"), 
                               new Colour("Yellow"), 
                               new Colour("Green"),
                               new Colour("Pink"), 
                               new Colour("Purple")
                           };

            return team;
        }
    }
}