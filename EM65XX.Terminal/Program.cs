using EM65XX.Core;

var mem = new Memory64K();

mem.Clear(0xEA);

mem.Load(0xFFFC, 0x00, 0x80);
mem.Load(0x0000, 0xDD, 0x0D);
// mem.Load(0x8000, 0x38, 0xA9, 0x2A, 0x69, 0xF1, 0x1A);
mem.Load(0x8000, 0xA9, 0x02, 0x38, 0x6A);

var cpu = new Cpu65C02S(mem);
cpu.Reset();

Console.WriteLine("=== RESET STATE ===");
PrintState();

Console.WriteLine(ToDec(0x000, 2));

var iteration = 0;
while(true)
{
    Console.WriteLine($"=== TICK #{iteration:000} ===");    
    var instruction = InstructionsTable.Get(cpu.OpCode);
    Console.WriteLine($"{cpu.Registers.ProgramCounter:X4} {cpu.OpCode:X2} | {instruction.Mnemonic} ({instruction.Mode})");
    Console.WriteLine();

    cpu.Tick();

    PrintState();

    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
        break;
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