using EM65XX.Core.Abstraction;

namespace EM65XX.Core;

public class PageStack(IMemory memory, byte page) : IStack8
{    
    public byte Pointer { get; set; }

    public void Reset()
        => Pointer = 0xFF;

    public void Push(byte value)
        => memory.Write(page, Pointer--, value);

    public void PushWord(ushort word)
    {        
        Push((byte)(word >> 8)); 
        Push((byte)(word & 0xFF));
    }

    public byte Pop()
        => memory.Read(page, ++Pointer);

    public ushort PopWord()
    {
        var lo = Pop();
        var hi = Pop();

        return (ushort)(lo | (hi << 8));
    }    
}
