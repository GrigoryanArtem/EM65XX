using EM65XX.Core.Enums;

namespace EM65XX.Core.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class InstructionAttribute(Mnemonic mnemonic) : Attribute
{
    public Mnemonic Mnemonic { get; } = mnemonic;
}
