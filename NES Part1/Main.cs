/*
Nes emulator created in c#

With special thanks to olc https://onelonecoder.com/ for his tutorials on the nes which this is based on.
 */
 
using System;
using NES.Components;
using NES.Components.CPU;

namespace NES
{
    class Program
    {
        
       // static GFX _ppu;

//        static Bus _bus;
        static void Main(string[] args)
        {


                using (var nes = new GFX(256, 240, true))
                {
                    nes.Run();
                }



/* 
            _bus = new Bus();

            _ppu = new GFX(256, 240, true);


            Clock c = new Clock(26.601712f);
            c.Tick += CPUTick;
            c.Start();


            while (true)
            {
                System.Threading.Thread.Sleep(1);
            }

*/


        }


        public static void CPUTick(float delay)
        {
            //_bus.Clock();
            //_cpu.Cycle();


        }
    }
}