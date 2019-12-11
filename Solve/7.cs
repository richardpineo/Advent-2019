using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using IntType = System.Int64;


class Solve7 : ISolve
{
    public string Description()
    {
        return "Day 7: Amplification Circuit";
    }

    const string Input = "Input//7.txt";
    const string ExampleA = "Examples//7a.txt";
    const string ExampleB = "Examples//7b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        string[] lines = File.ReadAllLines(ExampleA, Encoding.UTF8);
        foreach (var line in lines)
        {
            var tokens = line.Split(" ");

            var answer = IntType.Parse(tokens[0]);

            // Array of 5 IntType
            var order = tokens[1].Split(",").Select(t => IntType.Parse(t));

            var power = ComputePower(order.ToArray(), tokens[2]);

            if (power != answer)
            {
                Console.WriteLine($"{power} should have been {answer}, {tokens[2]}");
                return false;
            }
        }

        return true;
    }


    private bool ProveB()
    {
        string[] lines = File.ReadAllLines(ExampleA, Encoding.UTF8);
        foreach (var line in lines)
        {
            var tokens = line.Split(" ");

            var answer = IntType.Parse(tokens[0]);

            // Array of 5 IntType
            var order = tokens[1].Split(",").Select(t => IntType.Parse(t));

            var power = ComputePowerFeedback(order.ToArray(), tokens[2]);

            if (power != answer)
            {
                Console.WriteLine($"{power} should have been {answer}, {tokens[2]}");
                return false;
            }
        }

        return true;
    }

    private IntType ComputePower(IntType[] order, string rawProgram)
    {
        IntType power = 0;
        foreach (var phase in order)
        {
            IntType[] inputs = { phase, power };

            var program = Intcode.ParseInput(rawProgram);

            var inputPos = 0;
            var state = new Intcode.State(program.ToArray(), inputs[inputPos++]);
            while (Intcode.Step(state))
            {
                if (!state.input.HasValue && inputPos < inputs.Length)
                {
                    state.input = inputs[inputPos++];
                }
            }
            power = state.output ?? -1;
        }
        return power;
    }

    private IntType ComputePowerFeedback(IntType[] order, string rawProgram)
    {
        var states = new List<Intcode.State>();
        states.Add(new Intcode.State(Intcode.ParseInput(rawProgram).ToArray(), order[0]));
        states.Add(new Intcode.State(Intcode.ParseInput(rawProgram).ToArray(), order[1]));
        states.Add(new Intcode.State(Intcode.ParseInput(rawProgram).ToArray(), order[2]));
        states.Add(new Intcode.State(Intcode.ParseInput(rawProgram).ToArray(), order[3]));
        states.Add(new Intcode.State(Intcode.ParseInput(rawProgram).ToArray(), order[4]));

        // Initialize
        var hasInitialized = false;

        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 0 && !hasInitialized && !states[0].input.HasValue)
                {
                    hasInitialized = true;
                    states[0].input = 0;
                }

                if (states[i].pos != -1)
                {
                    Intcode.Step(states[i]);
                }
                else if (i == 4)
                {
                    // We've already pushed to the first state.
                    return states[0].input.Value;
                }

                if (states[i].output.HasValue)
                {
                    var toState = i + 1;
                    if (toState == 5)
                    {
                        toState = 0;
                    }
                    if (!states[toState].input.HasValue)
                    {
                        states[toState].input = states[i].PopOutput();
                    }
                }
            }
        }
    }

    private bool[] getModes(IntType command)
    {
        var numArgs = 10; // max of 10 args.
        var bitfield = command / 100;
        var modes = new bool[numArgs];
        for (IntType i = 0; i < numArgs; i++)
        {
            modes[i] = 0 != bitfield % 2;
            bitfield = bitfield / 10;
        }
        return modes;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        IntType[] boosters = { 0, 1, 2, 3, 4 };
        return SolveFor(boosters, ComputePower).ToString();
    }

    public string SolveB()
    {
        IntType[] boosters = { 5, 6, 7, 8, 9 };
        return SolveFor(boosters, ComputePowerFeedback).ToString();
    }

    // https://stackoverflow.com/questions/1952153/what-is-the-best-way-to-find-all-combinations-of-items-in-an-array/10629938#10629938
    static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, IntType length)
    {
        if (length == 1) return list.Select(t => new T[] { t });
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(o => !t.Contains(o)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    private IntType SolveFor(IntType[] boosters, Func<IntType[], string, IntType> computer)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var rawProgram = lines[0];

        IntType maxPower = 0;

        var attempts = GetPermutations(boosters, 5);

        foreach (var attempt in attempts)
        {
            var power = computer(attempt.ToArray(), rawProgram);
            maxPower = Math.Max(power, maxPower);
        }

        return maxPower;
    }
}
