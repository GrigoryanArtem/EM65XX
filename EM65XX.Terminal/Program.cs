using EM65XX.Core;

var mem = new Memory64K();

mem.Clear(0xEA);

mem.Load(0xFFFC, 0x00, 0x00);
mem.Load(0x0000, 0xA9, 0x2A, 0xA0, 0x2B, 0xA2, 0x9C);

var cpu = new Cpu65C02S(mem);
cpu.Reset();

Console.WriteLine("=== RESET STATE ===");
PrintState();

int ticks = 3;

for(int i = 0; i < ticks; i++)
{
    cpu.Tick();
    Console.WriteLine($"=== TICK #{i:000} ===");
    PrintState();
}

void PrintState()
{
    Console.WriteLine($"{cpu.ProgramCounter:X4} {cpu.OpCode:X2}");
    Console.WriteLine($"Status: {cpu.ProcessorStatus:b8}");
    Console.WriteLine($"Reg A: {cpu.RegisterA:X2}");
    Console.WriteLine($"Reg Y: {cpu.RegisterY:X2}");
    Console.WriteLine($"Reg X: {cpu.RegisterX:X2}");
    Console.WriteLine();
}