using EM65XX.Core;

var mem = new Memory64K();

mem.Clear(0xEA);

mem.Load(0xFFFC, 0x00, 0x00);
mem.Load(0x0000, 0xA9, 0x00, 0x00);

var cpu = new Cpu65C02S(mem);
cpu.Reset();