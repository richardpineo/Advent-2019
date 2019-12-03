using System;
using System.Diagnostics;

// https://adventofcode.com/
namespace AOC2019
{
    class Program
    {
        static void Main(string[] args)
        {
            ISolve[] toSolve = {
                new Solve1(),
                new Solve2(),
                new Solve3()
            };

            foreach (var solve in toSolve)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Console.WriteLine($"Solving: {solve.Description()}");
                if (SolveOne(solve, true) && SolveOne(solve, false))
                {
                    Console.WriteLine("Success!");
                }
                Console.WriteLine($"    {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine();
            }
        }

        static bool SolveOne(ISolve solve, bool isA)
        {
            var desc = isA ? "A" : "B";
            Console.Write($"  Prove {desc}: ");
            if (!solve.Prove(isA))
            {
                Console.WriteLine("Embarrassing Failure!");
                return false;
            }
            else
            {
                Console.WriteLine("Tremendous Success");
                var solution = solve.Solve(isA);
                Console.WriteLine($"  Solve {desc}: {solution}");
                return true;
            }

        }
    }
}
