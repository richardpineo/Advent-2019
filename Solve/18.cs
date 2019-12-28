using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;
using Point = System.Drawing.Point;
using System.Linq;

class Solve18 : ISolve
{
    public string Description()
    {
        return "Day 18: Many-Worlds Interpretation";
    }

    const string Input = "Input//18.txt";

    enum Movement
    {
        North = 1,
        South = 2,
        West = 3,
        East = 4,
    };

    class Map
    {
        public void SetAt(Point p, char c)
        {
            Entries[p] = c;
            if (char.IsLower(c) && char.IsLetter(c))
            {
                AllKeys.Add(c);
            }
            if (c == Start)
            {
                StartingPosition = p;
            }
        }

        public int Solve()
        {
            // Recurse, walking in all directions, until all the keys are found.
            int? minSteps = null;
            Step(StartingPosition, new List<char>(), new List<Point>(), new List<Point>(), ref minSteps);
            return minSteps.Value;
        }

        Point move(Movement movement, Point loc)
        {
            switch (movement)
            {
                case Movement.North:
                    return new Point(loc.X, loc.Y - 1);
                case Movement.South:
                    return new Point(loc.X, loc.Y + 1);
                case Movement.West:
                    return new Point(loc.X - 1, loc.Y);
                case Movement.East:
                    return new Point(loc.X + 1, loc.Y);
            }
            throw new Exception("Invalid movement");
        }

        void Step(Point currentPos, List<char> foundKeys, List<Point> steps, List<Point> stepsSinceLastKey, ref int? minSteps)
        {
            if (foundKeys.Count == AllKeys.Count)
            {
                minSteps = steps.Count + stepsSinceLastKey.Count;
                return;
            }

            if (minSteps.HasValue && ((stepsSinceLastKey.Count + steps.Count + 1) >= minSteps.Value))
            {
                return;
            }

            // Try walking in all directions
            Random rnd = new Random();
            Point[] possiblePaths = new Point[] {
                move(Movement.North, currentPos),
                move(Movement.South, currentPos),
                move(Movement.East, currentPos),
                move(Movement.West, currentPos)
            };
            possiblePaths = possiblePaths.Where(p => CanGo(p, stepsSinceLastKey, foundKeys)).OrderBy(x => rnd.Next()).ToArray();

            foreach (var possible in possiblePaths)
            {
                var keyAt = KeyAt(possible);

                var stackSteps = steps;
                var stackFoundKeys = foundKeys;
                var stackStepsSinceLastKey = new List<Point>(stepsSinceLastKey);
                stackStepsSinceLastKey.Add(possible);

                if (keyAt.HasValue && !foundKeys.Contains(keyAt.Value))
                {
                    stackFoundKeys = new List<char>(foundKeys);
                    stackFoundKeys.Add(keyAt.Value);

                    stackSteps = new List<Point>(steps);
                    stackSteps.AddRange(stackStepsSinceLastKey);
                    stackStepsSinceLastKey = new List<Point>();

                    Console.WriteLine($"{minSteps} {stackSteps.Count} {stackFoundKeys.Count} Keys: {string.Join(",", stackFoundKeys)}");
                    // Console.WriteLine($"{minSteps} Steps: {string.Join(",", stackSteps)}");
                }
                Step(possible, stackFoundKeys, stackSteps, stackStepsSinceLastKey, ref minSteps);
            }
        }

        char? KeyAt(Point p)
        {
            var entry = Entries.GetValueOrDefault(p, (char)666);
            if (char.IsLetter(entry) && char.IsLower(entry))
            {
                return entry;
            }
            return null;
        }

        bool CanGo(Point p, List<Point> steps, List<char> foundKeys)
        {
            var entry = Entries.GetValueOrDefault(p, (char)666);
            if (entry == 666)
            {
                return false;
            }
            if (steps.Contains(p))
            {
                return false;
            }

            if (entry == Wall)
            {
                return false;
            }
            if (char.IsLetter(entry))
            {
                if (char.IsUpper(entry))
                {
                    return foundKeys.Contains(char.ToLower(entry));
                }
                return true;
            }
            if (entry == Start || entry == Open)
            {
                return true;
            }
            throw new Exception("What is this? " + entry);
        }

        HashSet<char> AllKeys = new HashSet<char>();

        const char Start = '@';
        const char Wall = '#';
        const char Open = '.';

        Dictionary<Point, char> Entries = new Dictionary<Point, char>();

        Point StartingPosition;
    }


    public bool Prove(bool isA)
    {
        return isA ? ProveA() : true;
    }

    public bool ProveA()
    {

        string[] Examples = new string[] {
            "Examples//18-4.txt",
            "Examples//18-1.txt",
            "Examples//18-2.txt",
            "Examples//18-3.txt",
          //  "Examples//18-4.txt",
            "Examples//18-5.txt",
        };
        foreach (var example in Examples)
        {
            var lines = File.ReadAllLines(example, Encoding.UTF8);
            var steps = int.Parse(lines[0]);
            var map = new Map();
            for (int row = 1; row < lines.Length; row++)
            {
                for (int col = 0; col < lines[row].Length; col++)
                {
                    map.SetAt(new Point(col, row - 1), lines[row][col]);
                }
            }

            int calculatedSteps = map.Solve();
            if (steps != calculatedSteps)
            {
                Console.WriteLine($"{example} found {calculatedSteps} expected {steps}");
                return false;
            }
        }
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var map = new Map();
        for (int row = 0; row < lines.Length; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                map.SetAt(new Point(col, row), lines[row][col]);
            }
        }

        int calculatedSteps = map.Solve();
        return calculatedSteps.ToString();
    }

    public string SolveB()
    {
        return "NOPE";
    }
    Point move(Movement movement, Point loc)
    {
        switch (movement)
        {
            case Movement.North:
                return new Point(loc.X, loc.Y + 1);
            case Movement.South:
                return new Point(loc.X, loc.Y - 1);
            case Movement.West:
                return new Point(loc.X - 1, loc.Y);
            case Movement.East:
                return new Point(loc.X + 1, loc.Y);
        }
        throw new Exception("Invalid movement");
    }
}
