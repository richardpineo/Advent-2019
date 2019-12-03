using System;
using System.IO;
using System.Text;

class Solve1 : ISolve
{
    public string Description()
    {
        return "Day 1: The Tyranny of the Rocket Equation";
    }

    const string inputA = "Input//1a.txt";
    const string ExampleA = "Examples//1a.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        string[] lines = File.ReadAllLines(ExampleA, Encoding.UTF8);
        foreach (var line in lines)
        {
            var inOut = line.Split(" ");
            int mass = int.Parse(inOut[0]);
            int fuel = CalculateFuel(mass);
            if (int.Parse(inOut[1]) != fuel)
            {
                Console.WriteLine($"{mass} does not equal {fuel}");
                return false;
            }
        }

        return true;
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
        string[] lines = File.ReadAllLines(inputA, Encoding.UTF8);

        var sum = 0;
        foreach (var line in lines)
        {
            var fuel = CalculateFuel(int.Parse(line));
            sum += fuel;
        }
        return sum.ToString();
    }


    private int CalculateFuel(int mass)
    {
        var fuel = mass / 3 - 2;
        return fuel;
    }

    public string SolveB()
    {
        return "NotImpl";
    }
}
