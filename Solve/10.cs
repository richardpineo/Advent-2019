using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.PointF;

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

        if (!StarMap.Test())
        {
            return false;
        }

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

        public static bool Test()
        {
            if (!StarMap.PointOnLineSegment(new Point(3, 4), new Point(1, 0), new Point(2, 2)))
            {
                return false;
            }
            return true;
        }

        // https://stackoverflow.com/questions/7050186/find-if-point-lays-on-line-segment/25689069
        private static bool PointOnLineSegment(Point pt1, Point pt2, Point pt, double epsilon = 0.0001)
        {
            if (pt.X - Math.Max(pt1.X, pt2.X) > epsilon ||
                Math.Min(pt1.X, pt2.X) - pt.X > epsilon ||
                pt.Y - Math.Max(pt1.Y, pt2.Y) > epsilon ||
                Math.Min(pt1.Y, pt2.Y) - pt.Y > epsilon)
                return false;

            if (Math.Abs(pt2.X - pt1.X) < epsilon)
                return Math.Abs(pt1.X - pt.X) < epsilon || Math.Abs(pt2.X - pt.X) < epsilon;
            if (Math.Abs(pt2.Y - pt1.Y) < epsilon)
                return Math.Abs(pt1.Y - pt.Y) < epsilon || Math.Abs(pt2.Y - pt.Y) < epsilon;

            double x = pt1.X + (pt.Y - pt1.Y) * (pt2.X - pt1.X) / (pt2.Y - pt1.Y);
            double y = pt1.Y + (pt.X - pt1.X) * (pt2.Y - pt1.Y) / (pt2.X - pt1.X);

            return Math.Abs(pt.X - x) < epsilon || Math.Abs(pt.Y - y) < epsilon;
        }

        public bool isVisible(Point star, Point toCheck)
        {
            if (star == toCheck)
            {
                return false;
            }

            // Walk through all the other stars to see if anything is in between
            foreach (Point inBetween in stars)
            {
                if (inBetween != star && inBetween != toCheck)
                {
                    if (PointOnLineSegment(star, toCheck, inBetween))
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        public void Solve()
        {
            foreach (var star in stars)
            {
                var numVisible = 0;
                foreach (var toCheck in stars)
                {
                    if (isVisible(star, toCheck))
                    {
                        numVisible++;
                    }
                }
                numVisiblePerStar[star] = numVisible;
            }

            int max = numVisiblePerStar.Max(p => p.Value);
            var bestStar = numVisiblePerStar.First(p => p.Value == max).Key;
        }

        public int maxStarsVisible
        {
            get
            {
                return numVisiblePerStar.Max(p => p.Value);
            }
        }

        public Point bestStar
        {
            get
            {
                return numVisiblePerStar.First(p => p.Value == maxStarsVisible).Key;
            }
        }

        public Dictionary<Point, int> numVisiblePerStar = new Dictionary<Point, int>();

        public List<Point> stars = new List<Point>();
    }

    bool ProveFor(string test)
    {
        var lines = File.ReadAllLines(test, Encoding.UTF8);
        var bestLoc = lines[0].Split(",").Select(x => int.Parse(x));
        var count = int.Parse(lines[1]);
        var map = lines.Skip(2).ToArray();
        var starMap = StarMap.Parse(map);
        starMap.Solve();

        var bestStar = starMap.bestStar;
        return bestStar.X == bestLoc.ElementAt(0) && bestStar.Y == bestLoc.ElementAt(1) && starMap.maxStarsVisible == count;
    }

    public string Solve(bool isA)
    {
        if (isA)
        {
            var lines = File.ReadAllLines(Input, Encoding.UTF8);
            var starMap = StarMap.Parse(lines);
            starMap.Solve();
            return starMap.maxStarsVisible.ToString();
        }
        else
        {
            return "Not Impl";
        }
    }
}
