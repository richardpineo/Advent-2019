using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve19 : ISolve
{
    public string Description()
    {
        return "Day 19: Tractor Beam";
    }

    const string Input = "Input//19.txt";

    public bool Prove(bool isA)
    {
        return isA ? true : true;
    }

    public string Solve(bool isA)
    {
        return isA ? CountTractor(50, 50) : "NOTIMPL";
    }

    public string CountTractor(int xDim, int yDim)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var tractor = new Dictionary<Point, bool>();
        for (var x = 0; x < xDim; x++)
        {
            for (var y = 0; y < yDim; y++)
            {
                var state = new Intcode.State(program);
                state.input = x;
                while (Intcode.Step(state))
                {
                    if (!state.input.HasValue)
                    {
                        state.input = y;
                    }

                    var output = state.PopOutput();
                    if (output.HasValue)
                    {
                        tractor[new Point(x, y)] = output.Value != 0;
                    }
                }
            }
        }
        int count = tractor.Sum(t => t.Value ? 1 : 0);
        return count.ToString();
    }
}
