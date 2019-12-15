using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;

class Solve15 : ISolve
{
    public string Description()
    {
        return "Day 15: Oxygen System";
    }

    const string Input = "Input//15.txt";

    enum Movement
    {
        North = 1,
        South = 2,
        West = 3,
        East = 4,
    };

    enum Response
    {
        Wall = 0,
        Moved = 1,
        MovedAndFound = 2,
    };

    public bool Prove(bool isA)
    {
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);
        var found = FindIt(state, new List<Movement>());
        return found.Count.ToString();
    }

    bool backwards(Movement movement, Movement last)
    {
        switch (movement)
        {
            case Movement.North:
                return last == Movement.South;
            case Movement.South:
                return last == Movement.North;
            case Movement.West:
                return last == Movement.East;
            case Movement.East:
                return last == Movement.West;
        }
        throw new Exception("Invalid movement");
    }

    List<Movement> FindIt(Intcode.State initialState, List<Movement> current)
    {
        foreach (Movement m in Enum.GetValues(typeof(Movement)))
        {
            if (current.Count > 0 && backwards(m, current[current.Count - 1]))
            {
                // Don't go backwards
                continue;
            }

            var tempState = Intcode.State.Clone(initialState);
            tempState.input = (IntType)m;
            var tempMovement = new List<Movement>(current);
            tempMovement.Add(m);

            var hitWall = false;
            while (!hitWall && Intcode.Step(tempState))
            {
                var output = tempState.PopOutput();
                if (output.HasValue)
                {
                    switch ((Response)output.Value)
                    {
                        case Response.Moved:
                            // recurse
                            var found = FindIt(tempState, tempMovement);
                            if (found != null)
                            {
                                return found;
                            }
                            hitWall = true;
                            break;
                        case Response.Wall:
                            hitWall = true;
                            break;
                        case Response.MovedAndFound:
                            return tempMovement;
                    }
                }
            }
        }
        return null;
    }

    public string SolveB()
    {
        return "Not Impl";
    }
}
