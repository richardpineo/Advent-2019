using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

class Solve5 : ISolve
{
    public string Description()
    {
        return "Day 5: Sunny with a Chance of Asteroids";
    }

    const string Input = "Input//5.txt";
    const string ExampleA = "Examples//5a.txt";
    const string ExampleB = "Examples//5b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        string[] lines = File.ReadAllLines(ExampleA, Encoding.UTF8);
        foreach (var line in lines)
        {
            var inOut = line.Split(" ");

            var start = Intcode.ParseInput(inOut[0]);
            var end = Intcode.ParseInput(inOut[1]);
            var output = RunProgram(start, 1);
            var expected = string.Join(",", end);
            var actual = string.Join(",", start);
            if (expected != actual)
            {
                Console.WriteLine();
                Console.WriteLine($"  Expacted: {expected}");
                Console.WriteLine($"  Actual:   {actual}");
                return false;
            }
        }

        return true;
    }

    public bool ProveB()
    {
        string[] lines = File.ReadAllLines(ExampleB, Encoding.UTF8);

        var tests = new Dictionary<int, int>();
        tests[7] = 999;
        tests[8] = 1000;
        tests[9] = 1001;

        foreach (var line in lines)
        {
            var inputs = line.Split(" ");
            var input = int.Parse(inputs[0]);
            var expected = int.Parse(inputs[1]);
            var program = Intcode.ParseInput(inputs[2]);
            var output = RunProgram(program, input);
            if (output != expected)
            {
                Console.WriteLine();
                Console.WriteLine($"  Line:       {inputs[2]}");
                Console.WriteLine($"  Input:      {input}");
                Console.WriteLine($"  Expected:   {expected}");
                Console.WriteLine($"  Actual:     {output}");

                return false;
            }
        }
        return true;
    }

    private int? RunProgram(int[] program, int input)
    {
        var finalOutput = -1;
        var state = new Intcode.State { program = program, input = input };
        while (Intcode.Step(state))
        {
            // nothing
            // consume the output
            if (state.output.HasValue)
            {
                finalOutput = state.output.Value;
                state.output = null;
            }
        }
        return finalOutput;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return SolveFor(1).ToString();
    }

    public string SolveB()
    {
        return SolveFor(5).ToString();
    }
    private int SolveFor(int input)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var start = Intcode.ParseInput(lines[0]);
        return RunProgram(start, input).Value;
    }
}
