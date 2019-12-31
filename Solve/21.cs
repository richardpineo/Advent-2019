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

    private char[] ConvertToInstructions(string input)
    {
        return input.ToCharArray();
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    private void OutputAscii(string ascii)
    {
        Console.Write(ascii);
    }

    public string SolveA()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);

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
        var asciiText = ascii.Select(a => (char)a).Take(ascii.Count - 1);
        Console.WriteLine(asciiText);
        return lastVal.ToString();
    }

    public string SolveB()
    {
        return "";
    }
}
