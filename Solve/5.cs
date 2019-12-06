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
    const string ExampleB = "Examples//5a.txt";

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

    private int RunProgram(List<int> program, int input)
    {
        int output = 666;
        int pos = 0;
        for (int step = 0; step != -1; step = Step(program, pos, input, ref output))
        {
            // nothing
            pos += step;
        }
        return output;
    }

    private bool[] getModes(int numArgs, int command)
    {
        var bitfield = command / 100;
        var modes = new bool[numArgs];
        for (int i = 0; i < numArgs; i++)
        {
            modes[i] = 0 != bitfield % 2;
            bitfield = bitfield / 10;
        }
        return modes;
    }

    private int Step(List<int> program, int pos, int input, ref int output)
    {
        int command = program[pos];
        int opCode = command % 100;
        switch (opCode)
        {
            case 1:
                {
                    var modes = getModes(3, command);
                    return processAdd(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
                }
            case 2:
                {
                    var modes = getModes(3, command);
                    return processMul(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
                }
            case 3:
                {
                    return processInput(program[pos + 1], input, program);

                }
            case 4:
                {
                    return processOutput(program[pos + 1], program, ref output);
                }
            case 99:
                return -1;
        }
        throw new Exception("opcode is out of range");
    }

    public int processAdd(bool[] modes, int op1, int op2, int op3, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 + val2;
        return 4;
    }
    public int processMul(bool[] modes, int op1, int op2, int op3, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 * val2;
        return 4;
    }
    public int processInput(int op1, int input, List<int> program)
    {
        program[op1] = input;
        return 2;
    }

    public int processOutput(int op1, List<int> program, ref int output)
    {
        output = program[op1];
        return 2;
    }
    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return SolveFor().ToString();
    }

    private int SolveFor()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var start = ParseInput(lines[0]);
        return RunProgram(start, 1);
    }

    public string SolveB()
    {
        return "Not found!";
    }
}
