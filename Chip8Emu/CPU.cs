using System;

namespace Chip8Emu
{
    class CPU
    {
        private byte[] registers;

        private ushort I;

        private byte delayTimer;
        private byte soundTimer;

        private ushort pc;

        private byte sp;

        // array of 16 16 bit values
        private ushort[] stack;

        private bool running;

        // holds the actual bits that will be displayed
        private byte[,] display;


        public CPU()
        {
            registers = new byte[16];

            I = 0x0000;

            delayTimer = 0x00;
            soundTimer = 0x00;

            pc = 0x0200;

            stack = new ushort[16];
            sp = 0x00;

            for (int i = 0; i < stack.Length; i++)
            {
                stack[i] = 0x0000;
            }

            display = new byte[64, 32];
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    display[i, j] = 0x00;
                }
            }

        }

        public void Run(byte[] memory)
        {
            running = true;
            ushort left, right;
            while (running)
            {
                // left and right portions of the total instruction
                

                // leftmost byte is located at pc
                left = memory[pc];

                // move it to the left side of the ushort
                left <<= 8;

                // rightmost byte is at pc + 1
                right = memory[pc + 1];
                // left now becomes the complete instruction, ready to be processed
                left |= right;

                ProcessInstruction(left, ref memory);
                Console.WriteLine();

                //running = false;
            }

        }

        private void ProcessInstruction(ushort instr, ref byte[] memory)
        {
            Console.WriteLine("Instruction ushort: {0:X4}", instr);

            // first (left) byte is at index 0 of the array
            byte[] instrBytes= BitConverter.GetBytes(instr);
            Array.Reverse(instrBytes);

            byte leftNibble = (byte) (instrBytes[0] & 0xF0);

            // Determine what to do based on the left byte of instruction
            switch (leftNibble)
            {
                case 0x00:
                    switch (instrBytes[1])
                    {
                        case 0xE0:
                            Console.WriteLine("CLEAR THE DISPLAY");
                            break;
                        case 0xEE:
                            Console.WriteLine("RETURN FROM SUBROUTINE");
                            break;
                        default:
                            Console.WriteLine("Error processing instruction");
                            break;
                    }
                    break;

                case 0x20:
                    ushort address = (ushort) (instr & 0x0FFF);
                    Console.WriteLine("Calling a Subroutine at {0:X4}", address);
                    sp += 0x01;
                    stack[sp] = pc;
                    pc = address;

                    break;

                case 0x60:
                    Console.WriteLine("Load into register");
                    byte regNum = (byte) (instrBytes[0] & 0x0F);

                    Console.WriteLine("Reg Num: {0:X}", regNum);
                    registers[regNum] = instrBytes[1];
                    Console.WriteLine("Register now has: {0:X2}", registers[regNum]);
                    pc += 2;
                    break;

                case 0xA0:
                    ushort val = (ushort) (instr & 0x0FFF);
                    Console.WriteLine("Setting I to {0:X4}", val);
                    I = val;
                    pc += 2;
                    break;

                case 0xD0:
                    // TODO: Implement sprite wraparound
                    byte regX = (byte) (instrBytes[0] & 0x0F);
                    byte regY = (byte) ((instrBytes[1] & 0xF0) >> 4);

                    Console.WriteLine("Attempting to draw to display at ({0}, {1})", registers[regX], registers[regY]);
                    byte bytesToDraw = (byte) (instrBytes[1] & 0x0F);

                    for (byte i = 0; i < bytesToDraw; i++)
                    {
                        Console.WriteLine("{0:X2}", memory[I + i]);
                        byte sprite = memory[I + i];
                        
                        for (int x = 0; x < 8; x++)
                        {
                            if ((byte) (sprite & 0x80) > 0)
                            {
                                // if something gets overwritten
                                if (display[registers[regX] + x, registers[regY] + i] == 1)
                                {
                                    registers[0xF] = 1;
                                }

                                display[registers[regX] + x, registers[regY] + i] = 1;
                            }
                            sprite <<= 1;
                        }
                    } 
                    PrintDisplay();

                    pc += 2;
                    break;

                default:
                    Console.WriteLine("Unknown Instruction\nAborting");
                    running = false;
                    break;
            }

        }

        void PrintDisplay()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------");
            for (int j = 0; j < 32; j++)
            {
                Console.Write("|");
                for (int i = 0; i < 64; i++)
                {
                    if (display[i, j] == 0) { Console.Write("  "); }
                    else { Console.Write("88"); }
                }
                Console.Write("|");
                Console.WriteLine();
            }
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------");
        }
    }
}
