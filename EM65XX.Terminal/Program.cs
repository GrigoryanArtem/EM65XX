using EM65XX.Core;
using EM65XX.Core.Enums;
using EM65XX.Terminal.Parsers;
using System.Diagnostics;

var program = @"programs/add_decimal";

var parser = new ShortFormatParser();
var mem = parser.Parse(program);

var cpu = new Cpu65C02S(mem);
cpu.Reset();

Console.WriteLine();
Console.WriteLine("=== RESET STATE ===");
PrintState();

var iteration = 0;
var close = false;

while(cpu.State != CpuState.Stopped && !close)
{    
    var instruction = InstructionsTable.Get(cpu.OpCode);
    Console.WriteLine($"#{iteration++:000} | {cpu.Registers.ProgramCounter:X4} {cpu.OpCode:X2} | {instruction.Mnemonic} ({instruction.Mode})");
    
    cpu.Tick();
    
    while (true) 
    {
        var key = Console.ReadKey();
        if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
        {
            close = true;
            break;
        }

        if(key.Key == ConsoleKey.I)
        {
            Console.WriteLine();
            PrintState();
            continue;
        }

        break;
    }
}

Console.WriteLine();
Console.WriteLine($"{ToDec(0x000, 4)} + {ToDec(0x004, 4)} = {ToDec(0x0010, 4)}");

void PrintState()
{
    var registers = cpu.Registers;

    Console.WriteLine($"S: {registers.StackPointer:b8}");
    Console.WriteLine($"P: {registers.ProcessorStatus:b8}");
    Console.WriteLine($"A: {registers.A:X2}/{registers.A}");
    Console.WriteLine($"Y: {registers.Y:X2}/{registers.Y}");
    Console.WriteLine($"X: {registers.X:X2}/{registers.X}");
    Console.WriteLine();
}

long ToDec(ushort address, int count)
{
    var dec = 0L;
    
    for(int i = 0; i < count; i++)    
        dec |= (long)mem.Read((ushort)(address + i)) << (i * 8);

    return dec;
}