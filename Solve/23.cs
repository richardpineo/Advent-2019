using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

class Solve23 : ISolve
{
    public string Description()
    {
        return "Day 23: Category Six";
    }

    const string Input = "Input//23.txt";

    public bool Prove(bool isA)
    {
        return true;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    class Packet
    {
        public long X = -1;
        public long Y = -1;

        public bool SentX = false;
    }

    class PacketToSend
    {
        public long Destination = -1;
        public Packet ToSend = new Packet();
    }

    class Computer
    {
        public Computer(int address, long[] program, Action<PacketToSend> sendPacket)
        {
            packetSender = sendPacket;
            state = new Intcode.State(program, address);
        }

        public void QueuePacket(Packet packet)
        {
            queue.Enqueue(packet);
        }

        public void Step()
        {
            // Make sure the input is set to something.
            if (!state.input.HasValue)
            {
                if (Idle)
                {
                    state.input = -1;
                }
                else
                {
                    var packet = queue.Peek();
                    if (packet.SentX)
                    {
                        queue.Dequeue();
                        state.input = packet.Y;
                    }
                    else
                    {
                        packet.SentX = true;
                        state.input = packet.X;
                    }
                }
            }

            if (!Intcode.Step(state))
            {
                throw new Exception("oops?");
            }

            var output = state.PopOutput();
            if (output.HasValue)
            {
                if (toSend == null)
                {
                    toSend = new PacketToSend();
                    toSend.Destination = output.Value;
                }
                else if (!toSend.ToSend.SentX)
                {
                    toSend.ToSend.X = output.Value;
                    toSend.ToSend.SentX = true;
                }
                else
                {
                    // Send it
                    toSend.ToSend.Y = output.Value;
                    toSend.ToSend.SentX = false;
                    packetSender(toSend);
                    toSend = null;
                }
            }
        }

        public bool Idle
        {
            get
            {
                return queue.Count == 0;
            }
        }

        private Queue<Packet> queue = new Queue<Packet>();
        private Intcode.State state;
        private PacketToSend toSend;
        private Action<PacketToSend> packetSender;
    }

    public string SolveA()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);

        var computerBank = new Computer[50];
        const int numComputers = 50;

        long? answer = null;
        Action<PacketToSend> packetSender = (PacketToSend packet) =>
        {
            if (packet.Destination == 255)
            {
                answer = packet.ToSend.Y;
            }
            else
            {
                computerBank[packet.Destination].QueuePacket(packet.ToSend);
            }
        };

        for (int i = 0; i < numComputers; i++)
        {
            computerBank[i] = new Computer(i, program, packetSender);
        }

        while (!answer.HasValue)
        {
            foreach (var computer in computerBank)
            {
                computer.Step();
            }
        }
        return answer.Value.ToString();
    }

    class Nat
    {
        public void Receive(Packet packet)
        {
            current = packet;
        }

        public Packet current;
    }

    public string SolveB()
    {
        var lines = File.ReadAllLines(Input, Encoding.UTF8);
        var program = Intcode.ParseInput(lines[0]);

        var computerBank = new Computer[50];
        const int numComputers = 50;

        var nat = new Nat();

        long lastY = -666;
        long? answer = null;
        Action<PacketToSend> packetSender = (PacketToSend packet) =>
        {
            if (packet.Destination == 255)
            {
                nat.Receive(packet.ToSend);

                // Is anything processing?
                bool idle = computerBank.All(c => c.Idle);

                if (idle)
                {
                    if (lastY == nat.current.Y)
                    {
                        answer = lastY;
                    }
                    else
                    {
                        lastY = nat.current.Y;
                    }
                    computerBank[0].QueuePacket(nat.current);
                }
            }
            else
            {

                computerBank[packet.Destination].QueuePacket(packet.ToSend);
            }
        };

        for (int i = 0; i < numComputers; i++)
        {
            computerBank[i] = new Computer(i, program, packetSender);
        }

        while (!answer.HasValue)
        {
            foreach (var computer in computerBank)
            {
                computer.Step();
            }
        }
        return answer.Value.ToString();
    }
}
