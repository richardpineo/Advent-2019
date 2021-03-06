using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class Solve16 : ISolve
{
    public string Description()
    {
        return "Day 16: Flawed Frequency Transmission";
    }

    const string Input = "Input//16.txt";

    class Test
    {
        public string Input;
        public string Answer;
        public int NumberOfPhases;
    }

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        var tests = new Test[] {
                new Test() { Input = "12345678", Answer = "12345678", NumberOfPhases = 0 },
                new Test() { Input = "12345678", Answer = "48226158", NumberOfPhases = 1 },
                new Test() { Input = "12345678", Answer = "34040438", NumberOfPhases = 2 },
                new Test() { Input = "12345678", Answer = "03415518", NumberOfPhases = 3 },
                new Test() { Input = "12345678", Answer = "01029498", NumberOfPhases = 4 },
                new Test() { Input = "80871224585914546619083218645595", Answer = "24176176", NumberOfPhases = 100 },
                new Test() { Input = "19617804207202209144916044189917", Answer = "73745418", NumberOfPhases = 100 },
                new Test() { Input = "69317163492948606335995924319873", Answer = "52432133", NumberOfPhases = 100 }
            };

        foreach (var test in tests)
        {
            var answer = ApplyFinal(test.Input.ToCharArray(), test.NumberOfPhases).Substring(0, 8);
            if (test.Answer != answer)
            {
                Console.WriteLine($"Expected {test.Answer}, got {answer}");
                return false;
            }
        }
        return true;
    }

    private bool ProveB()
    {
        var tests = new Test[] {
            new Test() { Input = "03036732577212944063491565474664", Answer = "84462026", NumberOfPhases = 100 },
            new Test() { Input = "02935109699940807407585447034323", Answer = "78725270", NumberOfPhases = 100 },
            new Test() { Input = "03081770884921959731165446850517", Answer = "53553731", NumberOfPhases = 100 }
        };

        foreach (var test in tests)
        {
            var pos = int.Parse(test.Input.Substring(0, 7));
            var chars = test.Input.ToCharArray();
            var realInput = new char[10000 * chars.Length];
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < chars.Length; j++)
                {
                    realInput[i * chars.Length + j] = chars[j];
                }
            }
            // var answer = ApplyFinal(realInput, 100);
            // var finalAnswer = answer.Substring(pos, 8);
            var finalAnswer = "NotImpl";
            if (test.Answer != finalAnswer)
            {
                Console.WriteLine($"Expected {test.Answer}, got {finalAnswer}");
                return false;
            }
        }

        return true;
    }

    private int[] BasePattern = {
        0,1,0,-1
    };

    private string ApplyFinal(char[] chars, int numberOfPhases)
    {
        var input = chars.Select(s => int.Parse(s.ToString())).ToArray();
        for (var i = 0; i < numberOfPhases; i++)
        {
            input = Apply(input);
        }
        var answer = string.Join("", input);
        return answer;
    }

    private int[] Apply(int[] input)
    {
        var answer = new int[input.Length];
        for (int outIndex = 0; outIndex < input.Length; outIndex++)
        {
            var val = 0;
            for (int inputIndex = outIndex; inputIndex < input.Length; inputIndex++)
            {
                var factor = patternVal(inputIndex, outIndex);
                val += input[inputIndex] * factor;
            }
            answer[outIndex] = Math.Abs(val) % 10;
        }
        return answer;
    }

    private int patternVal(int position, int element)
    {
        return BasePattern[((position + 1) / (element + 1)) % BasePattern.Length];
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        var input = File.ReadAllLines(Input, Encoding.UTF8)[0];
        var answer = ApplyFinal(input.ToCharArray(), 100).Substring(0, 8);
        return answer;
    }

    public string SolveB()
    {
        return "not impl";
    }
}
