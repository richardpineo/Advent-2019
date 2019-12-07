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
            var program = ParseInput(inputs[2]);
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
        for (int pos = 0; pos != -1; pos = Step(program, pos, input, ref output))
        { }
        return output;
    }

    private int RunProgramB(List<int> program, int input)
    {
        int output = 666;
        for (int pos = 0; pos != -1; pos = Step(program, pos, input, ref output))
        {
        }
        return output;
    }

    private bool[] getModes(int command)
    {
        var numArgs = 10; // max of 10 args.
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
        var modes = getModes(command);

        switch (opCode)
        {
            case 1:
                return pos + add(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
            case 2:
                return pos + multiply(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
            case 3:
                return pos + read(program[pos + 1], input, program);
            case 4:
                return pos + write(modes, program[pos + 1], program, ref output);
            case 5:
                return jumpTrue(modes, pos, program[pos + 1], program[pos + 2], program);
            case 6:
                return jumpFalse(modes, pos, program[pos + 1], program[pos + 2], program);
            case 7:
                return pos + lessThan(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
            case 8:
                return pos + equals(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
            case 99:
                return -1;
        }
        throw new Exception("opcode is out of range");
    }

    public int add(bool[] modes, int op1, int op2, int op3, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 + val2;
        return 4;
    }
    public int multiply(bool[] modes, int op1, int op2, int op3, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 * val2;
        return 4;
    }
    public int read(int op1, int input, List<int> program)
    {
        program[op1] = input;
        return 2;
    }
    public int write(bool[] modes, int op1, List<int> program, ref int output)
    {
        output = modes[0] ? op1 : program[op1]; ;
        return 2;
    }
    public int jumpTrue(bool[] modes, int pos, int op1, int op2, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        return val1 != 0 ? val2 : (pos + 3);
    }
    public int jumpFalse(bool[] modes, int pos, int op1, int op2, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        return val1 == 0 ? val2 : (pos + 3);
    }
    public int lessThan(bool[] modes, int op1, int op2, int op3, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 < val2 ? 1 : 0;
        return 4;
    }
    public int equals(bool[] modes, int op1, int op2, int op3, List<int> program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 == val2 ? 1 : 0;
        return 4;
    }
    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return SolveFor(RunProgram, 1).ToString();
    }

    public string SolveB()
    {
        return SolveFor(RunProgramB, 5).ToString();
    }
    private int SolveFor(Func<List<int>, int, int> solver, int input)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var start = ParseInput(lines[0]);
        return solver(start, input);
    }
}
