namespace EM65XX.Core.Abstraction;

public interface IStack8
{
    byte Pointer { get; set; }

    byte Pop();
    ushort PopWord();

    void PushWord(ushort word);
    void Push(byte value);

    void Reset();
}
