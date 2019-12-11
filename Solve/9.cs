using System;
using System.Collections.Generic;
using IntType = System.Int64;

class Solve9 : ISolve
{
    public string Description()
    {
        return "Day 9: Sensor Boost";
    }

    const string Input = "Input//9.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        return ProveA1() && ProveA2() && ProveA3();
    }

    private bool ProveA1()
    {
        var input = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
        var program = Intcode.ParseInput(input);
        var state = new Intcode.State(program);
        var outputs = new List<IntType>();
        while (Intcode.Step(state))
        {
            if (state.output.HasValue)
            {
                var output = state.PopOutput();
                if (output.HasValue)
                {
                    outputs.Add(output.Value);
                }
            }
        }
        var final = string.Join(",", outputs);
        if (final != input)
        {
            Console.WriteLine();
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Final: {final}");
            return false;
        }
        return true;
    }
    private bool ProveA2()
    {
        var input = "1102,34915192,34915192,7,4,7,99,0";
        var program = Intcode.ParseInput(input);
        var state = new Intcode.State(program);
        while (Intcode.Step(state))
        {
            // nothing
        }
        return state.PopOutput().Value.ToString().Length == 16;
    }
    private bool ProveA3()
    {
        var input = "104,1125899906842624,99";
        var program = Intcode.ParseInput(input);
        var state = new Intcode.State(program);
        while (Intcode.Step(state))
        {
            // nothing
        }
        return state.output == 1125899906842624;
    }

    public bool ProveB()
    {
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return "NOTIMPL";
    }

    public string SolveB()
    {
        return "NOTIMPL";
    }
}
