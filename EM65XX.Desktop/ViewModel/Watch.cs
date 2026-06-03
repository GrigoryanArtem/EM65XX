using CommunityToolkit.Mvvm.ComponentModel;
using EM65XX.Core.Abstraction;
using System.ComponentModel;
using System.Numerics;

namespace EM65XX.Desktop.ViewModel;

public partial class Watch(IMemory memory) : ObservableObject
{
    [ObservableProperty]
    private int _address;

    [ObservableProperty]
    private int _size = 1;

    [ObservableProperty]
    private string _value = string.Empty;

    [ObservableProperty]
    private bool signed = false;

    public void Update()
    {
        BigInteger bi = BigInteger.Zero;

        for (int i = Size - 1; i >= 0; i--)
        {
            bi <<= 8;
            bi |= memory.Read((ushort)(Address + i));
        }

        if (Signed && Size > 0)
        {
            int bits = Size * 8;

            if ((bi & (BigInteger.One << (bits - 1))) != 0)
                bi -= (BigInteger.One << bits);
            
        }

        Value = bi.ToString();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Value))
            Update();

        base.OnPropertyChanged(e);
    }
}