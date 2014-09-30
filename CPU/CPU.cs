using System;

namespace CPU
{
    class WCPU
    {
        public const int OPCODE_FLAGS = 0x0000FFFF;

        /// <summary>
        /// Opcodes of the virtual WCPU
        /// </summary>
        /// Opcodes exist in the bottom 16 bits of the value.
        /// The top 16 bits can be used for two optional variable bytes.
        public enum OpCode : int
        {
            /// <summary>
            /// NO-OP
            /// </summary>
            NOP,
            /// <summary>
            /// Halt
            /// </summary>
            HLT,
            /// <summary>
            /// Load from memory
            /// </summary>
            /// Structure: |REG1|REG2|OP|
            /// REG1: Location register, containing the memory location to load
            /// REG2: Target register, will contain the loaded value after this operation is done
            LDM,
            /// <summary>
            /// Load a value to a register
            /// </summary>
            /// Structure: |REG|VAL|OP|
            /// REG: Register whose value will be set
            /// VAL: Value to set
            LDQ,
            /// <summary>
            /// Write to memory
            /// </summary>
            /// Structure: |REG1|REG2|OP|
            /// REG1: Location register, containing the memory location to write to
            /// REG2: Write register, containing the value to write to memory
            WRM,
            /// <summary>
            /// Signed add two values in registers
            /// </summary>
            /// Structure: |REG1|REG2|OP|
            /// REG 1: Source register 1 and target register, will contain the added value after this operation is done
            /// REG 2: Source register 2
            ADD,
            /// <summary>
            /// Sgined add a value in a register and another value
            /// </summary>
            /// Structure: |REG|VAL|OP|
            /// REG: Register whose value will be raised
            /// VAL: Value to add to the value in REG
            ADQ,
        }

        [Flags]
        public enum ErrFlags : int
        {
            ERR_UNKNOWN_OPCODE = 1 << 0,
            HALT = 1 << 31,
        }

        /// <summary>
        /// Public register count
        /// </summary>
        public const int REGISTER_COUNT = 16;
        private const int PRIVATE_REGISTER_COUNT = 1;
        private const byte PROGRAM_COUNTER_ID = 16;

        public const int RESERVED_MEMORY = 1024;
        private Memory memory;

        private int[] registers;

        private ErrFlags errorFlags;

        public WCPU(Memory memory)
        {
            this.memory = memory;

            this.registers = new int[REGISTER_COUNT + PRIVATE_REGISTER_COUNT];
        }

        public void Run()
        {
            while ((errorFlags & ErrFlags.HALT) != ErrFlags.HALT)
            {
                this.Decode(this.Fetch());
            }
        }

        private OpCode Fetch()
        {
            OpCode fetchedCode = (OpCode)this.memory.Read(this.registers[PROGRAM_COUNTER_ID]);
            this.RaiseProgramCounter();
            return fetchedCode;
        }

        private void Decode(OpCode opCode)
        {
            byte value1;
            byte value2;
            this.StripValues(opCode, out value1, out value2);
            switch ((OpCode)((int)opCode & OPCODE_FLAGS))
            {
                case OpCode.NOP:
                    break;
                case OpCode.HLT:
                    this.Hlt();
                    break;
                case OpCode.LDM:
                    this.Ldm(value1, value2);
                    break;
                case OpCode.LDQ:
                    this.Ldq(value1, value2);
                    break;
                case OpCode.WRM:
                    this.Wrm(value1, value2);
                    break;
                case OpCode.ADD:
                    this.Add(value1, value2);
                    break;
                case OpCode.ADQ:
                    this.Adq(value1, value2);
                    break;
                default:
                    this.UnknownOpcode();
                    break;
            }
        }

        private void StripValues(OpCode opCode, out byte value1, out byte value2)
        {
            value1 = (byte)(((uint)opCode >> 24) & 0xFFU);
            value2 = (byte)(((uint)opCode >> 16) & 0xFFU);
        }

        private void RaiseProgramCounter()
        {
            this.Adq(PROGRAM_COUNTER_ID, 1);
        }

        private void UnknownOpcode() 
        {
            errorFlags |= ErrFlags.ERR_UNKNOWN_OPCODE;
            this.Hlt();
        }

        private void Hlt()
        {
            errorFlags |= ErrFlags.HALT;
        }

        private void Ldm(byte sourceRegister, byte targetRegister) 
        {
            registers[targetRegister] = this.memory.Read(registers[sourceRegister]);
        }

        private void Ldq(byte register, byte value)
        {
            registers[register] = value;
        }

        private void Wrm(byte locationRegister, byte valueRegister)
        {
            memory.Write(registers[locationRegister], registers[valueRegister]);
        }

        private void Add(byte register1, byte register2)
        {
            registers[register1] = registers[register1] + registers[register2];
        }

        private void Adq(byte register, byte value)
        {
            registers[register] = registers[register] + value;
        }
    }
}
