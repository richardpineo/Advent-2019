using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class Solve22 : ISolve
{
    public string Description()
    {
        return "Day 22: Slam Shuffle";
    }

    const string Input = "Input//22.txt";

    public bool Prove(bool isA)
    {
        return isA ? (ProveA()) : true;
    }

    private long Apply(ShuffleCommand command, long pos, long length, bool invert)
    {
        if (invert)
        {
            return 0; // Not impl
        }
        else
        {
            return mod((command.a * pos + command.b), length);
        }
    }

    long modInverse(long a, long m)
    {
        a = a % m;
        for (long x = 1; x < m; x++)
            if ((a * x) % m == 1)
                return x;
        return 1;
    }

    // works with negatives.
    long mod(long x, long m)
    {
        long r = x % m;
        return r < 0 ? (r + m) : r;
    }

    bool ProveA()
    {
        var reversed = new ShuffleCommand[] {
            DealIntoNewStack()
        };

        var cut3 = new ShuffleCommand[] {
            CutN(3)
        };
        var cutNeg4 = new ShuffleCommand[] {
            CutN(-4)
        };

        var incN = new ShuffleCommand[] {
            DealWithIncrementN(3)
        };

        var ex1 = new ShuffleCommand[] {
            DealWithIncrementN(7),
            DealIntoNewStack(),
            DealIntoNewStack()
        };

        var ex2 = new ShuffleCommand[] {
            CutN(6),
            DealWithIncrementN(7),
            DealIntoNewStack()
        };
        var ex3 = new ShuffleCommand[] {
            DealWithIncrementN(7),
            DealWithIncrementN(9),
            CutN(-2)
        };

        var ex4 = new ShuffleCommand[] {
            DealIntoNewStack(),
            CutN(-2),
            DealWithIncrementN(7),
            CutN(8),
            CutN(-4),
            DealWithIncrementN(7),
            CutN(3),
            DealWithIncrementN(9),
            DealWithIncrementN(3),
            CutN(-1)
        };

        return
            Expect(6, reversed) &&
            Expect(0, cut3) &&
            Expect(7, cutNeg4) &&
            Expect(9, incN) &&
            Expect(1, ex1) &&
            Expect(0, ex2) &&
            Expect(1, ex3) &&
            Expect(8, ex4);
    }

    bool Expect(long expected, ShuffleCommand[] commands)
    {
        long length = 10;
        long initialPos = 3;
        long pos = initialPos;
        foreach (var c in commands)
        {
            pos = Apply(c, pos, length, false);
        }
        if (expected != pos)
        {
            Console.WriteLine("\nExpected:\n" + expected + "\nBut got:\n" + pos);
            return false;
        }

        // Apply backwards
        /*
        var backwards = commands.Reverse();
        foreach (var c in backwards)
        {
            pos = Apply(c, pos, length, true);
        }
        if (initialPos != pos)
        {
            Console.WriteLine("Reversed pos:\n" + initialPos + "\nBut got:\n" + pos);
            return false;
        }
        */
        return true;
    }


    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    string SolveA()
    {
        long length = 10007;
        var commands = ParseInput(length);
        long pos = 2019;
        foreach (var command in commands)
        {
            pos = Apply(command, pos, length, false);
        }
        return pos.ToString();
    }

    struct ShuffleCommand
    {
        // ax + b % m
        // https://codeforces.com/blog/entry/72593
        public long a;
        public long b;
    }

    ShuffleCommand DealIntoNewStack()
    {
        return new ShuffleCommand { a = -1, b = -1 };
    }

    ShuffleCommand CutN(long n)
    {
        return new ShuffleCommand { a = 1, b = -n };
    }

    ShuffleCommand DealWithIncrementN(long n)
    {
        return new ShuffleCommand { a = n, b = 0 };
    }

    ShuffleCommand[] ParseInput(long length)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var commands = new List<ShuffleCommand>();
        foreach (var line in lines)
        {
            if (line == "deal into new stack")
            {
                commands.Add(DealIntoNewStack());
            }
            else if (line.Contains("cut"))
            {
                var valString = line.TakeLast(line.Length - "cut ".Length);
                int val = int.Parse(string.Join("", valString));
                commands.Add(CutN(val));
            }
            else if (line.Contains("deal with increment"))
            {
                var valString = line.TakeLast(line.Length - "deal with increment ".Length);
                long val = int.Parse(string.Join("", valString));
                commands.Add(DealWithIncrementN(val));
            }
            else
            {
                throw new Exception("Unknown line: " + line);
            }
        }
        return commands.ToArray();
    }

    public string SolveB()
    {
        return "nope";
        /*
        long length = 119315717514047;
        long repeat = 101741582076661;
        var originalPos = 2020;
        long pos = originalPos;
        long cycle = 0;
        var commands = ParseInput(length).Reverse();
        for (long r = 0; r < repeat; r++)
        {
            foreach (var command in commands)
            {
                pos = Apply(command, pos, length, true);
            }
            cycle++;
            if (pos == originalPos)
            {
                // found the cycle.
                repeat = repeat % cycle;
            }
        }
        return pos.ToString();
        */
    }
}
