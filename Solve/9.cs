using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Row = System.Collections.Generic.List<int>;
using Layer = System.Collections.Generic.List<System.Collections.Generic.List<int>>;

class Solve9 : ISolve
{
    public string Description()
    {
        return "Day 9: Sensor Boost";
    }

    const string Input = "Input//9.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        return ProveA1() && ProveA2() && ProveA3();
    }

    private bool ProveA1()
    {
        return true;
    }
    private bool ProveA2()
    {
        return true;
    }
    private bool ProveA3()
    {
        return true;
    }

    public bool ProveB()
    {
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return "NOTIMPL";
    }

    public string SolveB()
    {
        return "NOTIMPL";
    }
}
