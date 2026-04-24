using EM65XX.Core.Enums;

namespace EM65XX.Core;

public readonly struct Instruction(Mnemonic mnemonic, AddressingMode mode)
{
    public readonly Mnemonic Mnemonic = mnemonic;
    public readonly AddressingMode Mode = mode;
}
