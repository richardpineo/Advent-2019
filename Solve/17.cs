using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve17 : ISolve
{
    public string Description()
    {
        return "Day 17: Set and Forget";
    }

    const string Input = "Input//17.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    enum Values
    {
        Space = '.',
        Scaffold = '#',
        FaceUp = '^',
        FaceDown = 'v',
        FaceLeft = '<',
        FaceRight = '>',
    }

    private bool ProveA()
    {
        string[] example = {
            "..#..........",
            "..#..........",
            "#######...###",
            "#.#...#...#.#",
            "#############",
            "..#...#...#..",
            "..#####...^.."
        };
        var rows = example[0].Length;
        var cols = example.Length;
        var environment = new Dictionary<Point, Values>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                environment[new Point(row, col)] = (Values)example[col][row];
            }
        }

        var intersections = FindIntersections(environment, rows, cols);
        var answer = intersections.Sum(p => p.X * p.Y);

        return answer == 76;
    }

    private Point[] FindIntersections(Dictionary<Point, Values> environment, int numRows, int numCols)
    {
        var points = new List<Point>();
        // Walk througb the environment
        for (int row = 1; row < numRows; row++)
        {
            for (int col = 1; col < numCols; col++)
            {
                // Check the location and surrounding values 
                if (ValueAt(new Point(row, col), environment) != Values.Scaffold ||
                    ValueAt(new Point(row + 1, col), environment) != Values.Scaffold ||
                    ValueAt(new Point(row - 1, col), environment) != Values.Scaffold ||
                    ValueAt(new Point(row, col + 1), environment) != Values.Scaffold ||
                    ValueAt(new Point(row, col - 1), environment) != Values.Scaffold)
                {
                    continue;
                }

                // Found one.
                points.Add(new Point(row, col));
            }
        }
        return points.ToArray();
    }

    private Values ValueAt(Point point, Dictionary<Point, Values> environment)
    {
        return environment.GetValueOrDefault(point, Values.Space);
    }

    private bool ProveB()
    {
        return false;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return "not impl";
    }

    public string SolveB()
    {
        return "not impl";
    }
}
