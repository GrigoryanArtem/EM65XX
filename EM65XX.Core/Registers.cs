using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;

namespace EM65XX.Core;

public class Registers(IStack8 stack) : IRegisters
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
    public Flags StatusFlags 
    {
        get; 
        set => field  = value | Flags.Unused; 
    }

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
}
