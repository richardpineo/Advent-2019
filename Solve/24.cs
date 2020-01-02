using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Solve24 : ISolve
{
    public string Description()
    {
        return "Day 24: Planet of Discord";
    }

    const string Input = "Input//24.txt";

    public bool Prove(bool isA)
    {
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    class Eris
    {
        public bool GetBug(int x, int y)
        {
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
                hash += bugs[i] ? PowerOf2(i) : 0;
            }
            return hash;
        }
        public static long PowerOf2(long power)
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

        return "Not yet";
    }

    private string SolveB()
    {
        return "Nope";
    }
}
