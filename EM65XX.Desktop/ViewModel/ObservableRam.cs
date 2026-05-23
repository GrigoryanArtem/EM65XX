using CommunityToolkit.Mvvm.ComponentModel;
using EM65XX.Core.Abstraction;

namespace EM65XX.Desktop.ViewModel;

public class ObservableRam : ObservableObject, IMemory
{
    public class ObservableByte : ObservableObject
    {
        public byte Value { get; set; }
    }

    public class MemoryRow(int address, ObservableByte[] values) : ObservableObject
    {
        public int Address { get; } = address;
        public ObservableByte[] Values { get; } = values;

        public ObservableByte this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }
    }

    public class MemoryPage : ObservableObject
    {
        public MemoryPage(int address)
        {
            Address = address;

            Values = new ObservableByte[256];

            for (int i = 0; i < Values.Length; i++)
                Values[i] = new();

            Rows = new MemoryRow[16];

            for (int row = 0; row < 16; row++)
            {
                var rowValues = new ObservableByte[16];

                for (int col = 0; col < 16; col++)
                    rowValues[col] = Values[row * 16 + col];

                Rows[row] = new MemoryRow(address + row * 16, rowValues);
            }
        }


        public int Address { get; }

        public ObservableByte[] Values { get; }
        public MemoryRow[] Rows { get; set; }
    }

    public MemoryPage[] Pages { get; }

    public ObservableRam()
    {
        Pages = new MemoryPage[256];

        for(int i = 0; i < Pages.Length; i++)
            Pages[i] = new MemoryPage(i * 256);
    }


    public byte this[ushort address] 
    {
        get => Read(address); 
        set => Write(address, value);
    }

    public void Clear(byte value)
    {
        throw new NotImplementedException();
    }

    public void Dispose() { }

    public void Load(ushort offset, IEnumerable<byte> data)
    {
        throw new NotImplementedException();
    }

    public byte Read(ushort address)
    {
        throw new NotImplementedException();
    }

    public byte Read(byte page, byte address)
    {
        throw new NotImplementedException();
    }

    public void Write(ushort address, byte value)
    {
        throw new NotImplementedException();
    }

    public void Write(byte page, byte address, byte value)
    {
        throw new NotImplementedException();
    }
}
