using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;
using Point = System.Drawing.Point;

class Solve18 : ISolve
{
    public string Description()
    {
        return "Day 18: Many-Worlds Interpretation";
    }

    const string Input = "Input//18.txt";

    enum Movement
    {
        North = 1,
        South = 2,
        West = 3,
        East = 4,
    };

    // const char Wall = '#';
    // const char Open = '.';

    public bool Prove(bool isA)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        return true;
    }

    class State
    {
        HashSet<char> keys = new HashSet<char>();
    }

    private int Solve()
    {
        return 1;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return "NOPE";
    }
    public string SolveB()
    {
        return "NOPE";
    }
    Point move(Movement movement, Point loc)
    {
        switch (movement)
        {
            case Movement.North:
                return new Point(loc.X, loc.Y + 1);
            case Movement.South:
                return new Point(loc.X, loc.Y - 1);
            case Movement.West:
                return new Point(loc.X - 1, loc.Y);
            case Movement.East:
                return new Point(loc.X + 1, loc.Y);
        }
        throw new Exception("Invalid movement");
    }
}
