/*
Nes emulator created in c#

With special thanks to olc https://onelonecoder.com/ for his tutorials on the nes which this is based on.
 */
using System;
using System.Text;
using NES.Components;
using NES.Components.CPU;

public class Debug
{

    public static string Decompile(){
        StringBuilder sb = new StringBuilder();
        var v =  Bus.GetData(Bus.cpu.pc);
        sb.AppendLine($"Current instruction is {v.ToString("X")}");
        sb.AppendLine($"{_6502.InstructionList[v].name}");
        sb.AppendLine($"Takes { _6502.InstructionList[v].cycles} cycles");

        return sb.ToString();
    }

    public static string GetCpuStatus(){
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(GetStatusReg());
        sb.AppendLine();
        sb.AppendLine(GetPC());
        sb.AppendLine(GetSP());
        sb.AppendLine(GetRegA());
        sb.AppendLine(GetRegX());
        sb.AppendLine(GetRegY());


        return sb.ToString();
    }
    public static string GetPC()
    {
        return Get16Bit(Bus.cpu.pc, "PC");
    }

    public static string GetSP()
    {
        return Get16Bit(Bus.cpu.stkp, "SP");
    }

    public static string GetRegA()
    {
        return GetReg(Bus.cpu.a, "A");
    }

    public static string GetRegX()
    {
        return GetReg(Bus.cpu.x, "X");
    }

    public static string GetRegY()
    {
        return GetReg(Bus.cpu.y, "Y");
    }


    public static string GetStatusReg()
    {


        StringBuilder sb = new StringBuilder();
        Func<byte, string> x = (v) =>
        {
            return v > 0 ? " 1" : " 0";
        };

        sb.AppendLine("STATUS: N V - B D I Z C");
        sb.Append("       ");
        sb.Append(x(Bus.cpu.GetFlag(_6502.FLAGS.N)));
        sb.Append(x(Bus.cpu.GetFlag(_6502.FLAGS.V)));
        sb.Append(" -");
        sb.Append(x(Bus.cpu.GetFlag(_6502.FLAGS.B)));
        sb.Append(x(Bus.cpu.GetFlag(_6502.FLAGS.D)));
        sb.Append(x(Bus.cpu.GetFlag(_6502.FLAGS.I)));
        sb.Append(x(Bus.cpu.GetFlag(_6502.FLAGS.Z)));
        sb.Append(x(Bus.cpu.GetFlag(_6502.FLAGS.C)));

        return sb.ToString();
    }

    public static string ArrayToDebugView(byte[] result, int start, int end)
    {
        int len = (end - start) + 1;
        StringBuilder sb = new StringBuilder();

        int wrapAt = 15;
        int i = 1;


        sb.Append(start.ToString("X").PadLeft(4, '0'));
        sb.Append(": ");


        for (int idx = 0; idx < len; idx++)
        {
            var v = result[idx];
            sb.Append(v.ToString("X").PadLeft(2, '0'));
            sb.Append(" ");

            if (wrapAt == 0)
            {
                wrapAt = 16;
                if (idx + 1 < len)
                {
                    sb.AppendLine();
                    sb.Append(((i * 16) + start).ToString("X").PadLeft(4, '0'));
                    sb.Append(": ");
                }
                i++;

            }
            wrapAt--;
        }


        return sb.ToString();
    }

    private static string Get16Bit(ushort v, string name)
    {
        return $"{name}: $" + v.ToString("X").PadLeft(4, '0');
    }

    private static string GetReg(byte reg, string name)
    {
        return $" {name}: $" + reg.ToString("X").PadLeft(2, '0');
    }
}