using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class Solve24 : ISolve
{
    public string Description()
    {
        return "Day 24: Planet of Discord";
    }

    const string Input = "Input//24.txt";
    const string Example = "Examples//24.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    public bool ProveA()
    {
        var eris = new Eris();
        var lines = File.ReadAllLines(Example, Encoding.UTF8);
        for (var y = 0; y < 5; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                eris.SetBug(x, y, lines[y][x] == '#');
            }
        }
        return SimpleSolution(eris, false) == 2129920;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    class Eris
    {
        public bool GetBug(int x, int y)
        {
            if (x < 0 || x >= 5 || y < 0 || y >= 5)
            {
                return false;
            }
            return bugs[y * 5 + x];
        }
        public void SetBug(int x, int y, bool isBug)
        {
            bugs[y * 5 + x] = isBug;
        }

        public long Hash()
        {
            long hash = 0;
            for (var i = 0; i < 25; i++)
            {
                if (bugs[i])
                {
                    hash += PowerOf2(i);
                }
            }
            return hash;
        }

        public void Trace()
        {
            Console.WriteLine("");
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    Console.Write(GetBug(x, y) ? "#" : ".");
                }
                Console.WriteLine("");
            }
            Console.WriteLine(Hash().ToString());
        }

        int NeighboringBugs(int x, int y)
        {
            var bugs = new bool[] {
                GetBug(x-1, y),
                GetBug(x+1, y),
                GetBug(x, y-1),
                GetBug(x, y+1),
            };
            return bugs.Sum(b => b ? 1 : 0);
        }

        public Eris Iterate()
        {
            var next = new Eris();
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    int neighbors = NeighboringBugs(x, y);
                    bool hasBug = GetBug(x, y);
                    bool isBug = hasBug ? (neighbors == 1) : (neighbors == 1 || neighbors == 2);
                    next.SetBug(x, y, isBug);
                }
            }
            return next;
        }

        bool[] bugs = new bool[25];
    }

    private string SolveA()
    {
        var eris = new Eris();
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        for (var y = 0; y < 5; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                eris.SetBug(x, y, lines[y][x] == '#');
            }
        }
        return SimpleSolution(eris, false).ToString();
    }

    private long SimpleSolution(Eris eris, bool trace)
    {
        var patterns = new HashSet<long>();
        var next = eris;
        while (true)
        {
            var hash = next.Hash();
            if (patterns.Contains(hash))
            {
                return hash;
            }
            patterns.Add(hash);
            next = next.Iterate();

            if (trace)
            {
                next.Trace();
            }
        }
    }

    static long PowerOf2(long power)
    {
        long result = 1;
        for (long i = 0; i < power; i++)
            result *= 2;
        return result;
    }

    class MultiEris
    {
        public class Index : IEquatable<Index>
        {
            public int Depth;
            public int X;
            public int Y;

            public bool Equals(MultiEris.Index other)
            {
                return other != null &&
                    other.Depth == Depth &&
                    other.X == X &&
                    other.Y == Y;
            }

            public override int GetHashCode()
            {
                return X * Y + Depth * 999999;
            }
            public bool IsValid
            {
                get
                {
                    if (X < 0 || X >= 5 || Y < 0 || Y >= 5)
                    {
                        return false;
                    }
                    if (X == 2 && Y == 2)
                    {
                        return false;
                    }

                    return true;
                }
            }
        }

        HashSet<Index> bugs = new HashSet<Index>();

        public bool GetContent(Index index)
        {
            if (!index.IsValid)
            {
                return false;
            }
            var content = bugs.Contains(index);
            return content;
        }
        public void SetContent(Index index, bool content)
        {
            bugs.Add(index);
        }

        public int BugCount
        {
            get
            {
                return bugs.Count;
            }
        }

        /*
                public void Trace()
                {
                    Console.WriteLine("");
                    for (var y = 0; y < 5; y++)
                    {
                        for (var x = 0; x < 5; x++)
                        {
                            Console.Write(GetBug(x, y) ? "#" : ".");
                        }
                        Console.WriteLine("");
                    }
                    Console.WriteLine(Hash().ToString());
                }
        */

        public static Index[] LeftNeighbors(Index index)
        {
            // Left
            if (index.X == 0)
            {
                // Up one depth
                return new Index[] { new Index { Depth = index.Depth - 1, X = 1, Y = 2 } };
            }
            else if (index.X == 3 && index.Y == 2)
            {
                // In one depth
                return new Index[] {
                    new Index { Depth = index.Depth + 1, X = 4, Y = 0 },
                    new Index { Depth = index.Depth + 1, X = 4, Y = 1 },
                    new Index { Depth = index.Depth + 1, X = 4, Y = 2 },
                    new Index { Depth = index.Depth + 1, X = 4, Y = 3 },
                    new Index { Depth = index.Depth + 1, X = 4, Y = 4 }
                };
            }
            else
            {
                return new Index[] { new Index { Depth = index.Depth, X = index.X - 1, Y = index.Y } };
            }
        }

        public static Index[] RightNeighbors(Index index)
        {
            if (index.X == 4)
            {
                // Up one depth
                return new Index[] { new Index { Depth = index.Depth - 1, X = 3, Y = 2 } };
            }
            else if (index.X == 1 && index.Y == 2)
            {
                // In one depth
                return new Index[] {
                    new Index { Depth = index.Depth + 1, X = 0, Y = 0 },
                    new Index { Depth = index.Depth + 1, X = 0, Y = 1 },
                    new Index { Depth = index.Depth + 1, X = 0, Y = 2 },
                    new Index { Depth = index.Depth + 1, X = 0, Y = 3 },
                    new Index { Depth = index.Depth + 1, X = 0, Y = 4 }
                };
            }
            else
            {
                return new Index[] { new Index { Depth = index.Depth, X = index.X + 1, Y = index.Y } };
            }
        }

        public static Index[] UpNeighbors(Index index)
        {
            if (index.Y == 0)
            {
                // Up one depth
                return new Index[] { new Index { Depth = index.Depth - 1, X = 2, Y = 1 } };
            }
            else if (index.X == 2 && index.Y == 3)
            {
                // In one depth
                return new Index[] {
                    new Index { Depth = index.Depth + 1, X = 0, Y = 4 },
                    new Index { Depth = index.Depth + 1, X = 1, Y = 4 },
                    new Index { Depth = index.Depth + 1, X = 2, Y = 4 },
                    new Index { Depth = index.Depth + 1, X = 3, Y = 4 },
                    new Index { Depth = index.Depth + 1, X = 4, Y = 4 }
                };
            }
            else
            {
                return new Index[] { new Index { Depth = index.Depth, X = index.X, Y = index.Y - 1 } };
            }
        }


        public static Index[] DownNeighbors(Index index)
        {
            if (index.Y == 4)
            {
                // Up one depth
                return new Index[] { new Index { Depth = index.Depth - 1, X = 2, Y = 3 } };
            }
            else if (index.X == 2 && index.Y == 1)
            {
                // In one depth
                return new Index[] {
                    new Index { Depth = index.Depth + 1, X = 0, Y = 0 },
                    new Index { Depth = index.Depth + 1, X = 1, Y = 0 },
                    new Index { Depth = index.Depth + 1, X = 2, Y = 0 },
                    new Index { Depth = index.Depth + 1, X = 3, Y = 0 },
                    new Index { Depth = index.Depth + 1, X = 4, Y = 0 }
                };
            }
            else
            {
                return new Index[] { new Index { Depth = index.Depth, X = index.X, Y = index.Y + 1 } };
            }
        }

        public static Index[] Neighbors(Index index)
        {
            var neighbors = new List<Index>();

            neighbors.AddRange(LeftNeighbors(index));
            neighbors.AddRange(UpNeighbors(index));
            neighbors.AddRange(RightNeighbors(index));
            neighbors.AddRange(DownNeighbors(index));

            return neighbors.ToArray();
        }

        Index[] IndicesWithBugs()
        {
            return bugs.ToArray();
        }

        public MultiEris Iterate()
        {
            var bugs = IndicesWithBugs();

            // do all the neighbors of all the bugs, and also the bugs.
            var toEvaluate = new List<MultiEris.Index>(bugs);
            foreach (var b in bugs)
            {
                toEvaluate.AddRange(Neighbors(b));
            }
            var unique = toEvaluate.Distinct();

            var next = new MultiEris();
            foreach (var u in unique)
            {
                var neighbors = Neighbors(u);
                var neighborBugCount = neighbors.Sum(n => GetContent(n) ? 1 : 0);
                bool hasBug = GetContent(u);
                bool isBug = hasBug ? (neighborBugCount == 1) : (neighborBugCount == 1 || neighborBugCount == 2);

                if (isBug)
                {
                    next.SetContent(u, true);
                }
            }

            return next;
        }
    }

    public bool ProveB()
    {
        var index1 = new MultiEris.Index { Depth = 0, X = 3, Y = 3 };
        var neighbors1 = MultiEris.Neighbors(index1);
        if (neighbors1.Length != 4)
        {
            return false;
        }
        var index2 = new MultiEris.Index { Depth = 1, X = 3, Y = 0 };
        var neighbors2 = MultiEris.Neighbors(index2);
        if (neighbors2.Length != 4)
        {
            return false;
        }
        var index3 = new MultiEris.Index { Depth = 1, X = 4, Y = 0 };
        var neighbors3 = MultiEris.Neighbors(index3);
        if (neighbors3.Length != 4)
        {
            return false;
        }
        var index4 = new MultiEris.Index { Depth = 1, X = 3, Y = 2 };
        var neighbors4 = MultiEris.Neighbors(index4);
        if (neighbors4.Length != 8)
        {
            return false;
        }

        var eris = new MultiEris();
        var lines = File.ReadAllLines(Example, Encoding.UTF8);
        for (var y = 0; y < 5; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                if (lines[y][x] == '#')
                {
                    var index = new MultiEris.Index { X = x, Y = y, Depth = 0 };
                    eris.SetContent(index, true);
                }
            }
        }

        if (eris.BugCount != 8)
        {
            return false;
        }

        // Iterate 10 times
        var next = eris;
        for (int i = 0; i < 10; i++)
        {
            next = next.Iterate();
        }
        if (next.BugCount != 99)
        {
            return false;
        }

        return true;
    }

    private string SolveB()
    {
        var eris = new MultiEris();
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        for (var y = 0; y < 5; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                if (lines[y][x] == '#')
                {
                    var index = new MultiEris.Index { X = x, Y = y, Depth = 0 };
                    eris.SetContent(index, true);
                }
            }
        }

        var next = eris;
        for (int i = 0; i < 200; i++)
        {
            next = next.Iterate();
        }
        return next.BugCount.ToString();
    }
}
