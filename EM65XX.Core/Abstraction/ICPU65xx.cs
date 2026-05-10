namespace EM65XX.Core.Abstraction;

public interface ICPU65xx
{
    Registers Registers { get; }

    void Reset();
    void Tick();
}
