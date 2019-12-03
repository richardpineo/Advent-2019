using System;
using System.IO;
using System.Text;

class Solve1 : ISolve
{
    public string Description()
    {
        return "Day 1: The Tyranny of the Rocket Equation";
    }

    const string Input = "Input//1.txt";
    const string ExampleA = "Examples//1a.txt";
    const string ExampleB = "Examples//1b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        return Prove(CalculateFuelSimple, ExampleA);
    }

    public bool ProveB()
    {
        return Prove(CalculateFuelComplex, ExampleB);
    }

    private bool Prove(Func<int, int> calcFn, string file)
    {
        string[] lines = File.ReadAllLines(file, Encoding.UTF8);
        foreach (var line in lines)
        {
            var inOut = line.Split(" ");
            int mass = int.Parse(inOut[0]);
            int fuel = calcFn(mass);
            int expected = int.Parse(inOut[1]);
            if (expected != fuel)
            {
                Console.WriteLine($"{mass} expected {expected} which does not equal {fuel}");
                return false;
            }
        }

        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return Solve(CalculateFuelSimple);
    }

    public string SolveB()
    {
        return Solve(CalculateFuelComplex);
    }

    private string Solve(Func<int, int> calcFn)
    {
        string[] lines = File.ReadAllLines(Input, Encoding.UTF8);

        var sum = 0;
        foreach (var line in lines)
        {
            var fuel = calcFn(int.Parse(line));
            sum += fuel;
        }
        return sum.ToString();
    }

    private int CalculateFuelSimple(int mass)
    {
        var fuel = mass / 3 - 2;
        return fuel;
    }

    private int CalculateFuelComplex(int mass)
    {
        int fuel = CalculateFuelSimple(mass);
        int fuelFuel = CalculateFuelSimple(fuel);
        if (fuelFuel > 0)
        {
            fuel += CalculateFuelComplex(fuel);
        }
        return fuel;
    }
}
