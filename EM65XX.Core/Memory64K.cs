using EM65XX.Core.Abstraction;

namespace EM65XX.Core;

public class Memory64K : IMemory
{
    private const int SIZE = 1 << 16;
    private readonly byte[] _data;

    public Memory64K()
        => _data = new byte[SIZE];

    public Memory64K(byte[] data)
    {
        if(data.Length != SIZE)
            throw new ArgumentOutOfRangeException(nameof(data), $"Data must be exactly {SIZE} bytes long.");

        _data = data;
    }        

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
        => _data[(page << 8) | address];

    public void Write(ushort address, byte value)
        => _data[address] = value;

    public void Write(byte page, byte address, byte value)
        => _data[(page << 8) | address] = value;

    public void Load(ushort offset, params byte[] data)
        => Array.Copy(data, 0, _data, offset, data.Length);

    public void Load(ushort offset, IEnumerable<byte> data)
    {
        foreach (var b in data)
            _data[offset++] = b;
    }
}
