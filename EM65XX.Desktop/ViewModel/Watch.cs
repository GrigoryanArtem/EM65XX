using CommunityToolkit.Mvvm.ComponentModel;
using EM65XX.Core.Abstraction;
using System.Numerics;

namespace EM65XX.Desktop.ViewModel;

public partial class Watch(IMemory memory) : ObservableObject
{
    [ObservableProperty]
    private int _address;

    [ObservableProperty]
    private int _size;

    [ObservableProperty]
    private string _value = string.Empty;

    public void Update()
    {
        BigInteger bi = BigInteger.Zero;

        for(int i = Size - 1; i >= 0; i--)
        {
            bi <<= 8;
            bi |= memory.Read((ushort)(Address + i));
        }

        Value = bi.ToString();
    }
}
