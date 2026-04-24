using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;

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

        _handlers.Add(Mnemonic.LDA, LDA);
        _handlers.Add(Mnemonic.LDX, LDX);
        _handlers.Add(Mnemonic.LDY, LDY);

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
