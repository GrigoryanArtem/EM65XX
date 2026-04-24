using EM65XX.Core.Abstraction;

namespace EM65XX.Core;

public class PageStack(IMemory memory, byte page) : IStack
{    
    public byte Pointer { get; set; }

    public void Reset()
        => Pointer = 0xFF;

    public void Push(byte value)
        => memory.Write(page, Pointer--, value);

    public byte Pop()
        => memory.Read(page, Pointer++);
}
