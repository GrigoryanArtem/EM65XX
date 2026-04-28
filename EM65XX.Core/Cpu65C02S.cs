using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;
using EM65XX.Core.Extensions;

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

        _handlers.Add(Mnemonic.NOP, NOP);

        // Arithmetic
        _handlers.Add(Mnemonic.ADC, ADC);
        _handlers.Add(Mnemonic.SBC, SBC);


        // Loads
        _handlers.Add(Mnemonic.LDA, LDA);
        _handlers.Add(Mnemonic.LDX, LDX);
        _handlers.Add(Mnemonic.LDY, LDY);

        // Stores
        _handlers.Add(Mnemonic.STA, STA);
        _handlers.Add(Mnemonic.STX, STX);
        _handlers.Add(Mnemonic.STY, STY);
        _handlers.Add(Mnemonic.STZ, STZ);


        // Flags
        _handlers.Add(Mnemonic.CLC, CLC);
        _handlers.Add(Mnemonic.SEC, SEC);
        _handlers.Add(Mnemonic.CLI, CLI);
        _handlers.Add(Mnemonic.SEI, SEI);
        _handlers.Add(Mnemonic.CLV, CLV);
        _handlers.Add(Mnemonic.CLD, CLD);
        _handlers.Add(Mnemonic.SED, SED);
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

    private void SBC(AddressingMode mode)
    {
        throw new NotImplementedException("SBC is not implemented yet");
    }

    #endregion

    #region Load    

    /// <summary>
    /// M -> A
    /// </summary>
    private void LDA(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.A = Memory[address];
        Registers.UpdateNZFlags(Registers.A);
    }

    /// <summary>
    /// M -> X
    /// </summary>
    private void LDX(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Registers.X = Memory[address];
        Registers.UpdateNZFlags(Registers.X);
    }

    /// <summary>
    /// M -> Y
    /// </summary>
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
    private void STA(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = Registers.A;
    }

    /// <summary>
    /// X -> M
    /// </summary>    
    private void STX(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = Registers.X;
    }

    /// <summary>
    /// Y -> M
    /// </summary>    
    private void STY(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = Registers.Y;
    }

    /// <summary>
    /// 00 -> M
    /// </summary>    
    private void STZ(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        Memory[address] = 0x00;
    }

    #endregion

    #region Flags

    /// <summary>
    /// 0 -> C
    /// </summary>
    private void CLC(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Carry, false);
    }

    /// <summary>
    /// 1 -> C
    /// </summary>
    private void SEC(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Carry, true);
    }

    /// <summary>
    /// 0 -> I
    /// </summary>
    private void CLI(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Interrupt, false);
    }

    /// <summary>
    /// 1 -> I
    /// </summary>
    private void SEI(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Interrupt, true);
    }

    /// <summary>
    /// 0 -> V
    /// </summary>
    private void CLV(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Overflow, false);
    }

    /// <summary>
    /// 0 -> V
    /// </summary>
    private void CLD(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Decimal, false);
    }

    /// <summary>
    /// 1 -> V
    /// </summary>
    private void SED(AddressingMode mode)
    {
        Registers.UpdateFlags(Flags.Decimal, true);
    }

    #endregion

    /// <summary>
    /// No Operation
    /// </summary>
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
}
