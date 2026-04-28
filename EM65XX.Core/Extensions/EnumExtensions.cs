using EM65XX.Core.Enums;

namespace EM65XX.Core.Extensions;

public static class EnumExtensions
{
    extension(Flags source)
    {
        public Flags UpdateFlags(Flags flags, bool value)
            => value ? source | flags : source & ~flags;        

        public byte FlagToByte(Flags flag)
            => (byte)(source.HasFlag(flag) ? 1 : 0);
    }
}
