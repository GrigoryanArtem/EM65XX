namespace EM65XX.Core.Abstraction;

public interface IStack
{
    byte Pointer { get; set; }

    byte Pop();
    void Push(byte value);
    void Reset();
}
