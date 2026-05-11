using EM65XX.Core;
using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;
using EM65XX.Terminal.Parsers;

namespace EM65XX.Terminal;

public class CpuRunner(
    IMemoryParser parser,
    bool stepMode)
{
    public void Run(string filename)
    {
        using var mem = Ram.Create64K();
        parser.Parse(filename, mem);

        var cpu = new Cpu65C02S(mem);
        cpu.Reset();
                
        var close = false;
        while (cpu.State != CpuState.Stopped && !close)
        {
            var instruction = InstructionsTable.Get(cpu.OpCode);
            Console.WriteLine($"{cpu.Registers.ProgramCounter:X4} {cpu.OpCode:X2} | {instruction.Mnemonic} ({instruction.Mode})");

            cpu.Tick();

            while (stepMode)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
                {
                    close = true;
                    break;
                }

                if (key.Key == ConsoleKey.I)
                {
                    Console.WriteLine();
                    PrintState(cpu.Registers);
                    continue;
                }

                break;
            }
        }

        Console.WriteLine();
        PrintState(cpu.Registers);
    }

    private static void PrintState(Registers registers)
    {        
        Console.WriteLine($"S: {registers.StackPointer:b8}");
        Console.WriteLine($"P: {registers.ProcessorStatus:b8}");
        Console.WriteLine($"A: {registers.A:X2}/{registers.A}");
        Console.WriteLine($"Y: {registers.Y:X2}/{registers.Y}");
        Console.WriteLine($"X: {registers.X:X2}/{registers.X}");        
    }

    private static long ToDec(IMemory mem, ushort address, int count)
    {
        var dec = 0L;

        for (int i = 0; i < count; i++)
            dec |= (long)mem.Read((ushort)(address + i)) << (i * 8);

        return dec;
    }
}
