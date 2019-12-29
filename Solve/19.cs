using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;
using IntType = System.Int64;

class Solve19 : ISolve
{
    public string Description()
    {
        return "Day 19: Tractor Beam";
    }

    const string Input = "Input//19.txt";

    public bool Prove(bool isA)
    {
        return isA ? true : ProveB();
    }

    public bool ProveB()
    {
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? CountTractor(50, 50) : SolveB();
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
                tractor[new Point(x, y)] = IsTractorOn(program, x, y);
            }
        }
        // DumpTractor(tractor);
        int count = tractor.Sum(t => t.Value ? 1 : 0);
        return count.ToString();
    }

    bool IsTractorOn(IntType[] program, int x, int y)
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
                return output.Value != 0;
            }
        }
        throw new Exception("Whaaaa?");
    }

    Point CalculateAnswer(IntType[] program, Point p1, Point p2, int dimension)
    {
        var m1 = (double)p1.Y / (double)p1.X;
        var m2 = (double)p2.Y / (double)p2.X;

        var x1 = ((double)dimension * m1 + (double)dimension) / (m2 - m1);
        var y1 = m1 * (x1 + (double)dimension);

        var p = new Point((int)Math.Round(x1), (int)Math.Round(y1));
        // Console.WriteLine($"p1: {p1}, p2: {p2}, m1: {m1}, m2: {m2}, ({x1},{y1}), final: {p}");

        // Dumb as bricks: just search around it. 
        var tries = new List<Point>();
        var buf = 75;
        for (var x = p.X - buf; x < p.X + buf; x++)
        {
            for (var y = p.Y - buf; y < p.Y + buf; y++)
            {
                tries.Add(new Point(x, y));
            }
        }
        var sorted = tries.OrderBy(t => t.X + t.Y);
        foreach (var t in sorted)
        {
            if (CheckSimple(program, t, dimension))
            {
                return t;
            }
        }

        return p;
    }

    bool CheckSimple(IntType[] program, Point p, int dimension)
    {
        var c1 = p;
        var c2 = new Point(p.X + dimension, p.Y);
        var c3 = new Point(p.X, p.Y + dimension);
        var c4 = new Point(p.X + dimension, p.Y + dimension);

        return Check(program, c1, true) &&
            Check(program, c2, true) &&
            Check(program, c3, true) &&
            Check(program, c4, true);
    }

    bool Check(IntType[] program, Point point, bool isOn)
    {
        bool tractorOn = IsTractorOn(program, point.X, point.Y);
        return isOn == tractorOn;
    }

    string SolveB()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);

        var answer = CalculateAnswer(program, new Point(150, 119), new Point(150, 143), 99);
        return $"{answer.X * 10000 + answer.Y}";

        // too low:    9801125
        // NO:        10530911
        // NO:        10880934
        // NO:        10580916
        // ANSWER:    10450905
        // too high:  11125945
        // too high:  11250980
    }

    void DumpTractor(Dictionary<Point, bool> tractor)
    {
        var maxX = tractor.Max(t => t.Key.X);
        var minX = tractor.Min(t => t.Key.X);
        var maxY = tractor.Max(t => t.Key.Y);
        var minY = tractor.Min(t => t.Key.Y);
        for (var y = minY; y < maxY; y++)
        {
            Console.WriteLine();
            for (var x = minX; x < maxX; x++)
            {
                Console.Write(tractor.GetValueOrDefault(new Point(x, y)) ? '#' : '.');
            }
        }
        Console.WriteLine();
    }
}
