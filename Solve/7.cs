using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

            var answer = int.Parse(tokens[0]);

            // Array of 5 int
            var order = tokens[1].Split(",").Select(t => int.Parse(t));

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

            var answer = int.Parse(tokens[0]);

            // Array of 5 int
            var order = tokens[1].Split(",").Select(t => int.Parse(t));

            var power = ComputePowerFeedback(order.ToArray(), tokens[2]);

            if (power != answer)
            {
                Console.WriteLine($"{power} should have been {answer}, {tokens[2]}");
                return false;
            }
        }

        return true;
    }

    private int ComputePower(int[] order, string rawProgram)
    {
        var power = 0;
        foreach (var phase in order)
        {
            int[] inputs = { phase, power };

            var program = ParseInput(rawProgram);

            var inputPos = 0;
            var state = new State { input = inputs[inputPos++], program = program.ToArray() };
            for (; state.pos != -1; state.pos = Step(state))
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

    private int ComputePowerFeedback(int[] order, string rawProgram)
    {
        var states = new List<State>();
        states.Add(new State { input = order[0], program = ParseInput(rawProgram).ToArray() });
        states.Add(new State { input = order[1], program = ParseInput(rawProgram).ToArray() });
        states.Add(new State { input = order[2], program = ParseInput(rawProgram).ToArray() });
        states.Add(new State { input = order[3], program = ParseInput(rawProgram).ToArray() });
        states.Add(new State { input = order[4], program = ParseInput(rawProgram).ToArray() });

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
                    states[i].pos = Step(states[i]);
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
                        states[toState].input = states[i].output;
                        states[i].output = null;
                    }
                }
            }
        }
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

    class State
    {
        public int? output;
        public int? input;
        public int pos;
        public int[] program;
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

    private int Step(State state)
    {
        ref int pos = ref state.pos;
        var program = state.program;
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
                return pos + read(program[pos + 1], ref state.input, program);
            case 4:
                return pos + write(modes, program[pos + 1], program, ref state.output);
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

    public int add(bool[] modes, int op1, int op2, int op3, int[] program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 + val2;
        return 4;
    }
    public int multiply(bool[] modes, int op1, int op2, int op3, int[] program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 * val2;
        return 4;
    }
    public int read(int op1, ref int? input, int[] program)
    {
        if (!input.HasValue)
        {
            return 0;
        }
        program[op1] = input.Value;
        input = null;
        return 2;
    }
    public int write(bool[] modes, int op1, int[] program, ref int? output)
    {
        if (output.HasValue)
        {
            return 0;
        }
        output = modes[0] ? op1 : program[op1]; ;
        return 2;
    }
    public int jumpTrue(bool[] modes, int pos, int op1, int op2, int[] program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        return val1 != 0 ? val2 : (pos + 3);
    }
    public int jumpFalse(bool[] modes, int pos, int op1, int op2, int[] program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        return val1 == 0 ? val2 : (pos + 3);
    }
    public int lessThan(bool[] modes, int op1, int op2, int op3, int[] program)
    {
        int val1 = modes[0] ? op1 : program[op1];
        int val2 = modes[1] ? op2 : program[op2];
        program[op3] = val1 < val2 ? 1 : 0;
        return 4;
    }
    public int equals(bool[] modes, int op1, int op2, int op3, int[] program)
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
        int[] boosters = { 0, 1, 2, 3, 4 };
        return SolveFor(boosters, ComputePower).ToString();
    }

    public string SolveB()
    {
        int[] boosters = { 5, 6, 7, 8, 9 };
        return SolveFor(boosters, ComputePowerFeedback).ToString();
    }

    // https://stackoverflow.com/questions/1952153/what-is-the-best-way-to-find-all-combinations-of-items-in-an-array/10629938#10629938
    static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new T[] { t });
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(o => !t.Contains(o)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    private int SolveFor(int[] boosters, Func<int[], string, int> computer)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var rawProgram = lines[0];

        var maxPower = 0;

        var attempts = GetPermutations(boosters, 5);

        foreach (var attempt in attempts)
        {
            var power = computer(attempt.ToArray(), rawProgram);
            maxPower = Math.Max(power, maxPower);
        }

        return maxPower;
    }
}
