using EM65XX.Core;
using EM65XX.Terminal.Parsers;

var program = @"programs/test";

var parser = new ShortFormatParser();
var mem = parser.Parse(program);

var cpu = new Cpu65C02S(mem);
cpu.Reset();

Console.WriteLine("4618 + 3546");

Console.WriteLine();
Console.WriteLine("=== RESET STATE ===");
PrintState();

Console.WriteLine(ToDec(0x000, 2));

var iteration = 0;
var close = false;

while(!close)
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

void PrintState()
{
    var registers = cpu.Registers;

    Console.WriteLine($"Status: {registers.ProcessorStatus:b8}");
    Console.WriteLine($"Reg A: {registers.A:X2}/{registers.A}");
    Console.WriteLine($"Reg Y: {registers.Y:X2}/{registers.Y}");
    Console.WriteLine($"Reg X: {registers.X:X2}/{registers.X}");
    Console.WriteLine();
}

long ToDec(ushort address, int count)
{
    var dec = 0L;
    
    for(int i = 0; i < count; i++)    
        dec |= (long)mem.Read((ushort)(address + i)) << (i * 8);

    return dec;
}