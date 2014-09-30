using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU
{
    class Memory
    {
        private int[] memory;

        public Memory(uint size)
        {
            memory = new int[size];
        }

        public Memory(int[] memory)
        {
            this.memory = (int[])memory.Clone();
        }

        public int Read(int location)
        {
            return memory[location];
        }

        public void Write(int location, int value)
        {
            memory[location] = value;
        }

        public void DumpToConsole()
        {
            for (int i = 0; i < memory.Length; i++)
            {
                Console.WriteLine("{0}: {1:X8}", i, memory[i]);
            }
        }
    }
}
