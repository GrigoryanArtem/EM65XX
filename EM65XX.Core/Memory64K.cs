using EM65XX.Core.Abstraction;

namespace EM65XX.Core;

public class Memory64K : IMemory
{
    private readonly byte[] _data = new byte[1 << 16];

    public void Clear(byte value)
        => Array.Fill(_data, value);

    public byte this[ushort address]
    {
        get => Read(address);
        set => Write(address, value);
    }

    public byte Read(ushort address)
        => _data[address];

    public byte Read(byte page, byte address)
        => _data[page * 0xFF + address];

    public byte Write(ushort address, byte value)
        => _data[address] = value;

    public byte Write(byte page, byte address, byte value)
        => _data[page * 0xFF + address] = value;

    public void Load(ushort offset, params byte[] data)
        => Array.Copy(data, 0, _data, offset, data.Length);    
}
