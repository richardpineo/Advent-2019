using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

[DebuggerDisplay("({reactions.Length} reactions)")]
class Factory
{
    private Factory()
    {

    }

    [DebuggerDisplay("{Count} {Id}")]
    class Component
    {
        public string Id;
        public int Count;

        public static Component Parse(string s)
        {
            var tokens = s.Split(" ");
            if (tokens.Count() != 2)
            {
                return null;
            }
            var component = new Component();
            component.Count = int.Parse(tokens[0]);
            component.Id = tokens[1];
            return component;
        }
    }

    [DebuggerDisplay("{Inputs.Length} Inputs -> {Output}")]
    class Reaction
    {
        public Component[] Inputs;
        public Component Output;
    }

    private Reaction[] reactions;

    public static Factory Parse(string[] lines)
    {
        // Each line has the form:
        // [<val> name], => <val name>
        var reactions = new List<Reaction>();
        foreach (string line in lines)
        {
            var leftRight = line.Split(" => ");
            if (leftRight.Count() != 2)
            {
                return null;
            }

            var ingredients = leftRight[0].Split(",").Select(s => s.Trim());
            var output = leftRight[1].Trim();
            var reaction = new Reaction();
            reaction.Inputs = ingredients.Select(i => Component.Parse(i)).ToArray();
            reaction.Output = Component.Parse(output);
            reactions.Add(reaction);
        }
        var factory = new Factory() { reactions = reactions.ToArray() };
        return factory;
    }

    public int Solve()
    {
        return 0;
    }
}

class Solve14 : ISolve
{
    public string Description()
    {
        return "Day 14: Space Stoichiometry";
    }

    const string Input = "Input//14.txt";

    string[] Examples = new string[] {
        "Examples//14a-1.txt",
        "Examples//14a-2.txt",
        "Examples//14a-3.txt",
        "Examples//14a-4.txt",
        "Examples//14a-5.txt",
    };

    public bool Prove(bool isA)
    {
        foreach (string example in Examples)
        {
            var lines = File.ReadAllLines(example, Encoding.UTF8);
            var answer = int.Parse(lines[0]);
            var input = lines.Skip(1);
            var factory = Factory.Parse(input.ToArray());
            var possible = factory.Solve();
            if (possible != answer)
            {
                Console.WriteLine($"Example {example} failed: got {possible}, expected {answer}");
                return false;
            }
        }

        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return "NotImpl";
    }
    public string SolveB()
    {
        return "NotImpl";
    }
}
