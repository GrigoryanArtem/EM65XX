namespace EM65XX.Core.Abstraction;

public interface ICPU65xx
{
    IRegisters Registers { get; }

    void Reset();
    void Tick();
}
