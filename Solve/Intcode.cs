using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class Intcode
{
    public static int[] ParseInput(string input)
    {
        var list = new List<int>();
        var inputs = input.Split(",");
        foreach (var i in inputs)
        {
            list.Add(int.Parse(i));
        }
        return list.ToArray();
    }

    public class State
    {
        public int? output;
        public int? input;
        public int pos = 0;
        public int[] program;
        public int relative;
    }

    private enum Mode
    {
        Immediate,
        Position,
        Relative
    }

    private static Mode[] getModes(int command)
    {
        var numArgs = 4; // max of 4 args.
        var bitfield = command / 100;
        var modes = new Mode[numArgs];
        for (int i = 0; i < numArgs; i++)
        {
            Mode mode;
            switch (bitfield % 10)
            {
                case 0:
                    mode = Mode.Immediate;
                    break;
                case 1:
                    mode = Mode.Position;
                    break;
                case 2:
                    mode = Mode.Relative;
                    break;
                default:
                    throw new Exception("bad mode! " + (bitfield % 10).ToString());

            }
            modes[i] = mode;
            bitfield = bitfield / 10;
        }
        return modes;
    }

    public static bool Step(State state)
    {
        ref int pos = ref state.pos;
        var program = state.program;
        int command = program[pos];
        int opCode = command % 100;
        var modes = getModes(command);

        switch (opCode)
        {
            case 1:
                pos += add(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
                return true;
            case 2:
                pos += multiply(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
                return true;
            case 3:
                pos += read(program[pos + 1], ref state.input, program);
                return true;
            case 4:
                pos += write(modes, program[pos + 1], program, ref state.output);
                return true;
            case 5:
                pos = jumpTrue(modes, pos, program[pos + 1], program[pos + 2], program);
                return true;
            case 6:
                pos = jumpFalse(modes, pos, program[pos + 1], program[pos + 2], program);
                return true;
            case 7:
                pos += lessThan(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
                return true;
            case 8:
                pos += equals(modes, program[pos + 1], program[pos + 2], program[pos + 3], program);
                return true;
            case 9:
                pos += relative(modes, program[pos + 1], program, ref state.relative);
                return true;

            case 99:
                pos = -1;
                return false;
        }
        throw new Exception("opcode is out of range");
    }

    private static int getValue(Mode mode, int op, int[] program)
    {
        switch (mode)
        {
            case Mode.Immediate:
                return program[op];
            case Mode.Position:
                return op;
            case Mode.Relative:
                // FIXME
                return op;
        }
        throw new Exception("Bad mode: " + mode.ToString());
    }

    static int add(Mode[] modes, int op1, int op2, int op3, int[] program)
    {
        int val1 = getValue(modes[0], op1, program);
        int val2 = getValue(modes[1], op2, program);
        program[op3] = val1 + val2;
        return 4;
    }
    static int multiply(Mode[] modes, int op1, int op2, int op3, int[] program)
    {
        int val1 = getValue(modes[0], op1, program);
        int val2 = getValue(modes[1], op2, program);
        program[op3] = val1 * val2;
        return 4;
    }
    static int read(int op1, ref int? input, int[] program)
    {
        if (!input.HasValue)
        {
            return 0;
        }
        program[op1] = input.Value;
        input = null;
        return 2;
    }
    static int write(Mode[] modes, int op1, int[] program, ref int? output)
    {
        if (output.HasValue)
        {
            return 0;
        }
        output = getValue(modes[0], op1, program);
        return 2;
    }
    static int relative(Mode[] modes, int op1, int[] program, ref int relative)
    {
        relative += getValue(modes[0], op1, program);
        return 2;
    }

    static int jumpTrue(Mode[] modes, int pos, int op1, int op2, int[] program)
    {
        int val1 = getValue(modes[0], op1, program);
        int val2 = getValue(modes[1], op2, program);
        return val1 != 0 ? val2 : (pos + 3);
    }
    static int jumpFalse(Mode[] modes, int pos, int op1, int op2, int[] program)
    {
        int val1 = getValue(modes[0], op1, program);
        int val2 = getValue(modes[1], op2, program);
        return val1 == 0 ? val2 : (pos + 3);
    }
    static int lessThan(Mode[] modes, int op1, int op2, int op3, int[] program)
    {
        int val1 = getValue(modes[0], op1, program);
        int val2 = getValue(modes[1], op2, program);
        program[op3] = val1 < val2 ? 1 : 0;
        return 4;
    }
    static int equals(Mode[] modes, int op1, int op2, int op3, int[] program)
    {
        int val1 = getValue(modes[0], op1, program);
        int val2 = getValue(modes[1], op2, program);
        program[op3] = val1 == val2 ? 1 : 0;
        return 4;
    }
}
