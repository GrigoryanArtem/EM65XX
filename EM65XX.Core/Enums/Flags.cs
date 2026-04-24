namespace EM65XX.Core.Enums;

[Flags]
public enum Flags
{
    Carry     = 0x01,
    Zero      = 0x02,
    Interrupt = 0x04,
    Decimal   = 0x08,
    Break     = 0x10,
    Overflow  = 0x40,
    Negative  = 0x80
}
