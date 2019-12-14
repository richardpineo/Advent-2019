using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve10 : ISolve
{
    public string Description()
    {
        return "Day 10: Monitoring Station";
    }

    const string Input = "Input//10.txt";

    public bool Prove(bool isA)
    {
        var tests = new string[] {
            "Examples//10-1.txt",
            "Examples//10-2.txt",
            "Examples//10-3.txt",
            "Examples//10-4.txt",
            "Examples//10-5.txt"
        };

        foreach (var test in tests)
        {
            if (!ProveFor(test))
            {
                return false;
            }
        }

        return true;
    }

    class StarMap
    {
        public static StarMap Parse(string[] lines)
        {
            var starMap = new StarMap();
            for (int y = 0; y < lines.Count(); y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        starMap.stars.Add(new Point(x, y));
                    }
                }
            }
            return starMap;
        }

        private HashSet<Point> stars = new HashSet<Point>();
    }

    bool ProveFor(string test)
    {
        var lines = File.ReadAllLines(test, Encoding.UTF8);
        var bestLoc = lines[0].Split(",").Select(x => int.Parse(x));
        var count = int.Parse(lines[1]);
        var map = lines.Skip(2).ToArray();
        var starMap = StarMap.Parse(map);

        return true;
    }

    public string Solve(bool isA)
    {
        return "Not Impl";
    }
}
