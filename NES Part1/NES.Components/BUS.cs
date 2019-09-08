/*
Nes emulator created in c#

With special thanks to olc https://onelonecoder.com/ for his tutorials on the nes which this is based on.
 */

using System;
using System.Text;
using NES.Components.CPU;

namespace NES.Components
{
    public class Bus
    {
        public static _6502 cpu;
        static byte[] ram;

        public Bus()
        {
            //64kb of memory on the nes
            ram = new byte[64 * 1024];

            //init it all to 0, just incase we create a new bus for some unknown reason atm
            for (int i = 0; i < ram.Length; i++)
            {
                ram[i] = 0x00;
            }



            //Lets add a cpu to the bus.
            cpu = new _6502();
            cpu.ConnectBus(this);

            ram[0xFFFC] = 0x00;
		    ram[0xFFFD] = 0x80;

            cpu.reset();
           Tick();
            
        }

        public void write(ushort addr, byte data)
        {
            if (addr >= 0x0 && addr <= 0xFFFF)
            {
                ram[addr] = data;
            }
        }
        public byte read(ushort addr)
        {
            if (addr >= 0x0000 && addr <= 0xFFFF)
                return ram[addr];

            return 0x00;
        }

        public byte[] GetAddressRange(int start, int end){
            int len = (end-start)+1;
            byte[] result = new byte[len];
            Array.Copy(ram, start, result, 0 , len);
           
            return result; 
        }
        
        public static byte GetData(int address){
            return ram[address];
        }

        public static void SetTestPrg(int start, byte[] app){
            Array.Copy(app,0,ram,start, app.Length);
        }




        

        public void Tick(){
            cpu.clock();
        }
    }

}