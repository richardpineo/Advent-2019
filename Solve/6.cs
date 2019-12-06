using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Spaceoids = System.Collections.Generic.Dictionary<string, string>;

class Solve6 : ISolve
{
    public string Description()
    {
        return "Day 6: Universal Orbit Map";
    }

    const string Input = "Input//6.txt";
    const string ExampleA = "Examples//6a.txt";
    const string ExampleB = "Examples//6b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveFor(evaluateA, ExampleA) : ProveFor(evaluateB, ExampleB);
    }

    private bool ProveFor(Func<Spaceoids, bool> evaluate, string filename)
    {
        return evaluate(ReadSpaceoids(filename));
    }

    private Spaceoids ReadSpaceoids(string filename)
    {
        string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
        var spaceoids = new Spaceoids();
        foreach (var line in lines)
        {

            var so = line.Split(")");
            // object to object it orbits
            spaceoids.Add(so[1], so[0]);
        }
        return spaceoids;

    }

    private bool evaluateA(Spaceoids spaceoids)
    {
        var answer = 42;
        var count = 0;
        foreach (var name in spaceoids.Keys)
        {
            count += orbitees(name, spaceoids);
        }
        return count == answer;
    }

    private bool orbiteeCount(Spaceoids spaceoids)
    {
        var answer = 42;
        var count = 0;
        foreach (var name in spaceoids.Keys)
        {
            count += orbitees(name, spaceoids);
        }
        return count == answer;
    }

    private int orbitees(string name, Spaceoids spaceoids)
    {
        if (!spaceoids.TryGetValue(name, out string orbitee))
            return 0;
        return 1 + orbitees(orbitee, spaceoids);
    }

    private void getPath(string name, Spaceoids s, ref List<string> path)
    {
        string orbitee;
        if (!s.TryGetValue(name, out orbitee))
        {
            return;
        }
        path.Add(name);
        getPath(orbitee, s, ref path);
    }

    private int countB(Spaceoids s)
    {
        var you = new List<string>();
        getPath(s["YOU"], s, ref you);
        var santa = new List<string>();
        getPath(s["SAN"], s, ref santa);

        for (var i = 0; i < you.Count; i++)
        {
            var santaIdx = santa.FindIndex(s => s == you[i]);
            if (-1 != santaIdx)
            {
                return i + santaIdx;
            }
        }
        return -1;
    }

    private bool evaluateB(Spaceoids s)
    {
        int count = countB(s);
        int answer = 4;
        return count == answer;
    }

    public string Solve(bool isA)
    {
        var spaceoids = ReadSpaceoids(Input);
        return isA ? SolveA(spaceoids) : SolveB(spaceoids);
    }

    private string SolveA(Spaceoids s)
    {
        var count = 0;
        foreach (var name in s.Keys)
        {
            count += orbitees(name, s);
        }
        return count.ToString();
    }

    private string SolveB(Spaceoids s)
    {
        int count = countB(s);
        return count.ToString();
    }
}
