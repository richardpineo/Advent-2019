using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve22 : ISolve
{
    public string Description()
    {
        return "Day 22: Slam Shuffle";
    }

    const string Input = "Input//22.txt";

    public bool Prove(bool isA)
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

    public bool Expect(string expected, int[] deck)
    {
        var got = string.Join(" ", deck);
        if (got != expected)
        {
            Console.WriteLine("\nExpected:\n" + expected + "\nBut got:\n" + got);
        }
        return got == expected;
    }

    public void DumpDeck(string desc, int[] deck)
    {
        Console.WriteLine(desc + ": " + string.Join(" ", deck));
    }

    public int[] InitDeck(int size)
    {
        List<int> deck = new List<int>();
        for (var i = 0; i < size; i++)
        {
            deck.Add(i);
        }
        return deck.ToArray();
    }

    public int[] DealIntoNewStack(int[] deck)
    {
        return deck.Reverse().ToArray();
    }

    public int[] CutN(int[] deck, int n)
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

    public int[] DealWithIncrementN(int[] deck, int N)
    {
        int[] newDeck = new int[deck.Length];
        for (int pos = 0; pos < deck.Length; pos++)
        {
            newDeck[(pos * N) % deck.Length] = deck[pos];
        }
        return newDeck;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        List<int> deck = new List<int>();
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        return "Sad death";
    }
    public string SolveB()
    {
        return "Sad death";
    }
}
