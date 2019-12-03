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
                if (SolveOne(solve, true) && SolveOne(solve, false))
                {
                    Console.WriteLine("Success!");
                }

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
