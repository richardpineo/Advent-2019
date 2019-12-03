using System;
using System.IO;
using System.Text;

// Find/replace X for the number.

class SolveX : ISolve
{
    public string Description()
    {
        return "Day X: Bacon";
    }

    const string Input = "Input//X.txt";
    const string ExampleA = "Examples//Xa.txt";
    const string ExampleB = "Examples//Xb.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        return false;
    }

    public bool ProveB()
    {
        return false;
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
