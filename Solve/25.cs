using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve25 : ISolve
{
    public string Description()
    {
        return "Day 25: Cryostasis";
    }

    const string Input = "Input//25.txt";

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
        interactive();
        return "Manual";
    }

    public void interactive()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);

        var input = "";
        var fullTrace = new StringBuilder();
        var lastLine = new StringBuilder();

        var index = 0;
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                char c = (char)output.Value;
                Console.Write(c);
                fullTrace.Append(c);
                switch (c)
                {
                    case '\n':
                        if (lastLine.ToString() == "Command?")
                        {
                            // Waiting for input.
                            input = Console.ReadLine();
                            // Console.WriteLine("Input received: " + input);
                            input += '\n';
                            index = 0;
                        }
                        lastLine.Clear();
                        break;
                    default:
                        lastLine.Append(c);
                        break;
                }
            }
            if (!state.input.HasValue && index < input.Length)
            {
                state.input = input[index++];
            }
        }
    }
    public string SolveB()
    {
        return "Manual";
    }
}
