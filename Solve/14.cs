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
        public Component(string id, long count)
        {
            Id = id;
            Count = count;
        }

        public string Id;
        public long Count;

        public static Component Parse(string s)
        {
            var tokens = s.Split(" ");
            if (tokens.Count() != 2)
            {
                return null;
            }
            return new Component(tokens[1], long.Parse(tokens[0]));
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

    public long Solve(long numFuel)
    {
        long oreUsed = 0;
        var stock = new Dictionary<string, long>();
        var needed = new Queue<Component>();
        needed.Enqueue(new Component("FUEL", numFuel));

        while (needed.Count > 0)
        {
            var toMake = needed.Dequeue();

            // Take from stock if possible.
            long onHand = 0;
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
        return isA ? ProveA() : ProveB();
    }

    public bool ProveA()
    {
        foreach (string example in Examples)
        {
            var lines = File.ReadAllLines(example, Encoding.UTF8);
            var answer = long.Parse(lines[0]);
            var input = lines.Skip(1);
            var factory = Factory.Parse(input.ToArray());
            var possible = factory.Solve(1);
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
        return factory.Solve(1).ToString();
    }

    public string SolveB()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var factory = Factory.Parse(lines.ToArray());

        // Find cost of one then hone in on it.
        var oreForOne = factory.Solve(1);

        var target = 1000000000000;
        var fuelLowerBound = target / oreForOne;
        var fuelUpperBound = fuelLowerBound * 2;

        var lower = factory.Solve(fuelLowerBound);
        var upper = factory.Solve(fuelUpperBound);

        long possible = BinarySearch(factory.Solve, target, fuelLowerBound, fuelUpperBound);
        return possible.ToString();
    }

    // https://stackoverflow.com/questions/49843710/binary-search-closest-value-c-sharp
    public static long BinarySearch(Func<long, long> eval, long item, long min, long max)
    {
        long first = min;
        long last = max;
        long mid = 0;
        do
        {
            mid = first + (last - first) / 2;
            var value = eval(mid);
            if (item > value)
                first = mid + 1;
            else
                last = mid - 1;
            if (value == item)
                return mid;
        } while (first <= last);
        return last;
    }

    public bool ProveB()
    {
        string[] Examples = new string[] {
                "Examples//14b-3.txt",
                "Examples//14b-4.txt",
                "Examples//14b-5.txt",
            };

        foreach (string example in Examples)
        {
            var lines = File.ReadAllLines(example, Encoding.UTF8);
            var answerA = int.Parse(lines[0]);
            var fuelProduced = int.Parse(lines[1]);
            var input = lines.Skip(2);
            var factory = Factory.Parse(input.ToArray());

            // Find cost of one then hone in on it.
            var oreForOne = factory.Solve(1);

            var target = 1000000000000;
            var fuelLowerBound = target / oreForOne;
            var fuelUpperBound = fuelLowerBound * 2;

            var lower = factory.Solve(fuelLowerBound);
            var upper = factory.Solve(fuelUpperBound);

            long possible = BinarySearch(factory.Solve, target, fuelLowerBound, fuelUpperBound);

            if (possible != fuelProduced)
            {
                Console.WriteLine($"Example {example} failed: got {possible}, expected {fuelProduced}");
                return false;
            }
        }

        return true;
    }
}
