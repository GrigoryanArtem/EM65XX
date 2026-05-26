using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EM65XX.Core;
using EM65XX.Core.Abstraction;
using System.Diagnostics;
using System.IO;
using static EM65XX.Desktop.ViewModel.ObservableRam;

namespace EM65XX.Desktop.ViewModel;

public partial class CodePageViewModel : ObservableObject
{
    private readonly ICPU65xx _cpu;

    public CodePageViewModel()
    {
        Ram = new();
        _cpu = new Cpu65C02S(Ram, s => new ObservableRegisters(s));

        Ram.Clear(0xEA);
        _cpu.Reset();
    }

    public ObservableRam Ram { get; }
    public IRegisters Registers => _cpu.Registers;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedPage))]
    private int _selectedPageIndex;

    [ObservableProperty]
    public string _asmCode;

    public MemoryPage SelectedPage => Ram.Pages[SelectedPageIndex];
    public MemoryPage StackPage => Ram.Pages[1];

    [RelayCommand]
    public void NextPage()
    {
        SelectedPageIndex = Math.Clamp(SelectedPageIndex + 1, 0, 255);
    }

    [RelayCommand]
    public void PrevPage()
    {
        SelectedPageIndex = Math.Clamp(SelectedPageIndex - 1, 0, 255);
    }

    [RelayCommand]
    public void ToZeroPage()
    {
        SelectedPageIndex = 0;
    }

    [RelayCommand]
    public void ToLastPage()
    {
        SelectedPageIndex = 255;
    }

    [RelayCommand]
    public void Load()
    {
        var input = Path.GetTempFileName();
        var output = Path.GetTempFileName();

        File.WriteAllText(input, AsmCode);


        var psi = new ProcessStartInfo
        {
            FileName = "vasm6502_oldstyle.exe",
            Arguments = $"-Fbin -wdc02 -dotdir -pad=0xea {input} -o \"{output}\"",
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        process.WaitForExit();

        using var stream = File.OpenRead(output);
        var memStream = new MemoryStream();

        stream.CopyTo(memStream);

        Ram.Load(0, memStream.ToArray());        
    }

    [RelayCommand]
    public void Reset()
    {
        _cpu.Reset();
    }

    [RelayCommand]
    public void Step()
    {
        _cpu.Tick();
    }
}
