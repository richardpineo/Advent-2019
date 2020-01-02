using System;
using System.Diagnostics;
using System.Collections.Generic;

// https://adventofcode.com/
namespace AOC2019
{
    class Program
    {
        static void Main(string[] args)
        {
            ISolve[] all = {
                new Solve1(),
                new Solve2(),
                new Solve3(),
                new Solve4(),
                new Solve5(),
                new Solve6(),
                new Solve7(),
                new Solve8(),
                new Solve9(),
                new Solve10(),
                new Solve11(),
                new Solve12(),
                new Solve13(),
                new Solve14(),
                new Solve15(),
                new Solve16(),
                new Solve17(),
                // new Solve18(),
                new Solve19(),
                new Solve21(),
                new Solve22(),
                new Solve24()
            };

            ISolve[] toSolve;
            if (args.Length > 0 && args[0] == "last")
            {
                toSolve = new ISolve[] { all[all.Length - 1] };
            }
            else
            {
                toSolve = all;
            }

            var failed = new List<ISolve>();
            var timeForAll = new Stopwatch();
            timeForAll.Start();
            var timeForEach = new Dictionary<ISolve, long>();
            foreach (var solve in toSolve)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Console.WriteLine($"Solving: {solve.Description()}");
                if (SolveOne(solve, true) && SolveOne(solve, false))
                {
                    Console.WriteLine("Success!");
                }
                else
                {
                    failed.Add(solve);
                }
                var elapsed = stopwatch.ElapsedMilliseconds;
                timeForEach[solve] = elapsed;
                Console.WriteLine();
            }

            var padding = 8;
            foreach (var t in timeForEach)
            {
                var timing = t.Value.ToString().PadLeft(padding) + " ms";
                Console.WriteLine($"{timing}   {t.Key.Description()}");
            }
            Console.WriteLine($"{"-----------".PadLeft(padding)}   ------");
            var allTiming = timeForAll.ElapsedMilliseconds.ToString().PadLeft(padding) + " ms";
            Console.WriteLine($"{allTiming}   Total");
            Console.WriteLine();

            if (failed.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine(" FAILURES DETECTED:");
                failed.ForEach(f => Console.WriteLine($"   {f.Description()}"));
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
