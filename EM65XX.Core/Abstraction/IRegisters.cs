using EM65XX.Core.Enums;

namespace EM65XX.Core.Abstraction;

public interface IRegisters
{
    byte A { get; set; }
    byte X { get; set; }
    byte Y { get; set; }

    byte ProcessorStatus { get; set; }
    Flags StatusFlags { get; set; }
    ushort ProgramCounter { get; set; }
    byte StackPointer { get; set; }
}
