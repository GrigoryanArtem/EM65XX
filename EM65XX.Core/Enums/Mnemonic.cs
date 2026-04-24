namespace EM65XX.Core.Enums;

public enum Mnemonic : byte
{
    #region Arithmetic

    /// <summary>
    /// <b>AD</b>d memory to accumulator with <b>C</b>arry
    /// </summary>
    ADC,

    /// <summary>
    /// <b>S</b>u<b>B</b>tract memory from accumulator with borrow (<b>C</b>arry bit)
    /// </summary>
    SBC,

    #endregion

    #region Logic

    /// <summary>
    /// "<b>AND</b>" memory with accumulator
    /// </summary>
    AND,

    /// <summary>
    /// "<b>OR</b>" memory with <b>A</b>ccumulator
    /// </summary>
    ORA,

    /// <summary>
    /// "<b>E</b>xclusive <b>OR</b>" memory with accumulate
    /// </summary>
    EOR,

    #endregion

    #region Shifts / Rotates

    ASL,
    LSR,
    ROL,
    ROR,

    #endregion

    #region Increments / Decrements

    INC,
    INX,
    INY,
    DEC,
    DEX,
    DEY,

    #endregion

    #region Loads

    LDA,
    LDX,
    LDY,

    #endregion

    #region Stores

    STA,
    STX,
    STY,
    STZ,

    #endregion

    #region Transfers

    TAX,
    TAY,
    TXA,
    TYA,
    TSX,
    TXS,

    #endregion

    #region Stack

    PHA,
    PHP,
    PHX,
    PHY,
    PLA,
    PLP,
    PLX,
    PLY,

    #endregion

    #region Flags

    CLC,
    SEC,
    CLI,
    SEI,
    CLV,
    CLD,
    SED,

    #endregion

    #region Comparison

    CMP,
    CPX,
    CPY,

    #endregion

    #region Bit

    BIT,
    TSB,
    TRB,

    #endregion

    #region Branches

    BCC,
    BCS,
    BEQ,
    BMI,
    BNE,
    BPL,
    BVC,
    BVS,
    BRA,

    #endregion

    #region Bit Branches

    BBR0,
    BBR1,
    BBR2,
    BBR3,
    BBR4,
    BBR5,
    BBR6,
    BBR7,

    BBS0,
    BBS1,
    BBS2,
    BBS3,
    BBS4,
    BBS5,
    BBS6,
    BBS7,

    #endregion

    #region Bit Manipulation 

    RMB0,
    RMB1,
    RMB2,
    RMB3,
    RMB4,
    RMB5,
    RMB6,
    RMB7,

    SMB0,
    SMB1,
    SMB2,
    SMB3,
    SMB4,
    SMB5,
    SMB6,
    SMB7,

    #endregion

    #region Jumps / Calls

    JMP,
    JSR,
    RTS,
    RTI,

    #endregion

    #region System

    BRK,
    NOP,
    WAI,
    STP

    #endregion
}
