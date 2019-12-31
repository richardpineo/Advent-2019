using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve21 : ISolve
{
    public string Description()
    {
        return "Day 21: Springdroid Adventure";
    }

    const string Input = "Input//21.txt";

    public bool Prove(bool isA)
    {
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        var instructions = new string[] {
            "NOT A J",
            "NOT B T",
            "OR T J",
            "NOT C T",
            "OR T J",
            "AND D J",
            "WALK",
            ""
        };
        return execute(instructions);
    }

    public string execute(string[] instructions)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);

        var input = string.Join('\n', instructions);
        var ascii = new List<long>();

        var index = 0;
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                ascii.Add(output.Value);
            }
            if (!state.input.HasValue && index < input.Length)
            {
                state.input = input[index++];
            }
        }
        var lastVal = ascii[ascii.Count - 1];
        if (lastVal < char.MaxValue)
        {
            var asciiText = ascii.Select(a => (char)a).Take(ascii.Count - 1);
            Console.WriteLine(string.Join("", asciiText));
            return "Sad death";
        }
        return lastVal.ToString();
    }
    public string SolveB()
    {
        var instructions = new string[] {
            "NOT C J",
            "AND D J",
            "AND H J",
            "NOT B T",
            "AND D T",
            "OR T J",
            "NOT A T",
            "OR T J",
            "RUN",
            ""
        };
        return execute(instructions);
    }
}
