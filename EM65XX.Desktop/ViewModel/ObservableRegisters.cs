using CommunityToolkit.Mvvm.ComponentModel;
using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;

namespace EM65XX.Desktop.ViewModel;

public partial class ObservableRegisters(IStack8 stack) : ObservableObject, IRegisters
{
    [ObservableProperty]
    private byte _a;
    [ObservableProperty]
    private byte _x;
    [ObservableProperty]
    private byte _y;

    [ObservableProperty]
    private ushort _programCounter;

    public byte StackPointer
    {
        get => stack.Pointer;
        set => SetProperty(stack.Pointer, value, stack, (s, v) => s.Pointer = v);
    }

    private Flags _statusFlags;
    public Flags StatusFlags
    {
        get => _statusFlags;
        set => SetProperty(ref _statusFlags, value | Flags.Unused);
    }

    public byte ProcessorStatus
    {
        get => (byte)StatusFlags;
        set => SetProperty(StatusFlags, (Flags)value, this, (s, v) => StatusFlags = v);
    }
}
