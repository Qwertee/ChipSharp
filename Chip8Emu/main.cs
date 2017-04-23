using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu
{
    class main
    {
        static void Main(string[] args)
        {
            // the main memory for the emulator
            byte[] memory = new byte[4096];

            // cpu to run the programs
            CPU cpu = new CPU();

            // load the game file
            Console.WriteLine("Enter a file path for the rom");
//            var filePath = Console.ReadLine();
            byte[] rom = System.IO.File.ReadAllBytes("games\\pong");

            foreach (byte b in rom)
            {
                // Console.WriteLine("{0:X}", b);
            }

            // load the rom into the correct location in memory
            LoadRom(ref memory, ref rom);

            cpu.Run(memory);

            Console.ReadLine();
        }

        static void LoadRom(ref byte[] memory, ref byte[] rom)
        {
            int currAddr = 0x200;
            for (int i = 0; i < rom.Length; i++)
            {
                memory[currAddr] = rom[i];
                currAddr += 0x1;
            }
        }

        static void PrintMemory(ref byte[] memory)
        {
            for (int i = 0; i < memory.Length; i+=2)
            {
                Console.Write("{0:X2}" + " " +  "{1:X2}" + "\n", memory[i], memory[i + 1]);
            }
        }
    }
}
