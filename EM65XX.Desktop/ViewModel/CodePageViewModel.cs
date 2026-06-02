using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EM65XX.Core;
using EM65XX.Core.Abstraction;
using System.Collections.ObjectModel;
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
    private string? _asmCode;

    [ObservableProperty]
    private byte _opCode;

    [ObservableProperty]
    private string? _mnemonic;

    [ObservableProperty]
    private string? _mode;

    [ObservableProperty]
    private string? _stdErr;

    [ObservableProperty]
    private bool _canRun;

    public ObservableCollection<Watch> Watches { get; } = new();
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
            Arguments = $"-x -Fbin -wdc02 -dotdir -pad=0xea {input} -o \"{output}\"",
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        process.WaitForExit();

        var err = process.StandardError.ReadToEnd();
        StdErr = String.IsNullOrWhiteSpace(err) ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}] Compilation completed" : err;

        if (process.ExitCode == 0)
        {
            using var stream = File.OpenRead(output);
            var memStream = new MemoryStream();

            stream.CopyTo(memStream);
            Ram.Load(0, memStream.ToArray());

            Reset();
        }
    }

    [RelayCommand]
    public void Reset()
    {
        _cpu.Reset();

        UpdateData();
    }

    [RelayCommand]
    public void Step()
    {
        _cpu.Tick();

        UpdateData();
    }

    [RelayCommand]
    public async Task Run()
    {            
        while(_cpu.State == Core.Enums.CpuState.Running)
        {
            for(int i = 0; i < 10; i++)
                _cpu.Tick();

            UpdateData();

            await Task.Delay(16);
        }
    }

    [RelayCommand]
    public void AddWatch()
    {
        var watch = new Watch(Ram);
        Watches.Add(watch);

        watch.Update();
    }

    private void UpdateData()
    {
        UpdateCpuInfo();

        foreach (var watch in Watches)
            watch.Update();
    }

    private void UpdateCpuInfo()
    {
        OpCode = _cpu.OpCode;
        var instruction = InstructionsTable.Get(OpCode);

        Mnemonic = instruction.Mnemonic.ToString();
        Mode = instruction.Mode.ToString();
    }
}
