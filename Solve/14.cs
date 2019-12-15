using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;


class Factory
{
    public Factory(string[] lines)
    {
    }

    public int Solve()
    {
        return 0;
    }
}

class Solve14 : ISolve
{
    public string Description()
    {
        return "Day 14: Space Stoichiometry";
    }

    const string Input = "Input//14.txt";

    string[] Examples = new string[] {
        "Examples//14a-1.txt",
        "Examples//14a-2.txt",
        "Examples//14a-3.txt",
        "Examples//14a-4.txt",
        "Examples//14a-5.txt",
    };

    public bool Prove(bool isA)
    {
        foreach (string example in Examples)
        {
            var lines = File.ReadAllLines(example, Encoding.UTF8);
            var answer = int.Parse(lines[0]);
            var input = lines.Skip(1);
            var factory = new Factory(input.ToArray());
            var possible = factory.Solve();
            if (possible != answer)
            {
                Console.WriteLine($"Example {example} failed: got {possible}, expected {answer}");
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
        return "NotImpl";
    }
    public string SolveB()
    {
        return "NotImpl";
    }
}
