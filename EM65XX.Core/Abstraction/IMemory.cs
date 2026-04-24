namespace EM65XX.Core.Abstraction;

public interface IMemory
{
    byte this[ushort address] { get; set; }
    void Clear(byte value);

    byte Read(ushort address);
    byte Read(byte page, byte address);
    byte Write(ushort address, byte value);
    byte Write(byte page, byte address, byte value);
}
