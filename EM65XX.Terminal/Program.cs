using EM65XX.Core;

var mem = new Memory64K();

mem.Clear(0xEA);

mem.Load(0xFFFC, 0x00, 0x80);
mem.Load(0x8000, 0x38, 0xA9, 0x2A, 0x69, 0xF1);

var cpu = new Cpu65C02S(mem);
cpu.Reset();

Console.WriteLine("=== RESET STATE ===");
PrintState();

var iteration = 0;
while(true)
{
    cpu.Tick();

    Console.WriteLine($"=== TICK #{iteration:000} ===");
    Console.WriteLine($"{cpu.Registers.ProgramCounter:X4} {cpu.OpCode:X2}");
    Console.WriteLine();

    PrintState();

    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
        break;
}

void PrintState()
{
    var registers = cpu.Registers;

    Console.WriteLine($"Status: {registers.ProcessorStatus:b8}");
    Console.WriteLine($"Reg A: {registers.A:X2}");
    Console.WriteLine($"Reg Y: {registers.Y:X2}");
    Console.WriteLine($"Reg X: {registers.X:X2}");
    Console.WriteLine();
}