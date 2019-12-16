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
        public Component(string id, int count)
        {
            Id = id;
            Count = count;
        }

        public string Id;
        public int Count;

        public static Component Parse(string s)
        {
            var tokens = s.Split(" ");
            if (tokens.Count() != 2)
            {
                return null;
            }
            return new Component(tokens[1], int.Parse(tokens[0]));
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
        var oreUsed = 0;
        var stock = new Dictionary<string, int>();
        var needed = new Queue<Component>();
        needed.Enqueue(new Component("FUEL", 1));

        while (needed.Count > 0)
        {
            var toMake = needed.Dequeue();

            // Take from stock if possible.
            var onHand = 0;
            stock.TryGetValue(toMake.Id, out onHand);
            if (onHand > toMake.Count)
            {
                stock[toMake.Id] = onHand - toMake.Count;
            }
            else
            {
                // figure out what we need to make the thing.
                var toMakeAmount = toMake.Count - onHand;

                var reaction = reactions.First(r => r.Output.Id == toMake.Id);
                var numNeeded = toMakeAmount / reaction.Output.Count;
                var leftover = toMakeAmount % reaction.Output.Count;
                if (leftover != 0)
                {
                    numNeeded += 1;
                    leftover = reaction.Output.Count - leftover;
                }

                stock[toMake.Id] = leftover;

                // components
                var neededComponents = reaction.Inputs.Select(i => new Component(i.Id, i.Count * numNeeded));
                foreach (var c in neededComponents)
                {
                    if (c.Id == "ORE")
                    {
                        oreUsed += c.Count;
                    }
                    else
                    {
                        needed.Enqueue(c);
                    }
                }
            }
        }

        // Basically just work backwards from one fuel.
        return oreUsed;
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
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var factory = Factory.Parse(lines.ToArray());
        return factory.Solve().ToString();
    }

    public string SolveB()
    {
        return "NotImpl";
    }
}
