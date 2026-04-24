using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;

namespace EM65XX.Core;

public partial class Cpu65C02S(IMemory memory) : ICentralProcessingUnit
{
    private const byte STACK_PAGE = 1;

    private readonly PageStack _stack = new(memory, STACK_PAGE);    

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
    public byte ProcessorStatus { get; set; }

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

    public Flags StatusFlags => (Flags)ProcessorStatus;

    public byte OpCode => Memory[ProgramCounter];

    public IMemory Memory { get; } = memory;

    public void Reset()
    {
        ProcessorStatus |= 0b00000100;
        ProcessorStatus &= 0b11110111;

        ProgramCounter = 0xFFFC;
        ProgramCounter = ReadAddress();
    }

    public void Tick()
    {
        switch (OpCode)
        {
            case 0xA9:
                LDA();
                break;

            case 0xEA:
                NOP();
                break;

            default:
                throw new NotImplementedException();
        }
            
    }

    /// <summary>
    /// LoaD Accumulator with memory
    /// </summary>
    private void LDA()
    {
        ProgramCounter++;
        RegisterA = Memory[ReadAddress()];
    }

    /// <summary>
    /// No OPeration
    /// </summary>
    private void NOP() 
    {
        ProgramCounter++;        
    }

    private ushort ReadAddress()
    {
        var lo = Memory[ProgramCounter++];
        var hi = Memory[ProgramCounter++];

        return (ushort)(lo | hi << 8);
    }        
}
