using EM65XX.Core.Abstraction;
using EM65XX.Core.Enums;

namespace EM65XX.Core.Extensions;

public static class RegistersExtensions
{
    extension(IRegisters registers)
    {
        public void UpdateFlags(Flags flags, byte value)
            => registers.StatusFlags = registers.StatusFlags.UpdateFlags(flags, value > 0);

        public void UpdateFlags(Flags flags, bool value)
            => registers.StatusFlags = registers.StatusFlags.UpdateFlags(flags, value);

        public void UpdateNZFlags(byte value)
        {
            registers.UpdateFlags(Flags.Negative, value >= 0x80);
            registers.UpdateFlags(Flags.Zero, value == 0);
        }
    }
}
