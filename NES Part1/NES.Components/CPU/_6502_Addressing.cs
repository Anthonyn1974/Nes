using System;

namespace NES.Components.CPU
{

    public partial class _6502
    {
        byte IMP()
        {
            fetched = a;
            return 0;
        }

        byte IMM()
        {
            addr_abs = pc++;
            return 0;
        }

        byte ZP0()
        {
            addr_abs = read(pc);
            pc++;
            addr_abs &= 0x00FF;
            return 0;
        }

        byte ZPX()
        {
            addr_abs = (ushort)(read(pc) + x);
            pc++;
            addr_abs &= 0x00FF;
            return 0;
        }

        byte ZPY()
        {
            addr_abs = (ushort)(read(pc) + y);
            pc++;
            addr_abs &= 0x00FF;
            return 0;
        }

        byte ABS()
        {
            ushort lo = read(pc);
            pc++;
            ushort hi = read(pc);
            pc++;

            addr_abs = (ushort)((hi << 8) | lo);

            return 0;
        }



        byte ABX()
        {
            ushort lo = read(pc);
            pc++;
            ushort hi = read(pc);
            pc++;

            addr_abs = (ushort)((hi << 8) | lo);
            addr_abs += x;

            if ((addr_abs & 0xFF00) != (hi << 8))
            {
                return 1;
            }

            return 0;
        }

        byte ABY()
        {
            ushort lo = read(pc);
            pc++;
            ushort hi = read(pc);
            pc++;

            addr_abs = (ushort)((hi << 8) | lo);
            addr_abs += y;

            if ((addr_abs & 0xFF00) != (hi << 8))
            {
                return 1;
            }

            return 0;
        }

        byte IND()
        {
            ushort lo = read(pc);
            pc++;
            ushort hi = read(pc);
            pc++;

            ushort ptr = (ushort)((hi << 8) | lo);

            if (lo == 0x00FF) //simulate the page boundary hardware bug
            {
                addr_abs = (ushort)((read(ptr & 0xFF00) << 8) | read(ptr));
            }
            else
            {
                addr_abs = (ushort)((read(ptr + 1) << 8) | read(ptr));
            }

            return 0;
        }


        byte IZX()
        {
            ushort t = read(pc);
            pc++;

            ushort lo = read(((ushort)t + (ushort)x) & 0x00FF);
            ushort hi = read(((ushort)t + (ushort)x + 1) & 0x00FF);

            addr_abs = (ushort)((hi << 8) | lo);

            return 0;
        }



        byte IZY()
        {
            ushort t = read(pc);
            pc++;

            ushort lo = read(t & 0x00FF);
            ushort hi = read((t + 1) & 0x00FF);

            addr_abs = (ushort)((hi << 8) | lo);
            addr_abs += y;

            if (lo == 0x00FF)
            {
                return 1;
            }


            return 0;
        }

        byte REL()
        {
            addr_rel = read(pc);
            pc++;

            if ((addr_rel & 0x80) == 1) //TODO: need to check this
                addr_rel |= 0xFF00;

            return 0;
        }












    }

}