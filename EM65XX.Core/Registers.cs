using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;
using EM65XX.Core.Extensions;

namespace EM65XX.Core;

public class Registers(IStack stack)
{
    /// <summary>
    /// Accumulator <b>A</b>
    /// </summary>
    public byte A { get; set; }

    /// <summary>
    /// Index Register <b>X</b>
    /// </summary>
    public byte X { get; set; }

    /// <summary>
    /// Index Register <b>Y</b>
    /// </summary>
    public byte Y { get; set; }

    /// <summary>
    /// Processor Status Register <b>P</b>
    /// </summary>
    public byte ProcessorStatus
    {
        get => (byte)StatusFlags;
        set => StatusFlags = (Flags)value;
    }

    /// <summary>
    /// Status Flags of Register P
    /// </summary>
    public Flags StatusFlags { get; set; }

    /// <summary>
    /// Program Counter <b>PC</b>
    /// </summary>
    public ushort ProgramCounter { get; set; }

    /// <summary>
    /// Stack Pointer <b>S</b>
    /// </summary>
    public byte StackPointer
    {
        get => stack.Pointer;
        set => stack.Pointer = value;
    }

    public void UpdateFlags(Flags flags, bool value)
        => StatusFlags = StatusFlags.UpdateFlags(flags, value);

    public void UpdateNZFlags(byte value)
    {
        UpdateFlags(Flags.Negative, value > 0x80);
        UpdateFlags(Flags.Zero, value == 0);
    }
    
}
