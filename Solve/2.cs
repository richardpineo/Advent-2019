using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Solve2 : ISolve
{
    public string Description()
    {
        return "Day 2: 1202 Program Alarm";
    }

    const string Input = "Input//2.txt";
    const string ExampleA = "Examples//2a.txt";
    const string ExampleB = "Examples//2b.txt";

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

            var start = ParseInput(inOut[0]);
            var end = ParseInput(inOut[1]);
            RunProgram(start);
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
        // No prove here.
        return true;
    }

    private List<int> ParseInput(string input)
    {
        var list = new List<int>();
        var inputs = input.Split(",");
        foreach (var i in inputs)
        {
            list.Add(int.Parse(i));
        }
        return list;
    }

    private void RunProgram(List<int> program)
    {
        for (int pos = 0; pos != -1; pos = Step(program, pos))
        {
            // nothing
        }
    }

    private int Step(List<int> program, int pos)
    {
        int command = program[pos];
        if (command == 99)
        {
            return -1;
        }

        int arg1 = program[program[pos + 1]];
        int arg2 = program[program[pos + 2]];
        int outPos = program[pos + 3];
        int val = (command == 1) ? (arg1 + arg2) : (arg1 * arg2);
        program[outPos] = val;
        return pos + 4;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return SolveFor(12, 2).ToString();
    }

    private int SolveFor(int noun, int verb)
    {
        string[] lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = ParseInput(lines[0]);

        program[1] = noun;
        program[2] = verb;

        RunProgram(program);

        return program[0];
    }

    public string SolveB()
    {
        for (int noun = 0; noun < 100; noun++)
        {
            for (int verb = 0; verb < 100; verb++)
            {
                var found = SolveFor(noun, verb);
                if (found == 19690720)
                {
                    return String.Format("{0:00}", noun) + String.Format("{0:00}", verb);
                }
            }
        }
        return "Not found!";
    }
}
