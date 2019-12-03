using System;

// https://adventofcode.com/
namespace AOC2019
{
    class Program
    {
        static void Main(string[] args)
        {
            ISolve[] toSolve = {
                new Solve1()
            };

            foreach (var solve in toSolve)
            {
                Console.WriteLine($"Solving: {solve.Description()}");

                Console.Write($"  Prove A: ");
                if( !solve.ProveA() ) 
                {
                    Console.WriteLine("Failure!");
                }
                else
                {
                    Console.WriteLine("Success!");
                    var a = solve.SolveA();
                    Console.WriteLine($"  Solve A: {a}");
                }

            }
        }
    }
}
