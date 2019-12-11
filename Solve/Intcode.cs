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
                add(modes, state);
                return true;
            case 2:
                multiply(modes, state);
                return true;
            case 3:
                read(modes, state);
                return true;
            case 4:
                write(modes, state);
                return true;
            case 5:
                jumpTrue(modes, state);
                return true;
            case 6:
                jumpFalse(modes, state);
                return true;
            case 7:
                lessThan(modes, state);
                return true;
            case 8:
                equals(modes, state);
                return true;
            case 9:
                relative(modes, state);
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
                throw new Exception("Bad mode: " + mode.ToString());
                //        return op;
        }
        throw new Exception("Bad mode: " + mode.ToString());
    }
    private static int valueAt(Mode[] modes, State state, int index)
    {
        return getValue(modes[index], state.program[state.pos + index + 1], state.program);
    }
    private static void writeTo(State state, int index, int value)
    {
        int offset = state.pos + index + 1;
        int writeOffset = state.program[offset];
        state.program[writeOffset] = value;
    }
    static void add(Mode[] modes, State state)
    {
        int sum = valueAt(modes, state, 0) + valueAt(modes, state, 1);
        writeTo(state, 2, sum);
        state.pos += 4;
    }
    static void multiply(Mode[] modes, State state)
    {
        int product = valueAt(modes, state, 0) * valueAt(modes, state, 1);
        writeTo(state, 2, product);
        state.pos += 4;
    }
    static void read(Mode[] modes, State state)
    {
        if (state.input.HasValue)
        {
            writeTo(state, 0, state.input.Value);
            state.input = null;
            state.pos += 2;
        }
    }
    static void write(Mode[] modes, State state)
    {
        if (!state.output.HasValue)
        {
            state.output = valueAt(modes, state, 0);
            state.pos += 2;
        }
    }
    static void jumpTrue(Mode[] modes, State state)
    {
        int val1 = valueAt(modes, state, 0);
        int val2 = valueAt(modes, state, 1);
        state.pos = val1 != 0 ? val2 : (state.pos + 3);
    }
    static void jumpFalse(Mode[] modes, State state)
    {
        int val1 = valueAt(modes, state, 0);
        int val2 = valueAt(modes, state, 1);
        state.pos = val1 == 0 ? val2 : (state.pos + 3);
    }
    static void lessThan(Mode[] modes, State state)
    {
        int val1 = valueAt(modes, state, 0);
        int val2 = valueAt(modes, state, 1);
        writeTo(state, 2, val1 < val2 ? 1 : 0);
        state.pos += 4;
    }
    static void equals(Mode[] modes, State state)
    {
        int val1 = valueAt(modes, state, 0);
        int val2 = valueAt(modes, state, 1);
        writeTo(state, 2, val1 == val2 ? 1 : 0);
        state.pos += 4;
    }

    static void relative(Mode[] modes, State state)
    {
        state.relative += valueAt(modes, state, 0);
        state.pos += 2;
    }
}
