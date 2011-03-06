using System;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Language;

namespace Rainbow.Demo.Objectflow.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var workflow = new RainbowWorkflow();
            workflow.Configure(Workflow<Colour>.Definition());

            workflow.Start(new Colour("Green"));

            Console.WriteLine("\r\nPress any key");
            Console.ReadKey();
        }
    }

    internal class RainbowWorkflow : AsAWorkflow<Colour>
    {
        public override void Configure(IDefine<Colour> workflow)
        {
            var doubleSpace = new DoubleSpace();

            workflow
                .Do(doubleSpace)
                .Do<DoubleSpace>(If.Not.Successfull(doubleSpace))
                .Do((c) =>
                        {
                            Console.WriteLine(c.Name);
                            return c;
                        });

            Workflow = workflow as IWorkflow<Colour>;
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

    public class MyWorkflow : Workflow<Colour>
    {
        public MyWorkflow()
        {

        }

        void SetOperations()
        {
            Definition()
                .Do<DoubleSpace>()
                .Then()
                .Do<DoubleSpace>();


        }
    }
}
