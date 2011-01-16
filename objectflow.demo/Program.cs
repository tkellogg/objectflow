using System;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.Demo.Objectflow.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var doubleSpace = new DoubleSpace();

            var result = Workflow<Colour>.Definition()
                .Do(doubleSpace)
                .Do<DoubleSpace>(If.Not.Successfull(doubleSpace))
                .Do((c) =>
                        {
                            Console.WriteLine(c.Name);
                            return c;
                        })
                .Start(new Colour("Green"));

            Console.WriteLine("\r\nPress any key");
            Console.ReadKey();
        }
    }
}

public class Colour
{
    public Colour(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

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
