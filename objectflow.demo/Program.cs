using System;
using System.Collections.Generic;
using Rainbow.Demo.Objectflow.Domain;

namespace Rainbow.Demo.Objectflow.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var teamMemberList = new ColourPipeline(GetTeams());
            teamMemberList.Start();

            Console.WriteLine("\r\nPress any key");
            Console.ReadKey();
        }

        private static IEnumerable<Colour> GetTeams()
        {
            var team = new Colour[]
                           {
                               new Colour("Bijou"), 
                               new Colour("Deepak"), 
                               new Colour("Garfield"),
                               new Colour("Leon"), 
                               new Colour("Surya")
                           };

            return team;
        }
    }
}