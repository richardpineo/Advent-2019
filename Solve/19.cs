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
        Console.WriteLine();
        // x = col+1
        // y = ln
        // var example1 = CalculateAnswer("example 1", new Point(35, 20), new Point(25, 30), 10);
        // var example2 = CalculateAnswer("example 2", new Point(42, 24), new Point(30, 36), 10);

        //var real1 = CalculateAnswer("real 1   ", new Point(39, 23), new Point(33, 35), 100);
        //var real2 = CalculateAnswer("real 2   ", new Point(36, 21), new Point(26, 31), 100);

        var real3 = CalculateAnswer("real 150   ", new Point(150, 119), new Point(150, 143), 99);
        Console.WriteLine($"Is it {real3.X * 10000 + real3.Y}");
        // var real4 = CalculateAnswer("real 150   ", new Point(151, 119), new Point(151, 143), 100);
        // var real5 = CalculateAnswer("real 200   ", new Point(201, 158), new Point(201, 191), 100);
        return true;
        // answerProve == new Point(25, 20);
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

    Point CalculateAnswer(string desc, Point p1, Point p2, int dimension)
    {
        var m1 = (double)p1.Y / (double)p1.X;
        var m2 = (double)p2.Y / (double)p2.X;

        var x1 = ((double)dimension * m1 + (double)dimension) / (m2 - m1);
        var y1 = m1 * (x1 + (double)dimension);

        var p = new Point((int)Math.Round(x1), (int)Math.Round(y1));
        Console.WriteLine($"{desc}: p1: {p1}, p2: {p2}, m1: {m1}, m2: {m2}, ({x1},{y1}), final: {p}");

        // CheckFinal(p, dimension);
        // CheckFinal(new Point(p.X - 1, p.Y - 1), dimension);
        //  CheckFinal(new Point(p.X - 2, p.Y - 2), dimension);
        //CheckFinal(new Point(p.X - 3, p.Y - 3), dimension);
        //CheckFinal(new Point(p.X - 4, p.Y - 4), dimension);
        //CheckFinal(new Point(p.X - 5, p.Y - 5), dimension);
        // CheckFinal(new Point(p.X, p.Y), dimension);


        var tries = new List<Point>();
        var buf = 100;
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
            if (CheckSimple(t, dimension))
            {
                return t;
            }
        }

        return p;
    }

    bool CheckSimple(Point p, int dimension)
    {
        var c1 = p;
        var c2 = new Point(p.X + dimension, p.Y);
        var c3 = new Point(p.X, p.Y + dimension);
        var c4 = new Point(p.X + dimension, p.Y + dimension);

        return Check(c1, true) &&
            Check(c2, true) &&
            Check(c3, true) &&
            Check(c4, true);
    }

    void CheckFinal(Point p, int dimension)
    {

        var c1 = p;
        var c2 = new Point(p.X + dimension - 1, p.Y);
        var c3 = new Point(p.X, p.Y + dimension - 1);
        var c4 = new Point(p.X + dimension - 1, p.Y + dimension - 1);


        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var tractor = new Dictionary<Point, bool>();

        for (var x = p.X - 10; x < p.X + dimension + 10; x++)
        {
            for (var y = p.Y - 10; y < p.Y + dimension + 10; y++)
            {
                tractor[new Point(x, y)] = IsTractorOn(program, x, y);
            }
        }
        DumpTractor(tractor);

        Console.WriteLine($"{c1} {c2} {c3} {c4}");

        Console.WriteLine("Corners:");
        Check(c1, true);
        Check(c2, true);
        Check(c3, true);
        Check(c4, true);

        Console.WriteLine("Just a bit outside:");
        Check(new Point(c2.X + 1, c2.Y), false);
        Check(new Point(c2.X, c2.Y - 1), false);
        Check(new Point(c3.X - 1, c3.Y), false);
        Check(new Point(c3.X, c3.Y + 1), false);
    }

    bool Check(Point point, bool isOn)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        bool tractorOn = IsTractorOn(program, point.X, point.Y);
        if (isOn != tractorOn)
        {
            Console.WriteLine($"{point} Fail. Should be {isOn}, is {tractorOn}");
        }
        else
        {
            Console.WriteLine($"{point} Success");
        }
        return isOn == tractorOn;
    }

    string SolveB()
    {
        //CountTractor(200, 200);

        //var p1 = new Point(200, 160);
        //var p2 = new Point(200, 192);

        //var p1 = new Point(100, 80);
        //var p2 = new Point(100, 96);

        //var real4 = CalculateAnswer("real 4   ", new Point(201, 158), new Point(201, 191), 10);
        var p1 = new Point(201, 158);
        var p2 = new Point(201, 191);
        return "foo";

        var answer = CalculateAnswer("solveb", p1, p2, 100);


        var final = answer.X * 10000 + answer.Y;

        Console.WriteLine($"\nAnswer is {answer}: {final}");
        return (answer.X * 10000 + answer.Y).ToString();

        // too low:    9801125
        // NO:        10530911
        // NO:        10880934
        //            10580916
        //            10450905
        // too high:  11125945
        // too high:  11250980
        // 100093
        // 
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
