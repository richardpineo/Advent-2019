using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;
using Point = System.Drawing.Point;

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

    void log(string s, int depth)
    {
        // Console.WriteLine("".PadLeft(depth) + s);
    }

    void MapIt(Intcode.State initialState, Point currentLoc, ref HashSet<Point> cells, ref Point oxygenCell, int depth)
    {
        foreach (Movement m in Enum.GetValues(typeof(Movement)))
        {
            log($"{currentLoc.ToString()}: moving {m}", depth);

            var tempState = Intcode.State.Clone(initialState);
            tempState.input = (IntType)m;
            var newLoc = move(m, currentLoc);
            if (cells.Contains(newLoc))
            {
                // we've been here before
                continue;
            }

            bool processedCurrent = false;
            while (!processedCurrent && Intcode.Step(tempState))
            {
                var output = tempState.PopOutput();
                if (output.HasValue)
                {
                    switch ((Response)output.Value)
                    {
                        case Response.Moved:
                            //  log($"Moved to {newLoc.ToString()}", depth);
                            break;
                        case Response.Wall:
                            log($"{newLoc.ToString()}: Wall", depth);
                            processedCurrent = true;
                            break;
                        case Response.MovedAndFound:
                            log($"{newLoc.ToString()}: found big O", depth);
                            oxygenCell = newLoc;
                            break;
                    }
                    if (!processedCurrent)
                    {
                        log($"{newLoc.ToString()}: Spreading it out here, boss", depth);
                        cells.Add(newLoc);
                        MapIt(tempState, newLoc, ref cells, ref oxygenCell, depth + 1);
                        processedCurrent = true;
                    }
                }
            }
        }
    }
    public string SolveB()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);
        var currentLoc = new Point(0, 0);
        var cells = new HashSet<Point>();
        cells.Add(currentLoc);
        var oxygenCell = new Point(-1000, -1000);
        MapIt(state, currentLoc, ref cells, ref oxygenCell, 0);

        // Now we have it mapped, spread out the oxy.
        var breathable = new HashSet<Point>();
        breathable.Add(oxygenCell);
        int minutes = 0;
        while (breathable.Count != cells.Count)
        {
            var newBreathable = new HashSet<Point>(breathable);
            foreach (var point in breathable)
            {
                foreach (Movement m in Enum.GetValues(typeof(Movement)))
                {
                    var spreadTo = move(m, point);
                    if (cells.Contains(spreadTo))
                    {
                        newBreathable.Add(spreadTo);
                    }
                }
            }
            breathable = newBreathable;

            minutes++;
        }

        return minutes.ToString();
    }
}
