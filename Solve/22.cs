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
        return isA ? (ProveA() && ProveAFast()) : true;
    }

    enum Shuffle
    {
        DealIntoNewStack,
        CutN,
        DealWithIncrementN,
    };

    public bool ProveA()
    {
        var deck = InitDeck(10);
        var reversed = DealIntoNewStack(deck);
        var cut3 = CutN(deck, 3);
        var cutNeg4 = CutN(deck, -4);
        var incN = DealWithIncrementN(deck, 3);

        var ex1 = DealIntoNewStack(DealIntoNewStack(DealWithIncrementN(deck, 7)));

        var ex2 = DealIntoNewStack(DealWithIncrementN(CutN(deck, 6), 7));

        var ex3 = CutN(DealWithIncrementN(DealWithIncrementN(deck, 7), 9), -2);

        /*
            deal into new stack
            cut -2
            deal with increment 7
            cut 8
            cut -4
            deal with increment 7
            cut 3
            deal with increment 9
            deal with increment 3
            cut -1
        */
        var ex4 =
            CutN(
                DealWithIncrementN(
                    DealWithIncrementN(
                            CutN(
                                DealWithIncrementN(
                                    CutN(
                                        CutN(
                                            DealWithIncrementN(
                                                CutN(
                                                    DealIntoNewStack(deck),
                                                -2),
                                            7),
                                        8),
                                    -4),
                                7),
                            3),
                        9),
                    3),
                -1);

        return
            Expect("0 1 2 3 4 5 6 7 8 9", deck) &&
            Expect("9 8 7 6 5 4 3 2 1 0", reversed) &&
            Expect("3 4 5 6 7 8 9 0 1 2", cut3) &&
            Expect("6 7 8 9 0 1 2 3 4 5", cutNeg4) &&
            Expect("0 7 4 1 8 5 2 9 6 3", incN) &&
            Expect("0 3 6 9 2 5 8 1 4 7", ex1) &&
            Expect("3 0 7 4 1 8 5 2 9 6", ex2) &&
            Expect("6 3 0 7 4 1 8 5 2 9", ex3) &&
            Expect("9 2 5 8 1 4 7 0 3 6", ex4);
    }

    private long Apply(ShuffleCommand command, long pos, long length, bool invert)
    {
        switch (command.shuffle)
        {
            case Shuffle.CutN:
                return invert ? InvertCutN(pos, command.arg, length) : CutN(pos, command.arg, length);
            case Shuffle.DealIntoNewStack:
                return invert ? InvertDealIntoNewStack(pos, length) : DealIntoNewStack(pos, length);
            case Shuffle.DealWithIncrementN:
                return invert ? InvertDealWithIncrementN(pos, command.arg, length) : DealWithIncrementN(pos, command.arg, length);
        }
        throw new Exception("Invalid command " + command);
    }

    public bool ProveAFast()
    {
        var length = 10;
        var pos = 3;

        var reversed = DealIntoNewStack(pos, length);
        var invertReversed = InvertDealIntoNewStack(reversed, length);

        var cut3 = CutN(pos, 3, length);
        var invertCut3 = InvertCutN(cut3, 3, length);

        var cutNeg4 = CutN(pos, -4, length);
        var invertCutNeg4 = InvertCutN(cutNeg4, -4, length);

        var incN = DealWithIncrementN(pos, 3, length);
        var invertIncN = InvertDealWithIncrementN(incN, 3, length);

        var ex1 = DealIntoNewStack(DealIntoNewStack(DealWithIncrementN(pos, 7, length), length), length);

        var ex2 = DealIntoNewStack(DealWithIncrementN(CutN(pos, 6, length), 7, length), length);

        var ex3 = CutN(DealWithIncrementN(DealWithIncrementN(pos, 7, length), 9, length), -2, length);

        /*
            deal into new stack
            cut -2
            deal with increment 7
            cut 8
            cut -4
            deal with increment 7
            cut 3
            deal with increment 9
            deal with increment 3
            cut -1
        */
        var ex4 =
            CutN(
                DealWithIncrementN(
                    DealWithIncrementN(
                            CutN(
                                DealWithIncrementN(
                                    CutN(
                                        CutN(
                                            DealWithIncrementN(
                                                CutN(
                                                    DealIntoNewStack(pos, length),
                                                -2, length),
                                            7, length),
                                        8, length),
                                    -4, length),
                                7, length),
                            3, length),
                        9, length),
                    3, length),
                -1, length);

        return
            Expect(3, pos) &&
            Expect(6, reversed) &&
            Expect(0, cut3) &&
            Expect(7, cutNeg4) &&
            Expect(9, incN) &&
            Expect(1, ex1) &&
            Expect(0, ex2) &&
            Expect(1, ex3) &&
            Expect(8, ex4) &&
            Expect(pos, invertReversed) &&
            Expect(pos, invertCut3) &&
            Expect(pos, invertCutNeg4) &&
            Expect(pos, invertIncN);
    }

    public bool Expect(string expected, long[] deck)
    {
        var got = string.Join(" ", deck);
        if (got != expected)
        {
            Console.WriteLine("\nExpected:\n" + expected + "\nBut got:\n" + got);
        }
        return got == expected;
    }
    public bool Expect(long got, long expected)
    {
        if (got != expected)
        {
            Console.WriteLine("\nExpected:\n" + expected + "\nBut got:\n" + got);
        }
        return got == expected;
    }

    public void DumpDeck(string desc, long[] deck)
    {
        Console.WriteLine(desc + ": " + string.Join(" ", deck));
    }

    public long[] InitDeck(long size)
    {
        List<long> deck = new List<long>();
        for (long i = 0; i < size; i++)
        {
            deck.Add(i);
        }
        return deck.ToArray();
    }

    public long[] DealIntoNewStack(long[] deck)
    {
        return deck.Reverse().ToArray();
    }

    public long DealIntoNewStack(long pos, long length)
    {
        return length - pos - 1;
    }

    public long InvertDealIntoNewStack(long pos, long length)
    {
        // Same as the first verse
        return length - pos - 1;
    }

    public long[] CutN(long[] deck, int n)
    {
        if (n > 0)
        {
            return deck.Skip(n).Concat(deck.Take(n)).ToArray();
        }
        else
        {
            return deck.TakeLast(-n).Concat(deck.Take(deck.Length + n)).ToArray();
        }
    }

    public long CutN(long pos, long n, long length)
    {
        return (pos - n + length) % length;
    }

    public long InvertCutN(long pos, long n, long length)
    {
        return (pos + n + length) % length;
    }

    public long[] DealWithIncrementN(long[] deck, long n)
    {
        long[] newDeck = new long[deck.Length];
        for (long pos = 0; pos < deck.Length; pos++)
        {
            newDeck[(pos * n) % deck.Length] = deck[pos];
        }
        return newDeck;
    }
    public long DealWithIncrementN(long pos, long n, long length)
    {
        return (pos * n) % length;
    }

    public long InvertDealWithIncrementN(long pos, long n, long length)
    {
        return (n * length / pos) % length;
    }

    public string Solve(bool isA)
    {
        if (isA)
        {
            var s1 = SolveA();
            var s2 = SolveAFast();
            if (s1 != s2)
            {
                return "Failure";
            }
            return s1;
        }
        return SolveB();
    }

    public string SolveA()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var deck = InitDeck(10007);
        foreach (var line in lines)
        {
            if (line == "deal into new stack")
            {
                deck = DealIntoNewStack(deck);
            }
            else if (line.Contains("cut"))
            {
                var valString = line.TakeLast(line.Length - "cut ".Length);
                int val = int.Parse(string.Join("", valString));
                deck = CutN(deck, val);
            }
            else if (line.Contains("deal with increment"))
            {
                var valString = line.TakeLast(line.Length - "deal with increment ".Length);
                long val = int.Parse(string.Join("", valString));
                deck = DealWithIncrementN(deck, val);
            }
            else
            {
                throw new Exception("Unknown line: " + line);
            }
        }
        long index = Array.FindIndex(deck, 0, c => c == 2019);
        return index.ToString();
    }
    public string SolveAFast()
    {
        var commands = ParseInput();
        long pos = 2019;
        long length = 10007;
        foreach (var command in commands)
        {
            pos = Apply(command, pos, length, false);
        }
        return pos.ToString();
    }

    struct ShuffleCommand
    {
        public Shuffle shuffle;
        public long arg;
    }

    ShuffleCommand[] ParseInput()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var commands = new List<ShuffleCommand>();
        foreach (var line in lines)
        {
            if (line == "deal into new stack")
            {
                commands.Add(new ShuffleCommand { shuffle = Shuffle.DealIntoNewStack });
            }
            else if (line.Contains("cut"))
            {
                var valString = line.TakeLast(line.Length - "cut ".Length);
                int val = int.Parse(string.Join("", valString));
                commands.Add(new ShuffleCommand { shuffle = Shuffle.CutN, arg = val });
            }
            else if (line.Contains("deal with increment"))
            {
                var valString = line.TakeLast(line.Length - "deal with increment ".Length);
                long val = int.Parse(string.Join("", valString));
                commands.Add(new ShuffleCommand { shuffle = Shuffle.DealWithIncrementN, arg = val });
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
        long length = 119315717514047;
        long repeat = 101741582076661;
        var originalPos = 2020;
        long pos = originalPos;
        long cycle = 0;
        var commands = ParseInput().Reverse();
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
    }
}
