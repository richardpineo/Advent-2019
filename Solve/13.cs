using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IntType = System.Int64;

class Solve13 : ISolve
{
    public string Description()
    {
        return "Day 13: Care Package";
    }

    const string Input = "Input//13.txt";

    enum Tile
    {
        Empty = 0,
        Wall = 1,
        Block = 2,
        Paddle = 3,
        Ball = 4
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
        var outputs = new List<IntType>();
        while (Intcode.Step(state))
        {
            if (state.output.HasValue)
            {
                var output = state.PopOutput();
                if (output.HasValue)
                {
                    outputs.Add(output.Value);
                }
            }
        }

        return countBlocks(outputs).ToString();
    }

    private int countBlocks(List<IntType> outputs)
    {
        int count = 0;
        for (int i = 2; i < outputs.Count; i += 3)
        {
            if (outputs[i] == (IntType)Tile.Block)
            {
                count++;
            }
        }
        return count;
    }

    public string SolveB()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        program[0] = 2;

        var state = new Intcode.State(program);
        var outputs = new List<IntType>();
        IntType score = 0;
        IntType paddlePos = 0;
        IntType ballPos = 0;
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                outputs.Add(output.Value);
                if (outputs.Count == 3)
                {
                    var x = outputs[0];
                    var y = outputs[1];
                    var other = outputs[2];
                    if (x == -1 && y == 0)
                    {
                        score = other;
                    }
                    else if (other == (IntType)Tile.Paddle)
                    {
                        paddlePos = x;
                    }
                    else if (other == (IntType)Tile.Ball)
                    {
                        ballPos = x;
                    }
                    // -1, 0, 1 for movement based on ball, paddle position.
                    state.input = ballPos.CompareTo(paddlePos);
                    outputs.Clear();
                }
            }
        }
        return score.ToString();
    }
}
