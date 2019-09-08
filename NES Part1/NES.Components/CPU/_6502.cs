/*
Nes emulator created in c#

With special thanks to olc https://onelonecoder.com/ for his tutorials on the nes which this is based on.
 */
using System;

namespace NES.Components.CPU
{
    public partial class _6502
    {
        public enum FLAGS : byte
        {
            C = (1 << 0), // Carry bit
            Z = (1 << 1), // Zero
            I = (1 << 2), // Disable Interrupts
            D = (1 << 3), // Decimal Mode
            B = (1 << 4), // Break
            U = (1 << 5), // Unused
            V = (1 << 6), // Overflow
            N = (1 << 7)  // Negative
        }

        Bus bus;

        public byte a = 0x00; //Accumulator register
        public byte x = 0x00; //x register
        public byte y = 0x00; //y register
        public byte stkp = 0x00; //Stack pointer
        public ushort pc = 0x0000; // programe counter
        public byte status = 0x00; //Status register

        byte fetched = 0x00;
        ushort addr_abs = 0x0000;
        ushort addr_rel = 0x0000;
        byte opcode = 0x00;
        byte cycles = 0x00;

        public _6502()
        {
            BuildInstrtions();
        }

        public void ConnectBus(Bus bus)
        {

            this.bus = bus;


        }
        void write(int addr, byte data)
        {
            bus.write((ushort)addr, data);
        }
        void write(ushort addr, byte data)
        {
            bus.write(addr, data);
        }
        byte read(ushort addr)
        {
            return bus.read(addr);
        }

        byte read(int addr)
        {
            return read((ushort)addr);
        }


        public byte GetFlag(FLAGS flag)
        {
            byte f = (byte)flag;
            return ((status & f) > 0) ? (byte)1 : (byte)0;
        }

        void SetFlag(FLAGS flag, bool v)
        {
            byte f = (byte)flag;
            if (v)
                status |= f;
            else
                status &= (byte)~f; //TODO: need to check this
        }

        public void clock()
        {
            if (cycles == 0)
            {
                opcode = read(pc);
                pc++;

                cycles = InstructionList[opcode].cycles;
                byte additional_Cycle1 = InstructionList[opcode].addrMode();
                byte additional_Cycle2 = InstructionList[opcode].operate();

                cycles += (byte)(additional_Cycle1 & additional_Cycle2);

            }

            cycles--;
        }
        public void reset()
        {
            a = 0;
            x = 0;
            y = 0;
            stkp = 0xFD;
            status = (0x00 | (byte)FLAGS.U);

            addr_abs = 0xFFFC;

            ushort lo = read(addr_abs + 0);
            ushort hi = read(addr_abs + 1);

            pc = (ushort)((hi << 8) | lo);

            addr_rel = 0x0;
            addr_abs = 0x0;
            fetched = 0x0;
            cycles = 8;

        }


        public bool Complete()
        {
            return cycles == 0;
        }

        void irq()
        {
            if (GetFlag(FLAGS.I) == 0)
            {
                write(0x0100 + stkp, (byte)((pc >> 8) & 0x00FF));
                stkp--;
                write(0x0100 + stkp, (byte)(pc & 0x00FF));
                stkp--;

                SetFlag(FLAGS.B, false);
                SetFlag(FLAGS.U, true);
                SetFlag(FLAGS.I, true);

                write(0x0100 + stkp, status);
                stkp--;

                addr_abs = 0xFFFE;

                ushort lo = read(addr_abs + 0);
                ushort hi = read(addr_abs + 1);

                pc = (byte)((hi << 8) | lo);

                cycles = 7;

            }

        }

        void nmi()
        {

            write(0x0100 + stkp, (byte)((pc >> 8) & 0x00FF));
            stkp--;
            write(0x0100 + stkp, (byte)(pc & 0x00FF));
            stkp--;

            SetFlag(FLAGS.B, false);
            SetFlag(FLAGS.U, true);
            SetFlag(FLAGS.I, true);

            write(0x0100 + stkp, status);
            stkp--;

            addr_abs = 0xFFFA;

            ushort lo = read(addr_abs + 0);
            ushort hi = read(addr_abs + 1);

            pc = (byte)((hi << 8) | lo);

            cycles = 8;

        }

        byte fetch()
        {
            if (!(InstructionList[opcode].addrMode == IMP))
            {
                fetched = read(addr_abs);
            }

            return fetched;
        }




    }
}