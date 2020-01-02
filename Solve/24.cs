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
        var eris = new Eris();
        var lines = File.ReadAllLines(Example, Encoding.UTF8);
        for (var y = 0; y < 5; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                eris.SetBug(x, y, lines[y][x] == '#');
            }
        }
        return SolveFor(eris, false) == 2129920;
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

        static long PowerOf2(long power)
        {
            long result = 1;
            for (long i = 0; i < power; i++)
                result *= 2;
            return result;
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
        return SolveFor(eris, false).ToString();
    }

    private long SolveFor(Eris eris, bool trace)
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

    private string SolveB()
    {
        return "Nope";
    }
}
