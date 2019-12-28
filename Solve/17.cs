using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve17 : ISolve
{
    public string Description()
    {
        return "Day 17: Set and Forget";
    }

    const string Input = "Input//17.txt";
    const string Example = "Examples//17b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    enum Values
    {
        Space = '.',
        Scaffold = '#',
        FaceUp = '^',
        FaceDown = 'v',
        FaceLeft = '<',
        FaceRight = '>',
    }

    private bool ProveA()
    {
        string[] example = {
            "..#..........",
            "..#..........",
            "#######...###",
            "#.#...#...#.#",
            "#############",
            "..#...#...#..",
            "..#####...^.."
        };
        var answer = AlignmentValue(example);
        return answer == 76;
    }

    private int AlignmentValue(string[] input)
    {
        var rows = input[0].Length;
        var cols = input.Length;
        var environment = new Dictionary<Point, Values>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                environment[new Point(row, col)] = (Values)input[col][row];
            }
        }

        var intersections = FindIntersections(environment, rows, cols);
        var answer = intersections.Sum(p => p.X * p.Y);
        return answer;
    }

    private Point[] FindIntersections(Dictionary<Point, Values> environment, int numRows, int numCols)
    {
        var points = new List<Point>();
        // Walk througb the environment
        for (int row = 1; row < numRows; row++)
        {
            for (int col = 1; col < numCols; col++)
            {
                // Check the location and surrounding values 
                if (ValueAt(new Point(row, col), environment) != Values.Scaffold ||
                    ValueAt(new Point(row + 1, col), environment) != Values.Scaffold ||
                    ValueAt(new Point(row - 1, col), environment) != Values.Scaffold ||
                    ValueAt(new Point(row, col + 1), environment) != Values.Scaffold ||
                    ValueAt(new Point(row, col - 1), environment) != Values.Scaffold)
                {
                    continue;
                }

                // Found one.
                points.Add(new Point(row, col));
            }
        }
        return points.ToArray();
    }

    private Values ValueAt(Point point, Dictionary<Point, Values> environment)
    {
        return environment.GetValueOrDefault(point, Values.Space);
    }

    private bool ProveB()
    {
        return true;
    }

    private char[] ConvertToInstructions(string input)
    {
        return input.ToCharArray();
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    private char? OutputToValue(long value)
    {
        if (value < char.MaxValue && value > 0)
        {
            return (char)value;
        }
        return null;
    }

    private void OutputAscii(string ascii)
    {
        Console.Write(ascii);
    }

    public string SolveA()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);
        var ascii = new StringBuilder();
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                var v = OutputToValue(output.Value);
                if (v.HasValue)
                {
                    ascii.Append(v.Value);
                }
            }
        }
        var asciiText = ascii.ToString();
        // OutputAscii(asciiText);
        var environment = asciiText.Split(new[] { newLine });
        environment = environment.Where(e => e.Length > 0).ToArray();
        var answer = AlignmentValue(environment);
        return answer.ToString();
    }

    private char newLine = '\n';

    public string SolveB()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        program[0] = 2;
        var state = new Intcode.State(program);

        var main = ConvertToInstructions("A,B,B,C,C,A,B,B,C,A");
        var A = ConvertToInstructions("R,4,R,12,R,10,L,12");
        var B = ConvertToInstructions("L,12,R,4,R,12");
        var C = ConvertToInstructions("L,12,L,8,R,10");
        var total = new List<char>();
        total.AddRange(main);
        total.Add(newLine);
        total.AddRange(A);
        total.Add(newLine);
        total.AddRange(B);
        total.Add(newLine);
        total.AddRange(C);
        total.Add(newLine);
        total.Add('n');
        total.Add(newLine);

        int current = 0;

        var ascii = new StringBuilder();
        long lastOutput = 0;
        while (Intcode.Step(state))
        {
            if (!state.input.HasValue && current < total.Count)
            {
                state.input = total[current++];
            }
            var output = state.PopOutput();
            if (output.HasValue)
            {
                lastOutput = output.Value;
                var v = OutputToValue(output.Value);
                if (v.HasValue)
                {
                    ascii.Append(v.Value);
                }
            }
        }
        var asciiText = ascii.ToString();
        // OutputAscii(asciiText);
        return lastOutput.ToString();
    }
}
