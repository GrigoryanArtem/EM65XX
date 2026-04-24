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
        _stack = new(memory, STACK_PAGE);
        Memory = memory;

        _handlers.Add(Mnemonic.NOP, NOP);
        _handlers.Add(Mnemonic.LDA, LDA);
        _handlers.Add(Mnemonic.LDX, LDX);
        _handlers.Add(Mnemonic.LDY, LDY);
    }

    #region Registers

    /// <summary>
    /// Accumulator A
    /// </summary>
    public byte RegisterA { get; set; }

    /// <summary>
    /// Index Register X
    /// </summary>
    public byte RegisterX { get; set; }

    /// <summary>
    /// Index Register Y
    /// </summary>
    public byte RegisterY { get; set; }

    /// <summary>
    /// Processor Status Register P
    /// </summary>
    public byte ProcessorStatus
    {
        get => (byte)StatusFlags;
        set => StatusFlags = (Flags)value;
    }

    /// <summary>
    /// Program Counter PC
    /// </summary>
    public ushort ProgramCounter { get; set; }

    /// <summary>
    /// Stack Pointer S
    /// </summary>
    public byte StackPointer 
    {
        get => _stack.Pointer;
        set => _stack.Pointer = value;
    }

    #endregion

    public Flags StatusFlags { get; set; }

    public byte OpCode => Memory[ProgramCounter];

    public IMemory Memory { get; }

    public void Reset()
    {
        ProcessorStatus |= 0b00000100;
        ProcessorStatus &= 0b11110111;

        ProgramCounter = 0xFFFC;
        ProgramCounter = ReadAddress(AddressingMode.Absolute);
    }

    public void Tick()
    {
        var instruction = InstructionsTable.Get(OpCode);
        ProgramCounter++;

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

        RegisterA = Memory[address];

        UpdateNegativeFlag(RegisterA);
        UpdateZeroFlag(RegisterA);
    }

    /// <summary>
    /// M -> X
    /// </summary>
    private void LDX(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        RegisterX = Memory[address];

        UpdateNegativeFlag(RegisterX);
        UpdateZeroFlag(RegisterX);
    }

    /// <summary>
    /// M -> Y
    /// </summary>
    private void LDY(AddressingMode mode)
    {
        var address = ReadAddress(mode);

        RegisterY = Memory[address];

        UpdateNegativeFlag(RegisterY);
        UpdateZeroFlag(RegisterY);
    }

    #endregion

    /// <summary>
    /// No Operation
    /// </summary>
    private void NOP(AddressingMode mode) 
    {
              
    }

    #region Addresses


    private void UpdateNegativeFlag(byte value)
        => StatusFlags = StatusFlags.UpdateFlags(Flags.Negative, value > 0x80);

    private void UpdateZeroFlag(byte value)
        => StatusFlags = StatusFlags.UpdateFlags(Flags.Zero, value == 0);

    private ushort ReadAddress(AddressingMode mode) => mode switch 
    {
        AddressingMode.Absolute => ReadAbsoluteAddress(),
        AddressingMode.Immediate => ProgramCounter++,

        _ => throw new NotSupportedException() 
    };

    private ushort ReadAbsoluteAddress()
    {
        var lo = ReadNext();
        var hi = ReadNext();

        return (ushort)(lo | hi << 8);
    }

    #endregion

    private byte ReadNext()
        => Memory[ProgramCounter++];
}
