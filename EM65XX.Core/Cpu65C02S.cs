using EM65XX.Core.Abstraction;
using EM65XX.Core.Attributes;
using EM65XX.Core.Enums;
using EM65XX.Core.Extensions;
using System.Reflection;

namespace EM65XX.Core;

public partial class Cpu65C02S : ICentralProcessingUnit
{
    private const byte STACK_PAGE = 1;

    private readonly Dictionary<Mnemonic, Action<AddressingMode>> _handlers = [];

    public Cpu65C02S(IMemory memory)
    {
        Memory = memory;

        Stack = new(memory, STACK_PAGE);
        Registers = new(Stack);

        RegisterHandlers();
    }

    public CpuState State { get; private set; }
    public Registers Registers { get; }

    public byte OpCode => Memory[Registers.ProgramCounter];

    public IMemory Memory { get; }
    private PageStack Stack { get; }

    public void Reset()
    {
        State = CpuState.Running;

        Registers.UpdateFlags(Flags.Break | Flags.Interrupt, true);
        Registers.UpdateFlags(Flags.Decimal, false);

        Registers.ProgramCounter = 0xFFFC;
        Registers.ProgramCounter = ReadAddress(AddressingMode.Absolute);
    }

    public void Tick()
    {
        if (State == CpuState.Stopped)
            return;

        var instruction = InstructionsTable.Get(OpCode);
        Registers.ProgramCounter++;

        var handler = _handlers[instruction.Mnemonic];
        handler(instruction.Mode);
    }

    #region Arithmetic

    /// <summary>
    /// A + M + C -> A
    /// </summary>
    [Instruction(Mnemonic.ADC)]
    private void ADC(AddressingMode mode)
    {
        var address = ReadAddress(mode);
        var memValue = Memory[address];

        var carry = Registers.StatusFlags.FlagToByte(Flags.Carry);
        var value = Registers.A + memValue + carry;

        Registers.UpdateFlags(Flags.Overflow,
            (((Registers.A ^ value) & 0x80) != 0) &&
            ((Registers.A ^ memValue) & 0x80) == 0);

        var decimalMode = Registers.StatusFlags.HasFlag(Flags.Decimal);
        if (decimalMode)
        {
            throw new NotImplementedException("Decimal mode is not implemented yet");
        }
        else
        {
            Registers.UpdateFlags(Flags.Carry, value > 255);
        }

        var byteValue = (byte)value;

        Registers.UpdateNZFlags(byteValue);
        Registers.A = byteValue;
    }

    /// <summary>
    /// A - M - (~C) -> A
    /// </summary>
    [Instruction(Mnemonic.SBC)]
    private void SBC(AddressingMode mode)
    {
        throw new NotImplementedException("SBC is not implemented yet");
    }

    #endregion

    #region Logic

    /// <summary>
    /// A ^ M -> A
    /// </summary>
    [Instruction(Mnemonic.AND)]
    private void AND(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.A = (byte)(Registers.A & Memory[address]);
        Registers.UpdateNZFlags(Registers.A);
    }

    /// <summary>
    /// A V M -> A
    /// </summary>
    [Instruction(Mnemonic.ORA)]
    private void ORA(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.A = (byte)(Registers.A | Memory[address]);
        Registers.UpdateNZFlags(Registers.A);
    }

    /// <summary>
    /// A v M -> A
    /// </summary>
    [Instruction(Mnemonic.EOR)]
    private void EOR(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.A = (byte)(Registers.A ^ Memory[address]);
        Registers.UpdateNZFlags(Registers.A);
    }

    #endregion

    #region Shifts / Rotates

    /// <summary>
    /// C <- 7 6 5 4 3 2 1 0 <- 0
    /// </summary>    
    [Instruction(Mnemonic.ASL)]
    private void ASL(AddressingMode mode)
    {
        if (mode == AddressingMode.Accumulator)
        {
            Registers.UpdateFlags(Flags.Carry, (Registers.A & 0x80) != 0);

            Registers.A <<= 1;
            Registers.UpdateNZFlags(Registers.A);
        }
        else
        {
            var address = ReadAddress(mode);
            Registers.UpdateFlags(Flags.Carry, (Memory[address] & 0x80) != 0);

            Memory[address] <<= 1;

            Registers.UpdateNZFlags(Memory[address]);
        }
    }

    /// <summary>
    /// 0 -> 7 6 5 4 3 2 1 0 -> C
    /// </summary>    
    [Instruction(Mnemonic.LSR)]
    private void LSR(AddressingMode mode)
    {
        if (mode == AddressingMode.Accumulator)
        {
            Registers.UpdateFlags(Flags.Carry, (Registers.A & 0x01) != 0);

            Registers.A >>= 1;
            Registers.UpdateNZFlags(Registers.A);
        }
        else
        {
            var address = ReadAddress(mode);
            Registers.UpdateFlags(Flags.Carry, (Memory[address] & 0x01) != 0);

            Memory[address] >>= 1;

            Registers.UpdateNZFlags(Memory[address]);
        }
    }

    /// <summary>
    /// C <- 7 6 5 4 3 2 1 0 <- C
    /// </summary>    
    [Instruction(Mnemonic.ROL)]
    private void ROL(AddressingMode mode)
    {
        var carry = Registers.StatusFlags.FlagToByte(Flags.Carry);
        if (mode == AddressingMode.Accumulator)
        {
            Registers.UpdateFlags(Flags.Carry, (Registers.A & 0x80) != 0);

            Registers.A = (byte)((Registers.A << 1) | carry);
            Registers.UpdateNZFlags(Registers.A);
        }
        else
        {
            var address = ReadAddress(mode);
            Registers.UpdateFlags(Flags.Carry, (Memory[address] & 0x80) != 0);

            Memory[address] = (byte)((Memory[address] << 1) | carry);
            Registers.UpdateNZFlags(Memory[address]);
        }
    }

    /// <summary>
    /// C -> 7 6 5 4 3 2 1 0 -> C
    /// </summary>    
    [Instruction(Mnemonic.ROR)]
    private void ROR(AddressingMode mode)
    {
        var carry = Registers.StatusFlags.FlagToByte(Flags.Carry);
        if (mode == AddressingMode.Accumulator)
        {
            Registers.UpdateFlags(Flags.Carry, (Registers.A & 0x01) != 0);

            Registers.A = (byte)((Registers.A >> 1) | (carry << 7));
            Registers.UpdateNZFlags(Registers.A);
        }
        else
        {
            var address = ReadAddress(mode);
            Registers.UpdateFlags(Flags.Carry, (Memory[address] & 0x01) != 0);

            Memory[address] = (byte)((Memory[address] >> 1) | (carry << 7));
            Registers.UpdateNZFlags(Memory[address]);
        }
    }

    #endregion

    #region Increments / Decrements

    /// <summary>
    /// Increments
    /// </summary>
    [Instruction(Mnemonic.INC)]
    private void INC(AddressingMode mode)
    {
        if (mode == AddressingMode.Accumulator)
        {
            Registers.A++;
            Registers.UpdateNZFlags(Registers.A);

        }
        else
        {
            var address = ReadAddress(mode);

            Memory[address]++;
            Registers.UpdateNZFlags(Memory[address]);
        }
    }

    /// <summary>
    /// X + 1 -> X
    /// </summary>
    [Instruction(Mnemonic.INX)]
    private void INX(AddressingMode mode)
    {
        Registers.X++;
        Registers.UpdateNZFlags(Registers.X);
    }

    /// <summary>
    /// Y + 1 -> Y
    /// </summary>
    [Instruction(Mnemonic.INY)]
    private void INY(AddressingMode mode)
    {
        Registers.Y++;
        Registers.UpdateNZFlags(Registers.Y);
    }

    /// <summary>
    /// Decrement
    /// </summary>
    [Instruction(Mnemonic.DEC)]
    private void DEC(AddressingMode mode)
    {
        if (mode == AddressingMode.Accumulator)
        {
            Registers.A--;
            Registers.UpdateNZFlags(Registers.A);

        }
        else
        {
            var address = ReadAddress(mode);

            Memory[address]--;
            Registers.UpdateNZFlags(Memory[address]);
        }
    }

    /// <summary>
    /// X + 1 -> X
    /// </summary>
    [Instruction(Mnemonic.DEX)]
    private void DEX(AddressingMode mode)
    {
        Registers.X--;
        Registers.UpdateNZFlags(Registers.X);
    }

    /// <summary>
    /// Y + 1 -> Y
    /// </summary>
    [Instruction(Mnemonic.DEY)]
    private void DEY(AddressingMode mode)
    {
        Registers.Y--;
        Registers.UpdateNZFlags(Registers.Y);
    }

    #endregion

    #region Loads    

    /// <summary>
    /// M -> A
    /// </summary>
    [Instruction(Mnemonic.LDA)]
    private void LDA(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.A = Memory[address];
        Registers.UpdateNZFlags(Registers.A);
    }

    /// <summary>
    /// M -> X
    /// </summary>
    [Instruction(Mnemonic.LDX)]
    private void LDX(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.X = Memory[address];
        Registers.UpdateNZFlags(Registers.X);
    }

    /// <summary>
    /// M -> Y
    /// </summary>
    [Instruction(Mnemonic.LDY)]
    private void LDY(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.Y = Memory[address];
        Registers.UpdateNZFlags(Registers.Y);
    }

    #endregion

    #region Stores

    /// <summary>
    /// A -> M
    /// </summary>    
    [Instruction(Mnemonic.STA)]
    private void STA(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = Registers.A;
    }

    /// <summary>
    /// X -> M
    /// </summary>    
    [Instruction(Mnemonic.STX)]
    private void STX(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = Registers.X;
    }

    /// <summary>
    /// Y -> M
    /// </summary>    
    [Instruction(Mnemonic.STY)]
    private void STY(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = Registers.Y;
    }

    /// <summary>
    /// 00 -> M
    /// </summary>    
    [Instruction(Mnemonic.STZ)]
    private void STZ(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = 0x00;
    }

    #endregion

    #region Transfers

    /// <summary>
    /// A -> X
    /// </summary>
    [Instruction(Mnemonic.TAX)]
    private void TAX(AddressingMode mode)
    {
        Registers.X = Registers.A;
        Registers.UpdateNZFlags(Registers.X);
    }

    /// <summary>
    /// A -> Y
    /// </summary>
    [Instruction(Mnemonic.TAY)]
    private void TAY(AddressingMode mode)
    {
        Registers.Y = Registers.A;
        Registers.UpdateNZFlags(Registers.Y);
    }

    /// <summary>
    /// X -> A
    /// </summary>
    [Instruction(Mnemonic.TXA)]
    private void TXA(AddressingMode mode)
    {
        Registers.A = Registers.X;
        Registers.UpdateNZFlags(Registers.A);
    }

    /// <summary>
    /// Y -> A
    /// </summary>
    [Instruction(Mnemonic.TYA)]
    private void TYA(AddressingMode mode)
    {
        Registers.A = Registers.Y;
        Registers.UpdateNZFlags(Registers.A);
    }

    /// <summary>
    /// S -> X
    /// </summary>
    [Instruction(Mnemonic.TSX)]
    private void TSX(AddressingMode mode)
    {
        Registers.X = Stack.Pointer;
        Registers.UpdateNZFlags(Registers.X);
    }

    /// <summary>
    /// X -> S
    /// </summary>
    [Instruction(Mnemonic.TXS)]
    private void TXS(AddressingMode mode)
    {
        Stack.Pointer = Registers.X;
    }

    #endregion

    #region Stack

    /// <summary>
    /// A -> Ms, S-1 -> S
    /// </summary>
    [Instruction(Mnemonic.PHA)]
    private void PHA(AddressingMode mode)
    {
        Stack.Push(Registers.A);
    }

    /// <summary>
    /// P -> Ms, S-1 -> S
    /// </summary>
    [Instruction(Mnemonic.PHP)]
    private void PHP(AddressingMode mode)
    {
        Stack.Push(Registers.ProcessorStatus);
    }

    /// <summary>
    /// X -> Ms, S-1 -> S
    /// </summary>
    [Instruction(Mnemonic.PHX)]
    private void PHX(AddressingMode mode)
    {
        Stack.Push(Registers.X);
    }

    /// <summary>
    /// Y -> Ms, S-1 -> S
    /// </summary>
    [Instruction(Mnemonic.PHY)]
    private void PHY(AddressingMode mode)
    {
        Stack.Push(Registers.Y);
    }

    /// <summary>
    /// S+1 -> S, Ms -> A
    /// </summary>
    [Instruction(Mnemonic.PLA)]
    private void PLA(AddressingMode mode)
    {
        Registers.A = Stack.Pop();
        Registers.UpdateNZFlags(Registers.A);
    }

    /// <summary>
    /// S+1 -> S, Ms -> P
    /// </summary>
    [Instruction(Mnemonic.PLP)]
    private void PLP(AddressingMode mode)
    {
        Registers.ProcessorStatus = Stack.Pop();
    }

    /// <summary>
    /// S+1 -> S, Ms -> X
    /// </summary>
    [Instruction(Mnemonic.PLX)]
    private void PLX(AddressingMode mode)
    {
        Registers.X = Stack.Pop();
        Registers.UpdateNZFlags(Registers.X);
    }

    /// <summary>
    /// S+1 -> S, Ms -> Y
    /// </summary>
    [Instruction(Mnemonic.PLY)]
    private void PLY(AddressingMode mode)
    {
        Registers.Y = Stack.Pop();
        Registers.UpdateNZFlags(Registers.Y);
    }

    #endregion

    #region Flags

    /// <summary>
    /// 0 -> C
    /// </summary>
    [Instruction(Mnemonic.CLC)]
    private void CLC(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Carry, false);
    }

    /// <summary>
    /// 1 -> C
    /// </summary>
    [Instruction(Mnemonic.SEC)]
    private void SEC(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Carry, true);
    }

    /// <summary>
    /// 0 -> I
    /// </summary>
    [Instruction(Mnemonic.CLI)]
    private void CLI(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Interrupt, false);
    }

    /// <summary>
    /// 1 -> I
    /// </summary>
    [Instruction(Mnemonic.SEI)]
    private void SEI(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Interrupt, true);
    }

    /// <summary>
    /// 0 -> V
    /// </summary>
    [Instruction(Mnemonic.CLV)]
    private void CLV(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Overflow, false);
    }

    /// <summary>
    /// 0 -> V
    /// </summary>
    [Instruction(Mnemonic.CLD)]
    private void CLD(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Decimal, false);
    }

    /// <summary>
    /// 1 -> V
    /// </summary>
    [Instruction(Mnemonic.SED)]
    private void SED(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Decimal, true);
    }

    #endregion

    #region Comparison

    /// <summary>
    /// A - M
    /// </summary>
    /// <param name="mode"></param>
    [Instruction(Mnemonic.CMP)]
    private void CMP(AddressingMode mode)
    {
        var address = ReadAddress(mode);
        Compare(Registers.A, Memory[address]);
    }

    /// <summary>
    /// X - M
    /// </summary>
    /// <param name="mode"></param>
    [Instruction(Mnemonic.CPX)]
    private void CPX(AddressingMode mode)
    {
        var address = ReadAddress(mode);
        Compare(Registers.X, Memory[address]);
    }

    /// <summary>
    /// Y - M
    /// </summary>
    /// <param name="mode"></param>
    [Instruction(Mnemonic.CPY)]
    private void CPY(AddressingMode mode)
    {
        var address = ReadAddress(mode);
        Compare(Registers.Y, Memory[address]);
    }

    /// <summary>
    /// a - b
    /// </summary>
    private void Compare(byte a, byte b)
    {
        var value = (byte)(a - b);

        Registers.UpdateNZFlags(value);
        Registers.UpdateFlags(Flags.Carry, a >= b);
    }

    #endregion

    #region Bit

    /// <summary>
    /// A & M
    /// </summary>
    [Instruction(Mnemonic.BIT)]
    private void BIT(AddressingMode mode)
    {
        var address = ReadAddress(mode);
        var value = Memory[address];

        Registers.UpdateFlags(Flags.Negative, (value & 0x80) == 0x80);
        Registers.UpdateFlags(Flags.Overflow, (value & 0x40) == 0x40);
        Registers.UpdateFlags(Flags.Zero, (value & Registers.A) == 0);
    }

    /// <summary>
    /// A V M -> M
    /// </summary>
    [Instruction(Mnemonic.TSB)]
    private void TSB(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = (byte)(Registers.A | Memory[address]);
        Registers.UpdateFlags(Flags.Zero, Memory[address] == 0);
    }

    /// <summary>
    /// ~A ^ M -> M
    /// </summary>
    [Instruction(Mnemonic.TRB)]
    private void TRB(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = (byte)(~Registers.A & Memory[address]);
        Registers.UpdateFlags(Flags.Zero, Memory[address] == 0);
    }


    #endregion

    #region Branches

    /// <summary>
    /// Branch if C=0
    /// </summary>    
    [Instruction(Mnemonic.BCC)]
    private void BCC(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (!Registers.StatusFlags.HasFlag(Flags.Carry))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch if C=1 
    /// </summary>    
    [Instruction(Mnemonic.BCS)]
    private void BCS(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (Registers.StatusFlags.HasFlag(Flags.Carry))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch if Z=1 
    /// </summary>    
    [Instruction(Mnemonic.BEQ)]
    private void BEQ(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (Registers.StatusFlags.HasFlag(Flags.Zero))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch if N=1
    /// </summary>    
    [Instruction(Mnemonic.BMI)]
    private void BMI(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (!Registers.StatusFlags.HasFlag(Flags.Negative))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch if Z=0 
    /// </summary>    
    [Instruction(Mnemonic.BNE)]
    private void BNE(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (Registers.StatusFlags.HasFlag(Flags.Negative))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch if N=0
    /// </summary>    
    [Instruction(Mnemonic.BPL)]
    private void BPL(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (Registers.StatusFlags.HasFlag(Flags.Negative))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch if V=0
    /// </summary>    
    [Instruction(Mnemonic.BVC)]
    private void BVC(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (!Registers.StatusFlags.HasFlag(Flags.Overflow))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch if V=1
    /// </summary>    
    [Instruction(Mnemonic.BVS)]
    private void BVS(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        if (Registers.StatusFlags.HasFlag(Flags.Overflow))
            Registers.ProgramCounter = address;
    }

    /// <summary>
    /// Branch Always
    /// </summary>    
    [Instruction(Mnemonic.BRA)]
    private void BRA(AddressingMode mode)
    {
        Registers.ProgramCounter = ReadAddress(mode);
    }

    #endregion

    #region Bit Branches

    /// <summary>
    /// Branch on bit 0 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR0)]
    private void BBR0(AddressingMode mode)
        => BB(0, false);
    
    /// <summary>
    /// Branch on bit 1 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR1)]
    private void BBR1(AddressingMode mode)
        => BB(1, false);

    /// <summary>
    /// Branch on bit 2 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR1)]
    private void BBR2(AddressingMode mode)
        => BB(2, false);

    /// <summary>
    /// Branch on bit 3 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR3)]
    private void BBR3(AddressingMode mode)
        => BB(3, false);

    /// <summary>
    /// Branch on bit 4 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR4)]
    private void BBR4(AddressingMode mode)
        => BB(4, false);

    /// <summary>
    /// Branch on bit 5 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR5)]
    private void BBR5(AddressingMode mode)
        => BB(5, false);

    /// <summary>
    /// Branch on bit 6 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR6)]
    private void BBR6(AddressingMode mode)
        => BB(6, false);

    /// <summary>
    /// Branch on bit 7 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR7)]
    private void BBR7(AddressingMode mode)
        => BB(7, false);


    /// <summary>
    /// Branch on bit 0 set
    /// </summary>    
    [Instruction(Mnemonic.BBS0)]
    private void BBS0(AddressingMode mode)
        => BB(0, true);

    /// <summary>
    /// Branch on bit 1 set
    /// </summary>    
    [Instruction(Mnemonic.BBS1)]
    private void BBS1(AddressingMode mode)
        => BB(1, true);

    /// <summary>
    /// Branch on bit 2 set
    /// </summary>    
    [Instruction(Mnemonic.BBS1)]
    private void BBS2(AddressingMode mode)
        => BB(2, true);

    /// <summary>
    /// Branch on bit 3 set
    /// </summary>    
    [Instruction(Mnemonic.BBS3)]
    private void BBS3(AddressingMode mode)
        => BB(3, true);

    /// <summary>
    /// Branch on bit 4 set
    /// </summary>    
    [Instruction(Mnemonic.BBS4)]
    private void BBS4(AddressingMode mode)
        => BB(4, true);

    /// <summary>
    /// Branch on bit 5 set
    /// </summary>    
    [Instruction(Mnemonic.BBS5)]
    private void BBS5(AddressingMode mode)
        => BB(5, true);

    /// <summary>
    /// Branch on bit 6 set
    /// </summary>    
    [Instruction(Mnemonic.BBS6)]
    private void BBS6(AddressingMode mode)
        => BB(6, true);

    /// <summary>
    /// Branch on bit 7 reset
    /// </summary>    
    [Instruction(Mnemonic.BBR7)]
    private void BBS7(AddressingMode mode)
        => BB(7, true);

    private void BB(int bit, bool target)
    {
        var address = ReadAddress(AddressingMode.ZeroPage);
        var next = ReadAddress(AddressingMode.ProgramCounterRelative);

        var value = Memory[address];
        var set = (value & (1 << bit)) != 0;

        if (set == target)
            Registers.ProgramCounter = next;
    }

    #endregion

    #region Bit Manipulation 

    /// <summary>
    /// Reset Memory Bit 0
    /// </summary>    
    [Instruction(Mnemonic.RMB0)]
    private void RMB0(AddressingMode mode)
        => RMB(0);

    /// <summary>
    /// Reset Memory Bit 0
    /// </summary>    
    [Instruction(Mnemonic.RMB1)]
    private void RMB1(AddressingMode mode)
        => RMB(1);

    /// <summary>
    /// Reset Memory Bit 2
    /// </summary>    
    [Instruction(Mnemonic.RMB2)]
    private void RMB2(AddressingMode mode)
        => RMB(2);

    /// <summary>
    /// Reset Memory Bit 3
    /// </summary>    
    [Instruction(Mnemonic.RMB3)]
    private void RMB3(AddressingMode mode)
        => RMB(3);

    /// <summary>
    /// Reset Memory Bit 4
    /// </summary>    
    [Instruction(Mnemonic.RMB4)]
    private void RMB4(AddressingMode mode)
        => RMB(4);

    /// <summary>
    /// Reset Memory Bit 5
    /// </summary>    
    [Instruction(Mnemonic.RMB5)]
    private void RMB5(AddressingMode mode)
        => RMB(5);

    /// <summary>
    /// Reset Memory Bit 6
    /// </summary>    
    [Instruction(Mnemonic.RMB6)]
    private void RMB6(AddressingMode mode)
        => RMB(6);

    /// <summary>
    /// Reset Memory Bit 7
    /// </summary>    
    [Instruction(Mnemonic.RMB7)]
    private void RMB7(AddressingMode mode)
        => RMB(7);

    /// <summary>
    /// Set Memory Bit 0
    /// </summary>    
    [Instruction(Mnemonic.SMB0)]
    private void SMB0(AddressingMode mode)
        => SMB(0);

    /// <summary>
    /// Set Memory Bit 0
    /// </summary>    
    [Instruction(Mnemonic.SMB1)]
    private void SMB1(AddressingMode mode)
        => SMB(1);

    /// <summary>
    /// Set Memory Bit 2
    /// </summary>    
    [Instruction(Mnemonic.SMB2)]
    private void SMB2(AddressingMode mode)
        => SMB(2);

    /// <summary>
    /// Set Memory Bit 3
    /// </summary>    
    [Instruction(Mnemonic.SMB3)]
    private void SMB3(AddressingMode mode)
        => SMB(3);

    /// <summary>
    /// Set Memory Bit 4
    /// </summary>    
    [Instruction(Mnemonic.SMB4)]
    private void SMB4(AddressingMode mode)
        => SMB(4);

    /// <summary>
    /// Set Memory Bit 5
    /// </summary>    
    [Instruction(Mnemonic.SMB5)]
    private void SMB5(AddressingMode mode)
        => SMB(5);

    /// <summary>
    /// Set Memory Bit 6
    /// </summary>    
    [Instruction(Mnemonic.SMB6)]
    private void SMB6(AddressingMode mode)
        => SMB(6);

    /// <summary>
    /// Set Memory Bit 7
    /// </summary>    
    [Instruction(Mnemonic.SMB7)]
    private void SMB7(AddressingMode mode)
        => SMB(7);

    private void RMB(int bit)
        => MB(bit, false);

    private void SMB(int bit)
        => MB(bit, true);

    private void MB(int bit, bool value)
    {
        var address = ReadAddress(AddressingMode.ZeroPage);

        if (value)
        {
            Memory[address] |= (byte)(1 << bit);
        }
        else
        {
            Memory[address] &= (byte)~(1 << bit);
        }
    }

    #endregion

    #region Jumps / Calls

    /// <summary>
    /// Jump to new location
    /// </summary>    
    [Instruction(Mnemonic.JMP)]
    private void JMP(AddressingMode mode)
    {
        Registers.ProgramCounter = ReadAddress(mode);
    }

    #endregion

    #region System

    /// <summary>
    /// No Operation
    /// </summary>
    [Instruction(Mnemonic.NOP)]
    private void NOP(AddressingMode mode)
    {

    }

    /// <summary>
    /// STOP (1 -> PHI2)
    /// </summary>
    [Instruction(Mnemonic.STP)]
    private void STP(AddressingMode mode)
    {
        State = CpuState.Stopped;
    }

    #endregion

    #region Addresses

    private ushort ReadAddress(AddressingMode mode) => mode switch
    {
        AddressingMode.Absolute => ReadAbsoluteAddress(),

        AddressingMode.AbsoluteIndexedIndirect => ReadAbsoluteIndexedIndirectAddress(),

        AddressingMode.AbsoluteIndexedX => ReadAbsoluteXAddress(),
        AddressingMode.AbsoluteIndexedY => ReadAbsoluteYAddress(),

        AddressingMode.AbsoluteIndirect => ReadAbsoluteIndirectAddress(),

        AddressingMode.ProgramCounterRelative => ProgramCounterRelativeAddress(),

        AddressingMode.Stack => Stack.Pointer,

        AddressingMode.ZeroPage => ReadZeroPageAddress(),
        AddressingMode.ZeroPageIndexedX => ReadZeroPageXAddress(),
        AddressingMode.ZeroPageIndexedY => ReadZeroPageYAddress(),

        AddressingMode.ZeroPageIndirect => ReadZeroPageIndirectAddress(),
        AddressingMode.ZeroPageIndexedIndirect => ReadZeroPageIndexedIndirectAddress(),
        AddressingMode.ZeroPageIndirectIndexedY => ReadZeroPageIndirectIndexedYAddress(),

        AddressingMode.Immediate => Registers.ProgramCounter++,

        _ => throw new NotSupportedException()
    };

    private ushort ReadAbsoluteAddress()
    {
        var lo = ReadNext();
        var hi = ReadNext();

        return (ushort)(lo | hi << 8);
    }

    private ushort ReadAbsoluteIndexedIndirectAddress()
    {
        var lo = ReadNext();
        var hi = ReadNext();

        var address = ToAddress(lo, hi, Registers.X);

        var refLo = Memory[address];
        var refHi = Memory[(ushort)(address + 1)];

        return ToAddress(refLo, refHi);
    }

    private ushort ReadAbsoluteIndirectAddress()
    {
        var lo = ReadNext();
        var hi = ReadNext();

        var address = ToAddress(lo, hi);

        var refLo = Memory[address];
        var refHi = Memory[(ushort)(address + 1)];

        return ToAddress(refLo, refHi);
    }

    private ushort ReadAbsoluteXAddress()
    {
        var lo = ReadNext();
        var hi = ReadNext();

        return (ushort)((lo | hi << 8) + Registers.X);
    }

    private ushort ReadAbsoluteYAddress()
    {
        var lo = ReadNext();
        var hi = ReadNext();

        return (ushort)((lo | hi << 8) + Registers.Y);
    }

    private ushort ReadZeroPageAddress()
        => ReadNext();

    private ushort ReadZeroPageXAddress()
        => (byte)(ReadNext() + Registers.X);

    private ushort ReadZeroPageYAddress()
        => (byte)(ReadNext() + Registers.Y);

    private ushort ProgramCounterRelativeAddress()
    {
        var offset = (sbyte)ReadNext();
        return (ushort)(Registers.ProgramCounter + offset);
    }

    private ushort ReadZeroPageIndirectAddress()
    {
        var lo = ReadNext();
        var address = ToAddress(lo, 0x00);

        var refLo = Memory[address];
        var refHi = Memory[(ushort)(address + 1)];

        return ToAddress(refLo, refHi);
    }

    private ushort ReadZeroPageIndexedIndirectAddress()
    {
        var lo = ReadNext();
        var address = ToAddress((byte)(lo + Registers.X), 0x00);

        var refLo = Memory[address];
        var refHi = Memory[(ushort)(address + 1)];

        return ToAddress(refLo, refHi);
    }

    private ushort ReadZeroPageIndirectIndexedYAddress()
    {
        var zp = ReadNext();

        var lo = Memory[zp];
        var hi = Memory[(byte)(zp + 1)];

        var @base = ToAddress(lo, hi);

        return (ushort)(@base + Registers.Y);
    }

    #endregion

    private static ushort ToAddress(byte lo, byte hi)
        => (ushort)(lo | hi << 8);

    private static ushort ToAddress(byte lo, byte hi, byte offset)
        => (ushort)((lo | hi << 8) + offset);

    private byte ReadNext()
        => Memory[Registers.ProgramCounter++];

    private void RegisterHandlers()
    {
        var type = GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

        var instructions = methods
            .Select(m => (Method: m, Attribute: m.GetCustomAttribute<InstructionAttribute>()))
            .Where(t => t.Attribute is not null);

        foreach (var (method, info) in instructions)
            _handlers.Add(info!.Mnemonic, method.CreateDelegate<Action<AddressingMode>>(this));
    }
}
