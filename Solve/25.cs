using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class Solve25 : ISolve
{
    public string Description()
    {
        return "Day 25: Cryostasis";
    }

    const string Input = "Input//25.txt";
    const string CommandFile = "Examples//25-commands.txt";
    const string ItemFile = "Examples//25-items.txt";

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
        semiautomatic();
        return "Manual";
    }

    public void semiautomatic()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);

        var input = "";
        var fullTrace = new StringBuilder();
        var lastLine = new StringBuilder();
        var lastResponse = "";
        string[] commands = File.ReadAllLines(CommandFile, Encoding.UTF8);
        string[] items = File.ReadAllLines(ItemFile, Encoding.UTF8);
        var commandIndex = 0;

        var dropAll = items.Select(i => "drop " + i);
        var isCarrying = true;

        // Assume the answer is somewhere between 2 and 6 tries.
        var combinations = new List<IEnumerable<string>>();
        combinations.AddRange(Util.GetKCombs(items, 2));
        combinations.AddRange(Util.GetKCombs(items, 3));
        combinations.AddRange(Util.GetKCombs(items, 4));
        combinations.AddRange(Util.GetKCombs(items, 5));
        combinations.AddRange(Util.GetKCombs(items, 6));
        var combinationIndex = 0;

        var isTryingItem = false;

        var index = 0;
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                char c = (char)output.Value;
                Console.Write(c);
                fullTrace.Append(c);
                switch (c)
                {
                    case '\n':
                        if (lastLine.ToString() == "Command?")
                        {
                            if (isTryingItem)
                            {
                                if (!lastResponse.ToString().Contains("Alert!"))
                                {
                                    Console.WriteLine("EUREKA");
                                    Console.WriteLine(combinations[combinationIndex].Select(s => s).ToArray());
                                    return;
                                }
                            }
                            lastResponse = "";

                            if (commandIndex < commands.Length)
                            {
                                input = commands[commandIndex++];
                                Console.WriteLine("> " + input);
                            }
                            else
                            {
                                if (combinationIndex < combinations.Count)
                                {
                                    if (isCarrying)
                                    {
                                        isTryingItem = false;

                                        // Drop everything.
                                        commands = dropAll.ToArray();
                                        commandIndex = 0;
                                        isCarrying = false;
                                        break;
                                    }
                                    else
                                    {
                                        isTryingItem = true;
                                        // Now we are at the place with the stuff.
                                        var tryItems = combinations[combinationIndex++];
                                        // Generate commands for the items
                                        commands = tryItems.Select(i => "take " + i).ToArray();
                                        commands = commands.Append("east").ToArray();
                                        commandIndex = 0;
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Embarrassing failure");
                                    return;
                                }
                                // input = Console.ReadLine();
                            }
                            input += '\n';
                            index = 0;
                        }
                        lastResponse += "\n" + lastLine.ToString();
                        lastLine.Clear();
                        break;
                    default:
                        lastLine.Append(c);
                        break;
                }
            }
            if (!state.input.HasValue && index < input.Length)
            {
                state.input = input[index++];
            }
        }
    }
    public string SolveB()
    {
        return "Manual";
    }
}



/*

    public void interactive()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);

        var input = "";
        var fullTrace = new StringBuilder();
        var lastLine = new StringBuilder();
        var commands = new StringBuilder();

        var index = 0;
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                char c = (char)output.Value;
                Console.Write(c);
                fullTrace.Append(c);
                switch (c)
                {
                    case '\n':
                        if (lastLine.ToString() == "Command?")
                        {
                            // Dump previous commands
                            Console.WriteLine(commands);

                            // Waiting for input.
                            input = Console.ReadLine();
                            // Console.WriteLine("Input received: " + input);
                            input += '\n';
                            index = 0;
                            commands.Append(input);
                        }
                        lastLine.Clear();
                        break;
                    default:
                        lastLine.Append(c);
                        break;
                }
            }
            if (!state.input.HasValue && index < input.Length)
            {
                state.input = input[index++];
            }
        }
    }

*/
