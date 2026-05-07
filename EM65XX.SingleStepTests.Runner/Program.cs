using EM65XX.Core;
using EM65XX.Core.Abstraction;
using EM65XX.SingleStepTests.Runner.Model;
using EM65XX.SingleStepTests.Runner.Table;
using System.Text;
using System.Text.Json;

namespace EM65XX.SingleStepTests.Runner;

internal class Program
{
    private static readonly char[] HEX = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'];

    static void Main(string[] args)
    {
        var files = Directory.GetFiles(@"", "*.json");
        Dictionary<string, double> results = [];

        var dirName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ffff");

        Directory.CreateDirectory(dirName);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var mem = new Memory64K();
        var cpu = new Cpu65C02S(mem);

        var total = 0;
        var passed = 0;

        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);

            StringBuilder sb = new StringBuilder();
            TestData[] tests = [];

            try
            {
                var json = File.ReadAllText(file);
                tests = JsonSerializer.Deserialize<TestData[]>(json, options);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to read or parse {file}: {ex.Message}");
                continue;
            }

            total += tests.Length;
            var localPassed = 0;

            foreach (var test in tests)
            {
                bool success = false;

                sb.AppendLine($"[INIT] {test.Name}");

                SetupCpu(mem, cpu, test.Initial);

                try
                {
                    cpu.Tick();
                    success = CompareStates(sb, test.Final, cpu);
                }
                catch (Exception ex)
                {
                    sb.AppendLine("\tException during execution:");
                    sb.AppendLine(ex.Message);


                    sb.AppendLine("\tRESET CPU");

                    mem = new Memory64K();
                    cpu = new Cpu65C02S(mem);
                }

                var status = success ? "[ OK ]" : "[FAIL]";

                if (success)
                {
                    localPassed++;
                    passed++;
                }

                if (!success)
                {
                    PrintState(sb, test.Initial);
                }

                sb.AppendLine($"{status} {test.Name}");
                sb.AppendLine();
            }


            var localStatus = localPassed == tests.Length ? "[ OK ]" : $"[FAIL]";
            

            var localPercentage = (double)localPassed/ tests.Length * 100.0;
            Console.WriteLine($"{localStatus} {name} {localPercentage,6:f2}%");

            File.WriteAllText(Path.Combine(dirName, $"{name}"), sb.ToString());

            results.Add(name, localPercentage);
        }

        var percentage = (double)passed / total * 100.0;
        Console.WriteLine();
        Console.WriteLine($"{passed}/{total} ({percentage:f2}%) tests passed.");



        var tb = TableBuilder.Create(TableOptions.Header);
        tb.SetVSeparator(" ");
        tb.AddColumn(new() { Header = "", Align = Align.Left, Width = 2 });
        foreach (var symbol in HEX)
            tb.AddColumn(new() { Header = symbol.ToString().ToUpperInvariant(), Align = Align.Right, Width = 5 });        

        var buffer = new object?[HEX.Length + 1];
        var nameBuffer = new object?[HEX.Length + 1];
        foreach (var f in HEX)
        {

            nameBuffer[0] = f.ToString().ToUpperInvariant();

            foreach (var (i, s) in HEX.Index())
            {
                var code = new string([f, s]);

                if(results.TryGetValue(code, out var value) && value < 100.0)
                {
                    var instr = InstructionsTable.Get(Convert.ToByte(code, 16));

                    nameBuffer[i + 1] = instr.Mnemonic.ToString();
                    buffer[i + 1] = value.ToString("f1");
                }
                else
                {
                    nameBuffer[i + 1] = null!;
                    buffer[i + 1] = null!;
                }                 
            }

            tb.AddRow(nameBuffer);
            tb.AddRow(buffer);
        }

        Console.WriteLine(tb.Build());
    }

    private static bool CompareStates(StringBuilder sb, State expected, Cpu65C02S actual)
    {
        if (expected.PC != actual.Registers.ProgramCounter)
        {
           sb.AppendLine($"\tPC mismatch: expected {expected.PC:X4}, actual {actual.Registers.ProgramCounter:X4}");
            return false;

        }

        if (expected.S != actual.Registers.StackPointer)
        {
           sb.AppendLine($"\tS mismatch: expected {expected.S:X2}, actual {actual.Registers.StackPointer:X2}");
            return false;
        }

        if (expected.A != actual.Registers.A)
        {
           sb.AppendLine($"\tA mismatch: expected {expected.A:X2}, actual {actual.Registers.A:X2}");
            return false;
        }

        if (expected.X != actual.Registers.X)
        {
           sb.AppendLine($"\tX mismatch: expected {expected.X:X2}, actual {actual.Registers.X:X2}");
            return false;
        }

        if (expected.Y != actual.Registers.Y)
        {
           sb.AppendLine($"\tY mismatch: expected {expected.Y:X2}, actual {actual.Registers.Y:X2}");
            return false;
        }

        if (expected.P != actual.Registers.ProcessorStatus)
        {
           sb.AppendLine($"\tP mismatch: expected {expected.P:X2}, actual {actual.Registers.ProcessorStatus:X2}");
            return false;
        }

        return true;
    }

    private static void PrintState(StringBuilder sb, State state)
    {
        sb.AppendLine();
        sb.AppendLine($"PC {state.PC:X4}");
        sb.AppendLine($"S  {state.S:X2}");
        sb.AppendLine($"A  {state.A:X2}");
        sb.AppendLine($"X  {state.X:X2}");
        sb.AppendLine($"Y  {state.Y:X2}");
        sb.AppendLine($"P  {state.P:X2}");

        sb.AppendLine();

        foreach (var ramEntry in state.Ram)
        {
            var address = (ushort)ramEntry[0];
            var value = (byte)ramEntry[1];
            sb.AppendLine($"{address:X4} {value:X2}");
        }
        sb.AppendLine();
    }

    private static void SetupCpu(IMemory mem, Cpu65C02S cpu, State state)
    {       
        for (var i = 0; i < state.Ram.Length; i++)
        {
            var address = (ushort)state.Ram[i][0];
            var value = (byte)state.Ram[i][1];

            mem[address] = value;
        }        

        cpu.Registers.ProgramCounter = (ushort)state.PC;
        cpu.Registers.StackPointer = (byte)state.S;
        cpu.Registers.A = (byte)state.A;
        cpu.Registers.X = (byte)state.X;
        cpu.Registers.Y = (byte)state.Y;
        cpu.Registers.ProcessorStatus = (byte)state.P;
    }
}
    