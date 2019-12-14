using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;

class Solve13 : ISolve
{
    public string Description()
    {
        return "Day 13: Care Package";
    }

    const string Input = "Input//13.txt";

    public bool Prove(bool isA)
    {
        return true;
    }


    public string Solve(bool isA)
    {
        return isA ? SolveA() : "Not Impl";
    }

    public string SolveA()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
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

        int count = 0;
        for (int i = 2; i < outputs.Count; i += 3)
        {
            if (outputs[i] == 2)
            {
                count++;
            }
        }

        return count.ToString();
    }
}
