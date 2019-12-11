using System;
using System.Collections.Generic;
using IntType = System.Int64;

class Intcode
{
    public static IntType[] ParseInput(string input)
    {
        var list = new List<IntType>();
        var inputs = input.Split(",");
        foreach (var i in inputs)
        {
            list.Add(IntType.Parse(i));
        }
        return list.ToArray();
    }

    public class State
    {
        public State(IntType[] program, IntType? initialInput = null)
        {
            for (IntType i = 0; i < program.Length; i++)
            {
                setAt(i, program[i]);
            }
            this.input = initialInput;
        }
        public IntType? output;
        public IntType? input;
        public IntType pos = 0;
        private Dictionary<IntType, IntType> memory = new Dictionary<IntType, IntType>();
        public IntType relativeBase;

        public IntType? PopOutput()
        {
            var value = output;
            if (output.HasValue)
            {
                output = null;
            }
            return value;
        }

        public IntType[] MemoryDump(IntType maxAddress)
        {
            var dump = new IntType[maxAddress];
            for (IntType i = 0; i < maxAddress; i++)
            {
                dump[i] = getAt(i);
            }
            return dump;
        }

        public IntType getAt(IntType index)
        {
            return memory.GetValueOrDefault(index);
        }
        public void setAt(IntType index, IntType value)
        {
            memory[index] = value;
        }
    }

    private enum Mode
    {
        Immediate,
        Position,
        Relative
    }

    private static Mode[] getModes(IntType command)
    {
        var numArgs = 4; // max of 4 args.
        var bitfield = command / 100;
        var modes = new Mode[numArgs];
        for (IntType i = 0; i < numArgs; i++)
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
        IntType command = state.getAt(state.pos);
        IntType opCode = command % 100;
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
                state.pos = -1;
                return false;
        }
        throw new Exception("opcode is out of range");
    }

    private static IntType getValue(Mode mode, IntType op, State state)
    {
        switch (mode)
        {
            case Mode.Immediate:
                return state.getAt(op);
            case Mode.Position:
                return op;
            case Mode.Relative:
                return state.getAt(state.relativeBase + op);
        }
        throw new Exception("Bad mode: " + mode.ToString());
    }
    private static IntType valueAt(Mode[] modes, State state, IntType index)
    {
        return getValue(modes[index], state.getAt(state.pos + index + 1), state);
    }
    private static void writeTo(State state, IntType index, IntType value)
    {
        IntType offset = state.pos + index + 1;
        IntType writeOffset = state.getAt(offset);
        state.setAt(writeOffset, value);
    }
    static void add(Mode[] modes, State state)
    {
        IntType sum = valueAt(modes, state, 0) + valueAt(modes, state, 1);
        writeTo(state, 2, sum);
        state.pos += 4;
    }
    static void multiply(Mode[] modes, State state)
    {
        IntType product = valueAt(modes, state, 0) * valueAt(modes, state, 1);
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
        IntType val1 = valueAt(modes, state, 0);
        IntType val2 = valueAt(modes, state, 1);
        state.pos = val1 != 0 ? val2 : (state.pos + 3);
    }
    static void jumpFalse(Mode[] modes, State state)
    {
        IntType val1 = valueAt(modes, state, 0);
        IntType val2 = valueAt(modes, state, 1);
        state.pos = val1 == 0 ? val2 : (state.pos + 3);
    }
    static void lessThan(Mode[] modes, State state)
    {
        IntType val1 = valueAt(modes, state, 0);
        IntType val2 = valueAt(modes, state, 1);
        writeTo(state, 2, val1 < val2 ? 1 : 0);
        state.pos += 4;
    }
    static void equals(Mode[] modes, State state)
    {
        IntType val1 = valueAt(modes, state, 0);
        IntType val2 = valueAt(modes, state, 1);
        writeTo(state, 2, val1 == val2 ? 1 : 0);
        state.pos += 4;
    }

    static void relative(Mode[] modes, State state)
    {
        state.relativeBase += valueAt(modes, state, 0);
        state.pos += 2;
    }
}
