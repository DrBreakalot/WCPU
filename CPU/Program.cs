using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] array = new int[] {OpcodeWithValues(WCPU.OpCode.LDQ, 0, 6), OpcodeWithValues(WCPU.OpCode.ADQ, 0, 4), OpcodeWithValues(WCPU.OpCode.LDQ, 1, 5), OpcodeWithValues(WCPU.OpCode.WRM, 1, 0), OpcodeWithValues(WCPU.OpCode.HLT, 0, 0), 0};
            Memory memory = new Memory(array);
            memory.DumpToConsole();
            new WCPU(memory).Run();
            memory.DumpToConsole();
            Console.ReadKey();
        }

        private static int OpcodeWithValues(WCPU.OpCode opcode, byte value1, byte value2)
        {
            return ((int)opcode) | (value1 << 24) | (value2 << 16);
        }
    }
}
