using System;

namespace NES.Components.CPU
{

    public partial class _6502
    {
        ushort temp = 0x0;

        byte ADC()
        {
            // Grab the data that we are adding to the accumulator
            fetch();

            // Add is performed in 16-bit domain for emulation to capture any
            // carry bit, which will exist in bit 8 of the 16-bit word
            temp = (ushort)((ushort)a + (ushort)fetched + (ushort)GetFlag(FLAGS.C));

            // The carry flag out exists in the high byte bit 0
            SetFlag(FLAGS.C, temp > 255);

            // The Zero flag is set if the result is 0
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0);

            // The signed Overflow flag is set based on all that up there! :D
            SetFlag(FLAGS.V, ((~((ushort)a ^ (ushort)fetched) & ((ushort)a ^ (ushort)temp)) & 0x0080) > 0);

            // The negative flag is set to the most significant bit of the result
            SetFlag(FLAGS.N, (temp & 0x80) > 0); //TODO: This needs checking

            // Load the result into the accumulator (it's 8-bit dont forget!)
            a = (byte)(temp & 0x00FF);

            // This instruction has the potential to require an additional clock cycle
            return 1;

        }
        byte AND()
        {
            fetch();
            a = (byte)(a & fetched);

            SetFlag(FLAGS.Z, a == 0x00);
            SetFlag(FLAGS.N, (a & 0x80) > 0);

            return 1;
        }
        byte ASL()
        {
            fetch();
            temp = (ushort)(fetched << 1);
            SetFlag(FLAGS.C, (temp & 0xFF00) > 0);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS.N, (temp & 0x80) > 0);

            if (InstructionList[opcode].addrMode == IMP)
                a = (byte)(temp & 0x00FF);
            else
                write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        byte BCC()
        {
            if (GetFlag(FLAGS.C) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte BCS()
        {
            if (GetFlag(FLAGS.C) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte BEQ()
        {
            if (GetFlag(FLAGS.Z) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte BIT()
        {
            fetch();
            temp = (ushort)(a & fetched);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS.N, (fetched & (1 << 7)) > 0);
            SetFlag(FLAGS.V, (fetched & (1 << 6)) > 0);
            return 0;
        }
        byte BMI()
        {
            if (GetFlag(FLAGS.N) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte BNE()
        {
            if (GetFlag(FLAGS.Z) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte BPL()
        {
            if (GetFlag(FLAGS.N) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte BRK()
        {
            pc++;

            SetFlag(FLAGS.I, true);
            write(0x0100 + stkp, (byte)((pc >> 8) & 0x00FF));
            stkp--;
            write(0x0100 + stkp, (byte)(pc & 0x00FF));
            stkp--;

            SetFlag(FLAGS.B, true);
            write(0x0100 + stkp, status);
            stkp--;
            SetFlag(FLAGS.B, false);

            pc = (ushort)((ushort)read(0xFFFE) | ((ushort)read(0xFFFF) << 8));
            return 0;
        }
        byte BVC()
        {
            if (GetFlag(FLAGS.V) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte BVS()
        {
            if (GetFlag(FLAGS.V) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                {
                    cycles++;
                }
                pc = addr_abs;
            }
            return 0;
        }
        byte CLC()
        {
            SetFlag(FLAGS.C, false);
            return 0;
        }
        byte CLD()
        {
            SetFlag(FLAGS.D, false);
            return 0;
        }
        byte CLI()
        {
            SetFlag(FLAGS.I, false);
            return 0;
        }
        byte CLV()
        {
            SetFlag(FLAGS.V, false);
            return 0;
        }
        byte CMP()
        {
            fetch();
            temp = (ushort)((ushort)a - (ushort)fetched);
            SetFlag(FLAGS.C, a >= fetched);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            return 1;
        }
        byte CPX()
        {
            fetch();
            temp = (ushort)((ushort)x - (ushort)fetched);
            SetFlag(FLAGS.C, x >= fetched);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            return 0;
        }
        byte CPY()
        {
            fetch();
            temp = (ushort)((ushort)y - (ushort)fetched);
            SetFlag(FLAGS.C, y >= fetched);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            return 0;
        }
        byte DEC()
        {
            fetch();
            temp = (ushort)(fetched - 1);
            write(addr_abs, (byte)(temp & 0x00FF));
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            return 0;
        }
        byte DEX()
        {
            x--;
            SetFlag(FLAGS.Z, x == 0x00);
            SetFlag(FLAGS.N, (x & 0x80) > 0);
            return 0;
        }
        byte DEY()
        {
            y--;
            SetFlag(FLAGS.Z, y == 0x00);
            SetFlag(FLAGS.N, (y & 0x80) > 0);
            return 0;
        }
        byte EOR()
        {
            fetch();
            a = (byte)(a ^ fetched);
            SetFlag(FLAGS.Z, a == 0x00);
            SetFlag(FLAGS.N, (a & 0x80) > 0);
            return 1;
        }
        byte INC()
        {
            fetch();
            temp = (ushort)(fetched + 1);
            write(addr_abs, (byte)(temp & 0x00FF));
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            return 0;
        }
        byte INX()
        {
            x++;
            SetFlag(FLAGS.Z, x == 0x00);
            SetFlag(FLAGS.N, (x & 0x80) > 0);
            return 0;
        }
        byte INY()
        {
            y++;
            SetFlag(FLAGS.Z, y == 0x00);
            SetFlag(FLAGS.N, (y & 0x80) > 0);
            return 0;
        }
        byte JMP()
        {
            pc = addr_abs;
            return 0;
        }
        byte JSR()
        {
            pc--;

            write(0x0100 + stkp, (byte)((pc >> 8) & 0x00FF));
            stkp--;
            write(0x0100 + stkp, (byte)(pc & 0x00FF));
            stkp--;

            pc = addr_abs;
            return 0;
        }
        byte LDA()
        {
            fetch();
            a = fetched;
            SetFlag(FLAGS.Z, a == 0x00);
            SetFlag(FLAGS.N, (a & 0x80) > 0);
            return 1;
        }
        byte LDX()
        {
            fetch();
            x = fetched;
            SetFlag(FLAGS.Z, x == 0x00);
            SetFlag(FLAGS.N, (x & 0x80) > 0);
            return 1;
        }
        byte LDY()
        {
            fetch();
            y = fetched;
            SetFlag(FLAGS.Z, y == 0x00);
            SetFlag(FLAGS.N, (y & 0x80) > 0);
            return 1;
        }
        byte LSR()
        {
            fetch();
            SetFlag(FLAGS.C, (fetched & 0x0001) > 0);
            temp = (ushort)(fetched >> 1);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            if (InstructionList[opcode].addrMode == IMP)
                a = (byte)(temp & 0x00FF);
            else
                write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        byte NOP()
        {
            // Sadly not all NOPs are equal, Ive added a few here
            // based on https://wiki.nesdev.com/w/index.php/CPU_unofficial_opcodes
            // and will add more based on game compatibility, and ultimately
            // I'd like to cover all illegal opcodes too
            switch (opcode)
            {
                case 0x1C:
                case 0x3C:
                case 0x5C:
                case 0x7C:
                case 0xDC:
                case 0xFC:
                    return 1;
            }
            return 0;
        }
        byte ORA()
        {
            fetch();
            a = (byte)(a | fetched);
            SetFlag(FLAGS.Z, a == 0x00);
            SetFlag(FLAGS.N, (a & 0x80) > 0);
            return 1;
        }
        byte PHA()
        {
            write((ushort)(0x0100 + stkp), a);
            stkp--;
            return 0;
        }
        byte PHP()
        {
            write(0x0100 + stkp, (byte)(status | (byte)FLAGS.B | (byte)FLAGS.U));
            SetFlag(FLAGS.B, false);
            SetFlag(FLAGS.U, false);
            stkp--;
            return 0;
        }
        byte PLA()
        {
            stkp++;
            a = read(0x0100 + stkp);
            SetFlag(FLAGS.Z, a == 0x00);
            SetFlag(FLAGS.N, (a & 0x80) > 0);
            return 0;
        }
        byte PLP()
        {
            stkp++;
            status = read(0x0100 + stkp);
            SetFlag(FLAGS.U, true);
            return 0;
        }
        byte ROL()
        {
            fetch();
            temp = (ushort)((ushort)(fetched << 1) | GetFlag(FLAGS.C));
            SetFlag(FLAGS.C, (temp & 0xFF00) > 0);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            if (InstructionList[opcode].addrMode == IMP)
                a = (byte)(temp & 0x00FF);
            else
                write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        byte ROR()
        {
            fetch();
            temp = (ushort)((GetFlag(FLAGS.C) << 7) | (fetched >> 1));
            SetFlag(FLAGS.C, (fetched & 0x01) > 0);
            SetFlag(FLAGS.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            if (InstructionList[opcode].addrMode == IMP)
                a = (byte)(temp & 0x00FF);
            else
                write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        byte RTI()
        {
            stkp++;
            status = read(0x0100 + stkp);
            status &= (byte)~FLAGS.B;
            status &= (byte)~FLAGS.U;

            stkp++;
            status = read(0x0100 + stkp);
            stkp++;
            status |= (byte)(read(0x0100 + stkp) << 8);


            return 0;
        }
        byte RTS()
        {
            stkp++;
            pc = (ushort)read(0x0100 + stkp);
            stkp++;
            pc |= (ushort)(read(0x0100 + stkp) << 8);

            pc++;
            return 0;
        }
        byte SBC()
        {
            fetch();

            // Operating in 16-bit domain to capture carry out

            // We can invert the bottom 8 bits with bitwise xor
            ushort value = (ushort)(((ushort)fetched) ^ 0x00FF);

            // Notice this is exactly the same as addition from here!
            temp = (ushort)((ushort)a + value + (ushort)GetFlag(FLAGS.C));
            SetFlag(FLAGS.C, (temp & 0xFF00) > 0);
            SetFlag(FLAGS.Z, ((temp & 0x00FF) == 0));
            SetFlag(FLAGS.V, ((temp ^ (ushort)a) & (temp ^ value) & 0x0080) > 0);
            SetFlag(FLAGS.N, (temp & 0x0080) > 0);
            a = (byte)(temp & 0x00FF);
            return 1;
        }
        byte SEC()
        {
            SetFlag(FLAGS.C, true);
            return 0;
        }
        byte SED()
        {
            SetFlag(FLAGS.D, true);
            return 0;
        }
        byte SEI()
        {
            SetFlag(FLAGS.I, true);
            return 0;
        }
        byte STA()
        {
            write(addr_abs, a);
            return 0;
        }
        byte STX()
        {
            write(addr_abs, x);
            return 0;
        }
        byte STY()
        {
            write(addr_abs, y);
            return 0;
        }
        byte TAX()
        {
            x = a;
            SetFlag(FLAGS.Z, x == 0x00);
            SetFlag(FLAGS.N, (x & 0x80) > 0);
            return 0;
        }
        byte TAY()
        {

            y = a;
            SetFlag(FLAGS.Z, y == 0x00);
            SetFlag(FLAGS.N, (y & 0x80) > 0);
            return 0;
        }
        byte TSX()
        {
            x = stkp;
            SetFlag(FLAGS.Z, x == 0x00);
            SetFlag(FLAGS.N, (x & 0x80) > 0);
            return 0;
        }
        byte TXA()
        {
            a = x;
            SetFlag(FLAGS.Z, a == 0x00);
            SetFlag(FLAGS.N, (a & 0x80) > 0);
            return 0;
        }
        byte TXS()
        {
            stkp = x;
            return 0;
        }
        byte TYA()
        {
            a = y;
            SetFlag(FLAGS.Z, a == 0x00);
            SetFlag(FLAGS.N, (a & 0x80) > 0);
            return 0;
        }

        byte XXX() { return 0; }
    }

}