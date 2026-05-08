using CommandLine;
using EM65XX.Core;
using EM65XX.Core.Abstraction;
using EM65XX.SingleStepTests.Runner.Abstractions;
using EM65XX.SingleStepTests.Runner.CommandLine;
using EM65XX.SingleStepTests.Runner.Containers;
using EM65XX.SingleStepTests.Runner.Model;
using EM65XX.SingleStepTests.Runner.Utility;
using System.Text;

namespace EM65XX.SingleStepTests.Runner;

internal class Program
{    
    private const string DT_FORMAT = "yyyy-MM-dd_HH-mm-ss-ffff";

    static void Main(string[] args)
        => Parser.Default.ParseArguments<Options>(args)
            .WithParsed(HandleOptions)
            .WithNotParsed(HandleParsingErrors);    

    private static void HandleOptions(Options options)
    {        
        Dictionary<string, double> results = [];

        string? outputDir = null; 
        if (options.Output is not null)
        {
            outputDir = Path.Combine(options.Output, DateTime.Now.ToString(DT_FORMAT));
            Directory.CreateDirectory(outputDir);
        }

        var mem = new Memory64K();
        var cpu = new Cpu65C02S(mem);

        var total = 0;
        var passed = 0;

        ITestContainer container = new FileTests(options.Directory, options.InstructionPattern);
        foreach (var batch in container.GetTests())
        {
            StringBuilder sb = new StringBuilder();

            total += batch.Count;
            var localPassed = 0;

            foreach (var test in batch.Tests)
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


            var localStatus = localPassed == batch.Count ? "[ OK ]" : $"[FAIL]";


            var localPercentage = (double)localPassed / batch.Count * 100.0;
            Console.WriteLine($"{localStatus} {batch.Name} {localPercentage,6:f2}%");

            if (outputDir is not null)
            {
                File.WriteAllText(Path.Combine(outputDir, $"{batch.Name}"), sb.ToString());
            }

            results.Add(batch.Name, localPercentage);
        }

        var percentage = (double)passed / total * 100.0;
        Console.WriteLine();
        Console.WriteLine($"{passed}/{total} ({percentage:f2}%) tests passed.");

        if (options.Table)
        {
            Console.WriteLine();
            Console.WriteLine(TestResultTable.CreateTable(results));
        }
    }

    private static void HandleParsingErrors(IEnumerable<Error> errors)
    {
        Console.Error.WriteLine("Failed to parse command line arguments.");
        Environment.Exit(1);
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
            sb.AppendLine($"\tP mismatch: expected {expected.P:X2}/{expected.P:B8}, actual {actual.Registers.ProcessorStatus:X2}/{actual.Registers.ProcessorStatus:B8}");
            return false;
        }

        return true;
    }

    private static void PrintState(StringBuilder sb, State state)
    {
        sb.AppendLine();
        sb.AppendLine($"PC {state.PC:X4}");

        sb.AppendLine($"S  {state.S:X2} {state.S:B8}");
        sb.AppendLine($"A  {state.A:X2} {state.A:B8}");
        sb.AppendLine($"X  {state.X:X2} {state.X:B8}");
        sb.AppendLine($"Y  {state.Y:X2} {state.Y:B8}");
        sb.AppendLine($"P  {state.P:X2} {state.P:B8}");

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
    