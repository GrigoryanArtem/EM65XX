using EM65XX.Core.Enums;

namespace EM65XX.Core;

public static class InstructionsTable
{
    private static readonly Instruction[] _table = new Instruction[0x100];

    static InstructionsTable()
    {
        Array.Fill(_table, new(Mnemonic.NOP, AddressingMode.Implied));

        // ADC
        _table[0x6D] = new(Mnemonic.ADC, AddressingMode.Absolute);
        _table[0x7D] = new(Mnemonic.ADC, AddressingMode.AbsoluteIndexedX);
        _table[0x79] = new(Mnemonic.ADC, AddressingMode.AbsoluteIndexedY);
        _table[0x69] = new(Mnemonic.ADC, AddressingMode.Immediate);
        _table[0x65] = new(Mnemonic.ADC, AddressingMode.ZeroPage);
        _table[0x61] = new(Mnemonic.ADC, AddressingMode.ZeroPageIndexedIndirect);
        _table[0x75] = new(Mnemonic.ADC, AddressingMode.ZeroPageIndexedX);
        _table[0x72] = new(Mnemonic.ADC, AddressingMode.ZeroPageIndirect);
        _table[0x71] = new(Mnemonic.ADC, AddressingMode.ZeroPageIndirectIndexedY);

        // AND
        _table[0x2D] = new(Mnemonic.AND, AddressingMode.Absolute);
        _table[0x3D] = new(Mnemonic.AND, AddressingMode.AbsoluteIndexedX);
        _table[0x39] = new(Mnemonic.AND, AddressingMode.AbsoluteIndexedY);
        _table[0x29] = new(Mnemonic.AND, AddressingMode.Immediate);
        _table[0x25] = new(Mnemonic.AND, AddressingMode.ZeroPage);
        _table[0x21] = new(Mnemonic.AND, AddressingMode.ZeroPageIndexedIndirect);
        _table[0x35] = new(Mnemonic.AND, AddressingMode.ZeroPageIndexedX);
        _table[0x32] = new(Mnemonic.AND, AddressingMode.ZeroPageIndirect);
        _table[0x31] = new(Mnemonic.AND, AddressingMode.ZeroPageIndirectIndexedY);

        // ASL
        _table[0x0E] = new(Mnemonic.ASL, AddressingMode.Absolute);
        _table[0x1E] = new(Mnemonic.ASL, AddressingMode.AbsoluteIndexedX);
        _table[0x0A] = new(Mnemonic.ASL, AddressingMode.Accumulator);
        _table[0x06] = new(Mnemonic.ASL, AddressingMode.ZeroPage);
        _table[0x16] = new(Mnemonic.ASL, AddressingMode.ZeroPageIndexedX);

        // BCC / BCS / BEQ / BMI / BNE / BPL / BVC / BVS / BRA
        _table[0x90] = new(Mnemonic.BCC, AddressingMode.ProgramCounterRelative);
        _table[0xB0] = new(Mnemonic.BCS, AddressingMode.ProgramCounterRelative);
        _table[0xF0] = new(Mnemonic.BEQ, AddressingMode.ProgramCounterRelative);
        _table[0x30] = new(Mnemonic.BMI, AddressingMode.ProgramCounterRelative);
        _table[0xD0] = new(Mnemonic.BNE, AddressingMode.ProgramCounterRelative);
        _table[0x10] = new(Mnemonic.BPL, AddressingMode.ProgramCounterRelative);
        _table[0x50] = new(Mnemonic.BVC, AddressingMode.ProgramCounterRelative);
        _table[0x70] = new(Mnemonic.BVS, AddressingMode.ProgramCounterRelative);
        _table[0x80] = new(Mnemonic.BRA, AddressingMode.ProgramCounterRelative);

        // BIT
        _table[0x2C] = new(Mnemonic.BIT, AddressingMode.Absolute);
        _table[0x3C] = new(Mnemonic.BIT, AddressingMode.AbsoluteIndexedX);
        _table[0x89] = new(Mnemonic.BIT, AddressingMode.Immediate);
        _table[0x24] = new(Mnemonic.BIT, AddressingMode.ZeroPage);
        _table[0x34] = new(Mnemonic.BIT, AddressingMode.ZeroPageIndexedX);

        // BRK
        _table[0x00] = new(Mnemonic.BRK, AddressingMode.Implied);

        // CMP
        _table[0xCD] = new(Mnemonic.CMP, AddressingMode.Absolute);
        _table[0xDD] = new(Mnemonic.CMP, AddressingMode.AbsoluteIndexedX);
        _table[0xD9] = new(Mnemonic.CMP, AddressingMode.AbsoluteIndexedY);
        _table[0xC9] = new(Mnemonic.CMP, AddressingMode.Immediate);
        _table[0xC5] = new(Mnemonic.CMP, AddressingMode.ZeroPage);
        _table[0xC1] = new(Mnemonic.CMP, AddressingMode.ZeroPageIndexedIndirect);
        _table[0xD5] = new(Mnemonic.CMP, AddressingMode.ZeroPageIndexedX);
        _table[0xD2] = new(Mnemonic.CMP, AddressingMode.ZeroPageIndirect);
        _table[0xD1] = new(Mnemonic.CMP, AddressingMode.ZeroPageIndirectIndexedY);

        // CPX
        _table[0xEC] = new(Mnemonic.CPX, AddressingMode.Absolute);
        _table[0xE0] = new(Mnemonic.CPX, AddressingMode.Immediate);
        _table[0xE4] = new(Mnemonic.CPX, AddressingMode.ZeroPage);

        // CPY
        _table[0xCC] = new(Mnemonic.CPY, AddressingMode.Absolute);
        _table[0xC0] = new(Mnemonic.CPY, AddressingMode.Immediate);
        _table[0xC4] = new(Mnemonic.CPY, AddressingMode.ZeroPage);

        // DEC
        _table[0xCE] = new(Mnemonic.DEC, AddressingMode.Absolute);
        _table[0xDE] = new(Mnemonic.DEC, AddressingMode.AbsoluteIndexedX);
        _table[0x3A] = new(Mnemonic.DEC, AddressingMode.Accumulator);
        _table[0xC6] = new(Mnemonic.DEC, AddressingMode.ZeroPage);
        _table[0xD6] = new(Mnemonic.DEC, AddressingMode.ZeroPageIndexedX);

        // DEX / DEY
        _table[0xCA] = new(Mnemonic.DEX, AddressingMode.Implied);
        _table[0x88] = new(Mnemonic.DEY, AddressingMode.Implied);

        // EOR
        _table[0x4D] = new(Mnemonic.EOR, AddressingMode.Absolute);
        _table[0x5D] = new(Mnemonic.EOR, AddressingMode.AbsoluteIndexedX);
        _table[0x59] = new(Mnemonic.EOR, AddressingMode.AbsoluteIndexedY);
        _table[0x49] = new(Mnemonic.EOR, AddressingMode.Immediate);
        _table[0x45] = new(Mnemonic.EOR, AddressingMode.ZeroPage);
        _table[0x41] = new(Mnemonic.EOR, AddressingMode.ZeroPageIndexedIndirect);
        _table[0x55] = new(Mnemonic.EOR, AddressingMode.ZeroPageIndexedX);
        _table[0x52] = new(Mnemonic.EOR, AddressingMode.ZeroPageIndirect);
        _table[0x51] = new(Mnemonic.EOR, AddressingMode.ZeroPageIndirectIndexedY);

        // INC
        _table[0xEE] = new(Mnemonic.INC, AddressingMode.Absolute);
        _table[0xFE] = new(Mnemonic.INC, AddressingMode.AbsoluteIndexedX);
        _table[0x1A] = new(Mnemonic.INC, AddressingMode.Accumulator);
        _table[0xE6] = new(Mnemonic.INC, AddressingMode.ZeroPage);
        _table[0xF6] = new(Mnemonic.INC, AddressingMode.ZeroPageIndexedX);

        // INX / INY
        _table[0xE8] = new(Mnemonic.INX, AddressingMode.Implied);
        _table[0xC8] = new(Mnemonic.INY, AddressingMode.Implied);

        // JMP
        _table[0x4C] = new(Mnemonic.JMP, AddressingMode.Absolute);
        _table[0x6C] = new(Mnemonic.JMP, AddressingMode.AbsoluteIndirect);
        _table[0x7C] = new(Mnemonic.JMP, AddressingMode.AbsoluteIndexedIndirect);

        // JSR
        _table[0x20] = new(Mnemonic.JSR, AddressingMode.Absolute);

        // LDA
        _table[0xAD] = new(Mnemonic.LDA, AddressingMode.Absolute);
        _table[0xBD] = new(Mnemonic.LDA, AddressingMode.AbsoluteIndexedX);
        _table[0xB9] = new(Mnemonic.LDA, AddressingMode.AbsoluteIndexedY);
        _table[0xA9] = new(Mnemonic.LDA, AddressingMode.Immediate);
        _table[0xA5] = new(Mnemonic.LDA, AddressingMode.ZeroPage);
        _table[0xA1] = new(Mnemonic.LDA, AddressingMode.ZeroPageIndexedIndirect);
        _table[0xB5] = new(Mnemonic.LDA, AddressingMode.ZeroPageIndexedX);
        _table[0xB2] = new(Mnemonic.LDA, AddressingMode.ZeroPageIndirect);
        _table[0xB1] = new(Mnemonic.LDA, AddressingMode.ZeroPageIndirectIndexedY);

        // LDX
        _table[0xAE] = new(Mnemonic.LDX, AddressingMode.Absolute);
        _table[0xBE] = new(Mnemonic.LDX, AddressingMode.AbsoluteIndexedY);
        _table[0xA2] = new(Mnemonic.LDX, AddressingMode.Immediate);
        _table[0xA6] = new(Mnemonic.LDX, AddressingMode.ZeroPage);
        _table[0xB6] = new(Mnemonic.LDX, AddressingMode.ZeroPageIndexedY);

        // LDY
        _table[0xAC] = new(Mnemonic.LDY, AddressingMode.Absolute);
        _table[0xBC] = new(Mnemonic.LDY, AddressingMode.AbsoluteIndexedX);
        _table[0xA0] = new(Mnemonic.LDY, AddressingMode.Immediate);
        _table[0xA4] = new(Mnemonic.LDY, AddressingMode.ZeroPage);
        _table[0xB4] = new(Mnemonic.LDY, AddressingMode.ZeroPageIndexedX);

        // LSR
        _table[0x4E] = new(Mnemonic.LSR, AddressingMode.Absolute);
        _table[0x5E] = new(Mnemonic.LSR, AddressingMode.AbsoluteIndexedX);
        _table[0x4A] = new(Mnemonic.LSR, AddressingMode.Accumulator);
        _table[0x46] = new(Mnemonic.LSR, AddressingMode.ZeroPage);
        _table[0x56] = new(Mnemonic.LSR, AddressingMode.ZeroPageIndexedX);

        // NOP
        _table[0xEA] = new(Mnemonic.NOP, AddressingMode.Implied);

        // ORA
        _table[0x0D] = new(Mnemonic.ORA, AddressingMode.Absolute);
        _table[0x1D] = new(Mnemonic.ORA, AddressingMode.AbsoluteIndexedX);
        _table[0x19] = new(Mnemonic.ORA, AddressingMode.AbsoluteIndexedY);
        _table[0x09] = new(Mnemonic.ORA, AddressingMode.Immediate);
        _table[0x05] = new(Mnemonic.ORA, AddressingMode.ZeroPage);
        _table[0x01] = new(Mnemonic.ORA, AddressingMode.ZeroPageIndexedIndirect);
        _table[0x15] = new(Mnemonic.ORA, AddressingMode.ZeroPageIndexedX);
        _table[0x12] = new(Mnemonic.ORA, AddressingMode.ZeroPageIndirect);
        _table[0x11] = new(Mnemonic.ORA, AddressingMode.ZeroPageIndirectIndexedY);

        // Stack
        _table[0x48] = new(Mnemonic.PHA, AddressingMode.Stack);
        _table[0x08] = new(Mnemonic.PHP, AddressingMode.Stack);
        _table[0xDA] = new(Mnemonic.PHX, AddressingMode.Stack);
        _table[0x5A] = new(Mnemonic.PHY, AddressingMode.Stack);
        _table[0x68] = new(Mnemonic.PLA, AddressingMode.Stack);
        _table[0x28] = new(Mnemonic.PLP, AddressingMode.Stack);
        _table[0xFA] = new(Mnemonic.PLX, AddressingMode.Stack);
        _table[0x7A] = new(Mnemonic.PLY, AddressingMode.Stack);

        // ROL
        _table[0x2E] = new(Mnemonic.ROL, AddressingMode.Absolute);
        _table[0x3E] = new(Mnemonic.ROL, AddressingMode.AbsoluteIndexedX);
        _table[0x2A] = new(Mnemonic.ROL, AddressingMode.Accumulator);
        _table[0x26] = new(Mnemonic.ROL, AddressingMode.ZeroPage);
        _table[0x36] = new(Mnemonic.ROL, AddressingMode.ZeroPageIndexedX);

        // ROR
        _table[0x6E] = new(Mnemonic.ROR, AddressingMode.Absolute);
        _table[0x7E] = new(Mnemonic.ROR, AddressingMode.AbsoluteIndexedX);
        _table[0x6A] = new(Mnemonic.ROR, AddressingMode.Accumulator);
        _table[0x66] = new(Mnemonic.ROR, AddressingMode.ZeroPage);
        _table[0x76] = new(Mnemonic.ROR, AddressingMode.ZeroPageIndexedX);

        // RTI / RTS
        _table[0x40] = new(Mnemonic.RTI, AddressingMode.Stack);
        _table[0x60] = new(Mnemonic.RTS, AddressingMode.Stack);

        // SBC
        _table[0xED] = new(Mnemonic.SBC, AddressingMode.Absolute);
        _table[0xFD] = new(Mnemonic.SBC, AddressingMode.AbsoluteIndexedX);
        _table[0xF9] = new(Mnemonic.SBC, AddressingMode.AbsoluteIndexedY);
        _table[0xE9] = new(Mnemonic.SBC, AddressingMode.Immediate);
        _table[0xE5] = new(Mnemonic.SBC, AddressingMode.ZeroPage);
        _table[0xE1] = new(Mnemonic.SBC, AddressingMode.ZeroPageIndexedIndirect);
        _table[0xF5] = new(Mnemonic.SBC, AddressingMode.ZeroPageIndexedX);
        _table[0xF2] = new(Mnemonic.SBC, AddressingMode.ZeroPageIndirect);
        _table[0xF1] = new(Mnemonic.SBC, AddressingMode.ZeroPageIndirectIndexedY);

        // Flags
        _table[0x18] = new(Mnemonic.CLC, AddressingMode.Implied);
        _table[0x38] = new(Mnemonic.SEC, AddressingMode.Implied);
        _table[0x58] = new(Mnemonic.CLI, AddressingMode.Implied);
        _table[0x78] = new(Mnemonic.SEI, AddressingMode.Implied);
        _table[0xB8] = new(Mnemonic.CLV, AddressingMode.Implied);
        _table[0xD8] = new(Mnemonic.CLD, AddressingMode.Implied);
        _table[0xF8] = new(Mnemonic.SED, AddressingMode.Implied);

        // STA
        _table[0x8D] = new(Mnemonic.STA, AddressingMode.Absolute);
        _table[0x9D] = new(Mnemonic.STA, AddressingMode.AbsoluteIndexedX);
        _table[0x99] = new(Mnemonic.STA, AddressingMode.AbsoluteIndexedY);
        _table[0x85] = new(Mnemonic.STA, AddressingMode.ZeroPage);
        _table[0x81] = new(Mnemonic.STA, AddressingMode.ZeroPageIndexedIndirect);
        _table[0x95] = new(Mnemonic.STA, AddressingMode.ZeroPageIndexedX);
        _table[0x92] = new(Mnemonic.STA, AddressingMode.ZeroPageIndirect);
        _table[0x91] = new(Mnemonic.STA, AddressingMode.ZeroPageIndirectIndexedY);

        // STX
        _table[0x8E] = new(Mnemonic.STX, AddressingMode.Absolute);
        _table[0x86] = new(Mnemonic.STX, AddressingMode.ZeroPage);
        _table[0x96] = new(Mnemonic.STX, AddressingMode.ZeroPageIndexedY);

        // STY
        _table[0x8C] = new(Mnemonic.STY, AddressingMode.Absolute);
        _table[0x84] = new(Mnemonic.STY, AddressingMode.ZeroPage);
        _table[0x94] = new(Mnemonic.STY, AddressingMode.ZeroPageIndexedX);

        // STZ
        _table[0x9C] = new(Mnemonic.STZ, AddressingMode.Absolute);
        _table[0x9E] = new(Mnemonic.STZ, AddressingMode.AbsoluteIndexedX);
        _table[0x64] = new(Mnemonic.STZ, AddressingMode.ZeroPage);
        _table[0x74] = new(Mnemonic.STZ, AddressingMode.ZeroPageIndexedX);

        // Transfers
        _table[0xAA] = new(Mnemonic.TAX, AddressingMode.Implied);
        _table[0xA8] = new(Mnemonic.TAY, AddressingMode.Implied);
        _table[0x8A] = new(Mnemonic.TXA, AddressingMode.Implied);
        _table[0x98] = new(Mnemonic.TYA, AddressingMode.Implied);
        _table[0xBA] = new(Mnemonic.TSX, AddressingMode.Implied);
        _table[0x9A] = new(Mnemonic.TXS, AddressingMode.Implied);

        // TRB / TSB
        _table[0x14] = new(Mnemonic.TRB, AddressingMode.ZeroPage);
        _table[0x1C] = new(Mnemonic.TRB, AddressingMode.Absolute);
        _table[0x04] = new(Mnemonic.TSB, AddressingMode.ZeroPage);
        _table[0x0C] = new(Mnemonic.TSB, AddressingMode.Absolute);

        // System
        _table[0xCB] = new(Mnemonic.WAI, AddressingMode.Implied);
        _table[0xDB] = new(Mnemonic.STP, AddressingMode.Implied);
    }

    public static Instruction Get(byte opCode)
        => _table[opCode];
}
