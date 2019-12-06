using System;
using System.IO;
using System.Text;

class Solve4 : ISolve
{
    public string Description()
    {
        return "Day 4: Secure Container";
    }

    const string Input = "Input//4.txt";
    const string ExampleA = "Examples//4a.txt";
    const string ExampleB = "Examples//4b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : false;
    }

    private bool ProveA()
    {
        string[] lines = File.ReadAllLines(ExampleA, Encoding.UTF8);
        foreach (var line in lines)
        {
            var inOut = line.Split(" ");

            if (inOut[1] != (evaluate(inOut[0]) ? "1" : "0"))
            {
                return false;
            }
        }

        return true;
    }

    private bool evaluate(string s)
    {
        return hasAdjacencies(s) && strictlyIncreasing(s);
    }

    private bool strictlyIncreasing(string s)
    {
        int last = 0;
        for (int i = 0; i < s.Length; i++)
        {
            string v = s.Substring(i, 1);
            var str = v;
            int val = int.Parse(str);
            if (val < last)
            {
                return false;
            }
            last = val;
        }
        return true;
    }

    private bool hasAdjacencies(string s)
    {
        // find adjacencies
        for (int i = 1; i < s.Length; i++)
        {
            if (s[i] == s[i - 1])
            {
                return true;
            }
        }
        return false;
    }
    public string Solve(bool isA)
    {
        return isA ? SolveA() : "NotImpl";
    }

    private string SolveA()
    {
        string[] lines = File.ReadAllLines(Input, Encoding.UTF8);
        var min = int.Parse(lines[0]);
        var max = int.Parse(lines[1]);
        var count = 0;
        for (int i = min; i <= max; i++)
        {
            if (evaluate(i.ToString()))
            {
                count++;
            }
        }
        return count.ToString();
    }
}
