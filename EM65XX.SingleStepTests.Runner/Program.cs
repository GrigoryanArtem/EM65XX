using CommandLine;
using EM65XX.Core;
using EM65XX.Core.Abstraction;
using EM65XX.SingleStepTests.Runner.Abstractions;
using EM65XX.SingleStepTests.Runner.CommandLine;
using EM65XX.SingleStepTests.Runner.Containers;
using EM65XX.SingleStepTests.Runner.Model;
using EM65XX.SingleStepTests.Runner.Utility;
using EM65XX.SingleStepTests.Runner.Writers;

namespace EM65XX.SingleStepTests.Runner;

internal class Program
{        
    static void Main(string[] args)
        => Parser.Default.ParseArguments<Options>(args)
            .WithParsed(HandleOptions)
            .WithNotParsed(HandleParsingErrors);    

    private static void HandleOptions(Options options)
    {        
        var writerFactory = new WriterFactory(options.Output);
        var (cpu, mem) = ResetCpu(); 
        
        Dictionary<string, double> results = [];

        var total = 0;
        var passed = 0;

        ITestContainer container = new FileTests(options.Directory, options.InstructionPattern);
        foreach (var batch in container.GetTests())
        {
            var writer = writerFactory.CreateWriter(batch.Name);

            total += batch.Count;
            var localPassed = 0;

            foreach (var test in batch.Tests)
            {
                bool success = false;

                writer.WriteLine($"[INIT] {test.Name}");

                SetupCpu(mem, cpu, test.Initial);

                try
                {
                    cpu.Tick();
                    success = CompareStates(writer, test.Final, cpu);
                }
                catch (Exception ex)
                {
                    writer.WriteLine("\tException during execution:");
                    writer.WriteLine(ex.Message);


                    writer.WriteLine("\tRESET CPU");

                    mem.Dispose();
                    (cpu, mem) = ResetCpu();
                }

                var status = success ? "[ OK ]" : "[FAIL]";

                if (success)
                {
                    localPassed++;
                    passed++;
                }

                if (!success)
                {
                    PrintState(writer, test.Initial);
                }

                writer.WriteLine($"{status} {test.Name}");
                writer.WriteLine();
            }


            var localStatus = localPassed == batch.Count ? "[ OK ]" : $"[FAIL]";


            var localPercentage = (double)localPassed / batch.Count * 100.0;
            Console.WriteLine($"{localStatus} {batch.Name} {localPercentage,6:f2}%");

            writer.Flush();
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

        mem.Dispose();
    }

    private static void HandleParsingErrors(IEnumerable<Error> errors)
    {
        Console.Error.WriteLine("Failed to parse command line arguments.");
        Environment.Exit(1);
    }

    private static bool CompareStates(IWriter sb, State expected, ICPU65xx actual)
    {
        var success = true;

        if (expected.PC != actual.Registers.ProgramCounter)
        {
            sb.WriteLine($"\tPC mismatch: expected {expected.PC:X4}, actual {actual.Registers.ProgramCounter:X4}");
            success = false;
        }

        if (expected.S != actual.Registers.StackPointer)
        {
            sb.WriteLine($"\tS mismatch: expected {expected.S:X2}, actual {actual.Registers.StackPointer:X2}");
            success = false;
        }

        if (expected.A != actual.Registers.A)
        {
            sb.WriteLine($"\tA mismatch: expected {expected.A:X2}, actual {actual.Registers.A:X2}");
            success = false;
        }

        if (expected.X != actual.Registers.X)
        {
            sb.WriteLine($"\tX mismatch: expected {expected.X:X2}, actual {actual.Registers.X:X2}");
            success = false;
        }

        if (expected.Y != actual.Registers.Y)
        {
            sb.WriteLine($"\tY mismatch: expected {expected.Y:X2}, actual {actual.Registers.Y:X2}");
            success = false;
        }

        if (expected.P != actual.Registers.ProcessorStatus)
        {
            sb.WriteLine($"\tP mismatch: expected {expected.P:X2}/{expected.P:B8}, actual {actual.Registers.ProcessorStatus:X2}/{actual.Registers.ProcessorStatus:B8}");
            success = false;
        }

        return success;
    }

    private static void PrintState(IWriter writer, State state)
    {
        writer.WriteLine();
        writer.WriteLine($"PC {state.PC:X4}");

        writer.WriteLine($"S  {state.S:X2} {state.S:B8}");
        writer.WriteLine($"A  {state.A:X2} {state.A:B8}");
        writer.WriteLine($"X  {state.X:X2} {state.X:B8}");
        writer.WriteLine($"Y  {state.Y:X2} {state.Y:B8}");
        writer.WriteLine($"P  {state.P:X2} {state.P:B8}");

        writer.WriteLine();

        foreach (var ramEntry in state.Ram)
        {
            var address = (ushort)ramEntry[0];
            var value = (byte)ramEntry[1];
            writer.WriteLine($"{address:X4} {value:X2}");
        }

        writer.WriteLine();
    }

    private static void SetupCpu(IMemory mem, ICPU65xx cpu, State state)
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

    private static (ICPU65xx cpu, IMemory mem) ResetCpu()
    {        
        var mem = Ram.Create64K();
        var cpu = new Cpu65C02S(mem);

        return (cpu, mem);
    }
}
    