using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve3 : ISolve
{
    public string Description()
    {
        return "Day 3: Crossed Wires";
    }

    const string Input = "Input//3.txt";
    const string ExampleA = "Examples//3a.txt";
    const string ExampleB = "Examples//3b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveFor(ExampleA, FindMinDistance) : ProveFor(ExampleB, FindMinSteps);
    }

    private bool ProveFor(string file, Func<Point[], Point[], int> minFn)
    {
        string[] lines = File.ReadAllLines(file, Encoding.UTF8);
        for (int i = 0; i < lines.Length; i += 3)
        {
            var expected = int.Parse(lines[i + 2]);

            var points1 = MakePoints(lines[i]);
            var points2 = MakePoints(lines[i + 1]);
            var actual = minFn(points1, points2);

            if (expected != actual)
            {
                Console.WriteLine();
                Console.WriteLine($"  Expected: {expected}");
                Console.WriteLine($"  Actual:   {actual}");
                return false;
            }
        }
        return true;
    }

    private int FindMinSteps(Point[] wire1, Point[] wire2)
    {
        return 0;
    }

    private int FindMinDistance(Point[] wire1, Point[] wire2)
    {
        var intersections = FindIntersections(wire1, wire2);
        var distances = intersections.Select(intersection => Math.Abs(intersection.X) + Math.Abs(intersection.Y));
        return distances.Min();
    }

    private Point[] FindIntersections(Point[] wire1, Point[] wire2)
    {
        var intersections = wire2.Intersect(wire1);
        return intersections.ToArray();
    }

    private Point[] MakePoints(string line)
    {
        var points = new List<Point>();
        var commands = line.Split(",");
        var position = new Point(0, 0);
        foreach (var command in commands)
        {
            var newPoints = ProcessCommand(position, command);
            points.AddRange(newPoints);
            position = newPoints[newPoints.Length - 1];
        }
        return points.ToArray();
    }

    private Point[] ProcessCommand(Point position, string command)
    {
        char c = command[0];
        int count = int.Parse(command.Substring(1));
        Func<Point, Point> mod;
        switch (c)
        {
            case 'U':
                mod = p => new Point(p.X, p.Y + 1);
                break;
            case 'D':
                mod = p => new Point(p.X, p.Y - 1);
                break;
            case 'L':
                mod = p => new Point(p.X - 1, p.Y);
                break;
            case 'R':
                mod = p => new Point(p.X + 1, p.Y);
                break;
            default:
                throw new Exception("Bad command: " + c);
        }
        var points = new List<Point>();
        for (int i = 0; i < count; i++)
        {
            position = mod(position);
            points.Add(position);
        }
        return points.ToArray();
    }

    public string Solve(bool isA)
    {
        return isA ? SolveFor(FindMinDistance) : SolveFor(FindMinSteps);
    }

    private string SolveFor(Func<Point[], Point[], int> minFn)
    {
        string[] lines = File.ReadAllLines(Input, Encoding.UTF8);
        var points1 = MakePoints(lines[0]);
        var points2 = MakePoints(lines[1]);

        var actual = FindMinDistance(points1, points2);
        return actual.ToString();
    }
}
