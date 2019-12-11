using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;

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
            var output = RunProgram(ref start, 1);
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

        var tests = new Dictionary<IntType, IntType>();
        tests[7] = 999;
        tests[8] = 1000;
        tests[9] = 1001;

        foreach (var line in lines)
        {
            var inputs = line.Split(" ");
            var input = IntType.Parse(inputs[0]);
            var expected = IntType.Parse(inputs[1]);
            var program = Intcode.ParseInput(inputs[2]);
            var output = RunProgram(ref program, input);
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

    private IntType? RunProgram(ref IntType[] program, IntType input)
    {
        IntType finalOutput = -1;
        var state = new Intcode.State(program, input);
        while (Intcode.Step(state))
        {
            // nothing
            // consume the output
            var tempOut = state.PopOutput();
            if (tempOut.HasValue)
            {
                finalOutput = tempOut.Value;
            }
        }
        program = state.MemoryDump(program.Length);
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
    private IntType SolveFor(IntType input)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var start = Intcode.ParseInput(lines[0]);
        return RunProgram(ref start, input).Value;
    }
}
