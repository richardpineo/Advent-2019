using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;
using Point = System.Drawing.Point;
using Hull = System.Collections.Generic.Dictionary<System.Drawing.Point, HullSquare>;

public class HullSquare
{
    public Color color = Color.Black;
}

public static class HullExtensions
{
    public static void Paint(this Hull hull, Point p, Color c)
    {
        hull[p] = new HullSquare { color = c };
    }

    public static Color ColorAt(this Hull hull, Point p)
    {
        var square = hull.GetValueOrDefault(p);
        return square != null ? square.color : Color.Black;
    }

    public static int Painted(this Hull hull)
    {
        return hull.Keys.Count;
    }

    public static void Render(this Hull hull)
    {
        // First, find the bounds.
        var topLeft = new Point(0, 0);
        var bottomRight = new Point(0, 0);
        foreach (var key in hull.Keys)
        {
            if (key.X < topLeft.X)
            {
                topLeft.X = key.X;
            }
            else if (key.X > bottomRight.X)
            {
                bottomRight.X = key.X;
            }

            if (key.Y < topLeft.Y)
            {
                topLeft.Y = key.Y;
            }
            else if (key.Y > bottomRight.Y)
            {
                bottomRight.Y = key.Y;
            }
        }

        // Render the square
        for (var y = topLeft.Y; y <= bottomRight.Y; y++)
        {
            for (var x = topLeft.X; x <= bottomRight.X; x++)
            {
                if (Color.White == hull.ColorAt(new Point(x, y)))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }
    }
}

public enum Direction
{
    up = 0,
    right = 1,
    down = 2,
    left = 3
}

public class Robot
{
    public Direction facing = Direction.up;
    public Point location = new Point(0, 0);

    public void TurnLeft()
    {
        switch (facing)
        {
            case Direction.up:
                facing = Direction.left;
                break;
            case Direction.left:
                facing = Direction.down;
                break;
            case Direction.down:
                facing = Direction.right;
                break;
            case Direction.right:
                facing = Direction.up;
                break;
        }
    }
    public void TurnRight()
    {
        // Lazy
        TurnLeft();
        TurnLeft();
        TurnLeft();
    }

    public void MoveForward()
    {
        switch (facing)
        {
            case Direction.up:
                location.Y--;
                break;
            case Direction.left:
                location.X--;
                break;
            case Direction.down:
                location.Y++;
                break;
            case Direction.right:
                location.X++;
                break;
        }
    }
}

public enum Color
{
    Black = 0,
    White = 1
}

class Solve11 : ISolve
{
    public string Description()
    {
        return "Day 11: Space Police";
    }

    const string Input = "Input//11.txt";

    public bool Prove(bool isA)
    {
        // No proof
        return true;
    }

    public string Solve(bool isA)
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var startingColor = isA ? Color.Black : Color.White;
        var state = new Intcode.State(program, (long)startingColor);
        var hull = new Hull();
        var robot = new Robot();

        var outputs = new List<IntType>();
        // 0 = paint
        // 1 = move
        var mode = 0;
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                if (mode == 0)
                {
                    // Paint
                    hull.Paint(robot.location, (Color)output.Value);
                }
                else
                {
                    // Move
                    if (output.Value == 0)
                    {
                        robot.TurnLeft();
                    }
                    else
                    {
                        robot.TurnRight();
                    }
                    robot.MoveForward();
                    state.input = (long)hull.ColorAt(robot.location);
                }
                mode = (mode + 1) % 2;
            }
        }

        if (isA)
        {
            // Count up the doubled squares.
            return hull.Painted().ToString();
        }
        else
        {
            // Render it
            hull.Render();
            return "Manual";
        }
    }
}
