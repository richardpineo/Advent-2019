using System;
using System.IO;
using System.Text;
using System.Linq;

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
        return isA ? ProveFor(evaluateA, ExampleA) : ProveFor(evaluateB, ExampleB);
    }

    private bool ProveFor(Func<string, bool> evaluate, string filename)
    {
        string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
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

    private bool evaluateA(string s)
    {
        return hasAdjacencies(s) && strictlyIncreasing(s);
    }

    private bool evaluateB(string s)
    {
        return hasAtLeastOneDouble(s) && strictlyIncreasing(s);
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

    private bool hasAtLeastOneDouble(string s)
    {
        var counts = s.GroupBy(c => c)
                      .Select(g => g.Count())
                      .Where(c => c == 2);

        return counts.Count() > 0;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveFor(evaluateA) : SolveFor(evaluateB);
    }

    private string SolveFor(Func<string, bool> evaluate)
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
