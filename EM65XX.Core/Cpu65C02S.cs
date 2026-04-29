using EM65XX.Core.Abstraction;
using EM65XX.Core.Attributes;
using EM65XX.Core.Enums;
using EM65XX.Core.Extensions;
using System.Reflection;

namespace EM65XX.Core;

public partial class Cpu65C02S : ICentralProcessingUnit
{
    private const byte STACK_PAGE = 1;

    private readonly PageStack _stack;
    private readonly Dictionary<Mnemonic, Action<AddressingMode>> _handlers = [];

    public Cpu65C02S(IMemory memory)
    {
        Memory = memory;

        _stack = new(memory, STACK_PAGE);
        Registers = new(_stack);

        RegisterHandlers();
    }

    public Registers Registers { get; }

    public byte OpCode => Memory[Registers.ProgramCounter];

    public IMemory Memory { get; }

    public void Reset()
    {
        Registers.UpdateFlags(Flags.Break | Flags.Interrupt, true);
        Registers.UpdateFlags(Flags.Decimal, false);

        Registers.ProgramCounter = 0xFFFC;
        Registers.ProgramCounter = ReadAddress(AddressingMode.Absolute);
    }

    public void Tick()
    {
        var instruction = InstructionsTable.Get(OpCode);
        Registers.ProgramCounter++;

        var handler = _handlers[instruction.Mnemonic];
        handler(instruction.Mode);
    }

    #region Arithmetic

    /// <summary>
    ///  A + M + C -> A
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

    [Instruction(Mnemonic.SBC)]
    private void SBC(AddressingMode mode)
    {
        throw new NotImplementedException("SBC is not implemented yet");
    }

    #endregion

    #region Shifts / Rotates

    /// <summary>
    /// C <- 7 6 5 4 3 2 1 0 <- 0
    /// </summary>    
    [Instruction(Mnemonic.ASL)]
    private void ASL(AddressingMode mode)
    {        
        if(mode == AddressingMode.Accumulator)
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

    //TAX,
    //TAY,
    //TXA,
    //TYA,
    //TSX,
    //TXS,

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

    /// <summary>
    /// No Operation
    /// </summary>
    [Instruction(Mnemonic.NOP)]
    private void NOP(AddressingMode mode)
    {

    }

    #region Addresses

    private ushort ReadAddress(AddressingMode mode) => mode switch
    {
        AddressingMode.Absolute => ReadAbsoluteAddress(),

        AddressingMode.AbsoluteIndexedX => ReadAbsoluteXAddress(),
        AddressingMode.AbsoluteIndexedY => ReadAbsoluteYAddress(),

        AddressingMode.ZeroPage => ReadZeroPageAddress(),
        AddressingMode.ZeroPageIndexedX => ReadZeroPageXAddress(),
        AddressingMode.ZeroPageIndexedY => ReadZeroPageYAddress(),

        AddressingMode.Immediate => Registers.ProgramCounter++,        

        _ => throw new NotSupportedException()
    };

    private ushort ReadAbsoluteAddress()
    {
        var lo = ReadNext();
        var hi = ReadNext();

        return (ushort)(lo | hi << 8);
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

    #endregion

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
