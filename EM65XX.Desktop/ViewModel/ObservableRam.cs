using CommunityToolkit.Mvvm.ComponentModel;
using EM65XX.Core.Abstraction;

namespace EM65XX.Desktop.ViewModel;

public partial class ObservableRam : ObservableObject, IMemory
{
    public partial class ObservableByte : ObservableObject
    {
        [ObservableProperty]
        public byte _value;
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
        foreach(var page in Pages)        
            for (int i = 0; i < page.Values.Length; i++)
                page.Values[i].Value = value;        
    }

    public void Dispose() { }

    public void Load(ushort offset, IEnumerable<byte> data)
    {
        foreach (var (idx, value) in data.Index())
            Write((ushort)(offset + idx), value);
    }

    public byte Read(ushort address)
    {        
        return Read((byte)(address / 256), (byte)(address % 256));
    }

    public byte Read(byte page, byte address)
    {        
        return Pages[page].Values[address].Value;
    }

    public void Write(ushort address, byte value)
    {
        Write((byte)(address / 256), (byte)(address % 256), value);
    }

    public void Write(byte page, byte address, byte value)
    {
        Pages[page].Values[address].Value = value;
    }
}
