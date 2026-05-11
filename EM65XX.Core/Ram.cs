using EM65XX.Core.Abstraction;
using System.Buffers;

namespace EM65XX.Core;

public class Ram : IMemory
{    
    private bool _disposed = false;
    private readonly byte[] _data;

    public Ram(int size)
    {
        _data = ArrayPool<byte>.Shared.Rent(size);
        Array.Clear(_data);
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

    public static Ram Create64K()
        => new(1 << 16);

    public static Ram Create32K()
        => new(1 << 8);

    public void Dispose()
    {
        if (_disposed)
            return;

        ArrayPool<byte>.Shared.Return(_data);
        _disposed = true;
    }
}
