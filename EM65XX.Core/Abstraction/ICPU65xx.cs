namespace EM65XX.Core.Abstraction;

public interface ICPU65xx
{
    IRegisters Registers { get; }
    byte OpCode { get; }

    void Reset();
    void Tick();
}
