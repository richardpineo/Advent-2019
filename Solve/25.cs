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
        if (isA)
        {
            var answer = findTheCode();
            // Console.WriteLine(fullTrace);
        }
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return findTheCode();
    }

    StringBuilder fullTrace = new StringBuilder();

    private string executeCommands(Intcode.State state, string[] commands, bool assumePrompt)
    {
        var lastLine = new StringBuilder();
        var commandOutput = new StringBuilder();

        var commandIndex = 0;
        var input = "";
        if (assumePrompt)
        {
            commandIndex++;
            input = commands[0] + "\n";
        }

        var index = 0;
        while (Intcode.Step(state))
        {
            var output = state.PopOutput();
            if (output.HasValue)
            {
                char c = (char)output.Value;
                fullTrace.Append(c);
                commandOutput.Append(c);
                switch (c)
                {
                    case '\n':
                        if (lastLine.ToString() == "Command?")
                        {
                            if (commandIndex < commands.Length)
                            {
                                input = commands[commandIndex++];
                                fullTrace.Append($"> {input}\n");
                            }
                            else
                            {
                                // We are done
                                return commandOutput.ToString();
                            }
                            input += '\n';
                            index = 0;
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
        return commandOutput.ToString();
    }

    private string findTheCode()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);
        var state = new Intcode.State(program);

        var fullTrace = new StringBuilder();
        var lastLine = new StringBuilder();
        string[] initializeCommands = File.ReadAllLines(CommandFile, Encoding.UTF8);
        string[] items = File.ReadAllLines(ItemFile, Encoding.UTF8);

        var dropAll = items.Select(i => "drop " + i).ToArray();

        // do the pre-canned commands
        var initializeOutput = executeCommands(state, initializeCommands, false);

        // Assume the answer is somewhere between 2 and 6 tries.
        var combinations = new List<IEnumerable<string>>();
        combinations.AddRange(Util.GetKCombs(items, 2));
        combinations.AddRange(Util.GetKCombs(items, 3));
        combinations.AddRange(Util.GetKCombs(items, 4));
        combinations.AddRange(Util.GetKCombs(items, 5));
        combinations.AddRange(Util.GetKCombs(items, 6));

        foreach (var combo in combinations)
        {
            // Drop all first
            var dropAllOutput = executeCommands(state, dropAll, true);

            // Generate commands for the items
            var comboCommands = combo.Select(i => "take " + i).ToArray();
            comboCommands = comboCommands.Append("east").ToArray();
            var commandOutput = executeCommands(state, comboCommands, true);

            if (!commandOutput.Contains("Alert!"))
            {
                // Find the number:
                var preceeding = "You should be able to get in by typing ";
                var index = commandOutput.IndexOf(preceeding);
                var startIndex = index + preceeding.Length;
                var endIndex = commandOutput.IndexOf(" ", startIndex);
                var answer = commandOutput.Substring(startIndex, endIndex - startIndex);
                return answer.ToString(); // 319815680
            }
        }

        // Interactive:
        while (true)
        {
            var input = Console.ReadLine();
            executeCommands(state, new[] { input }, true);
        }
    }
    public string SolveB()
    {
        return "Manual";
    }
}
