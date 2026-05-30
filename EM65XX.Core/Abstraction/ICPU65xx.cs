using EM65XX.Core.Enums;

namespace EM65XX.Core.Abstraction;

public interface ICPU65xx
{
    IRegisters Registers { get; }
    byte OpCode { get; }
    CpuState State { get; }

    void Reset();
    void Tick();
}
