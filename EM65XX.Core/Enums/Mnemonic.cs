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
    /// "<b>E</b>xclusive <b>OR</b>" memory with accumulator
    /// </summary>
    EOR,

    #endregion

    #region Shifts / Rotates

    /// <summary>
    /// <b>A</b>rithmetic <b>S</b>hift <b>L</b>eft
    /// </summary>
    ASL,

    /// <summary>
    /// <b>L</b>ogical <b>S</b>hift <b>R</b>ight
    /// </summary>
    LSR,

    /// <summary>
    /// <b>RO</b>tate <b>L</b>eft through carry
    /// </summary>
    ROL,

    /// <summary>
    /// <b>RO</b>tate <b>R</b>ight through carry
    /// </summary>
    ROR,

    #endregion

    #region Increments / Decrements

    /// <summary>
    /// <b>INC</b>rement memory
    /// </summary>
    INC,

    /// <summary>
    /// <b>IN</b>crement register <b>X</b>
    /// </summary>
    INX,

    /// <summary>
    /// <b>IN</b>crement register <b>Y</b>
    /// </summary>
    INY,

    /// <summary>
    /// <b>DEC</b>rement memory
    /// </summary>
    DEC,

    /// <summary>
    /// <b>DE</b>crement register <b>X</b>
    /// </summary>
    DEX,

    /// <summary>
    /// <b>DE</b>crement register <b>Y</b>
    /// </summary>
    DEY,

    #endregion

    #region Loads

    /// <summary>
    /// <b>L</b>oa<b>D</b> <b>A</b>ccumulator
    /// </summary>
    LDA,

    /// <summary>
    /// <b>L</b>oa<b>D</b> <b>X</b> register
    /// </summary>
    LDX,

    /// <summary>
    /// <b>L</b>oa<b>D</b> <b>Y</b> register
    /// </summary>
    LDY,

    #endregion

    #region Stores

    /// <summary>
    /// <b>ST</b>ore <b>A</b>ccumulator
    /// </summary>
    STA,

    /// <summary>
    /// <b>ST</b>ore <b>X</b> register
    /// </summary>
    STX,

    /// <summary>
    /// <b>ST</b>ore <b>Y</b> register
    /// </summary>
    STY,

    /// <summary>
    /// <b>ST</b>ore <b>Z</b>ero (write zero to memory)
    /// </summary>
    STZ,

    #endregion

    #region Transfers

    /// <summary>
    /// <b>T</b>ransfer <b>A</b>ccumulator to <b>X</b>
    /// </summary>
    TAX,

    /// <summary>
    /// <b>T</b>ransfer <b>A</b>ccumulator to <b>Y</b>
    /// </summary>
    TAY,

    /// <summary>
    /// <b>T</b>ransfer <b>X</b> to <b>A</b>ccumulator
    /// </summary>
    TXA,

    /// <summary>
    /// <b>T</b>ransfer <b>Y</b> to <b>A</b>ccumulator
    /// </summary>
    TYA,

    /// <summary>
    /// <b>T</b>ransfer <b>S</b>tack pointer to <b>X</b>
    /// </summary>
    TSX,

    /// <summary>
    /// <b>T</b>ransfer <b>X</b> to <b>S</b>tack pointer
    /// </summary>
    TXS,

    #endregion

    #region Stack

    /// <summary>
    /// <b>P</b>us<b>H</b> <b>A</b>ccumulator
    /// </summary>
    PHA,

    /// <summary>
    /// <b>P</b>us<b>H</b> <b>P</b>rocessor status
    /// </summary>
    PHP,

    /// <summary>
    /// <b>P</b>us<b>H</b> <b>X</b> register
    /// </summary>
    PHX,

    /// <summary>
    /// <b>P</b>us<b>H</b> <b>Y</b> register
    /// </summary>
    PHY,

    /// <summary>
    /// <b>P</b>u<b>L</b>l <b>A</b>ccumulator
    /// </summary>
    PLA,

    /// <summary>
    /// <b>P</b>u<b>L</b>l <b>P</b>rocessor status
    /// </summary>
    PLP,

    /// <summary>
    /// <b>P</b>u<b>L</b>l <b>X</b> register
    /// </summary>
    PLX,

    /// <summary>
    /// <b>P</b>u<b>L</b>l <b>Y</b> register
    /// </summary>
    PLY,

    #endregion

    #region Flags

    /// <summary>
    /// <b>CL</b>ear <b>C</b>arry
    /// </summary>
    CLC,

    /// <summary>
    /// <b>SE</b>t <b>C</b>arry
    /// </summary>
    SEC,

    /// <summary>
    /// <b>CL</b>ear <b>I</b>nterrupt disable
    /// </summary>
    CLI,

    /// <summary>
    /// <b>SE</b>t <b>I</b>nterrupt disable
    /// </summary>
    SEI,

    /// <summary>
    /// <b>CL</b>ear o<b>V</b>erflow
    /// </summary>
    CLV,

    /// <summary>
    /// <b>CL</b>ear <b>D</b>ecimal mode
    /// </summary>
    CLD,

    /// <summary>
    /// <b>SE</b>t <b>D</b>ecimal mode
    /// </summary>
    SED,

    #endregion

    #region Comparison

    /// <summary>
    /// <b>C</b>o<b>MP</b>are with accumulator
    /// </summary>
    CMP,

    /// <summary>
    /// <b>C</b>o<b>MP</b>are with <b>X</b>
    /// </summary>
    CPX,

    /// <summary>
    /// <b>C</b>o<b>MP</b>are with <b>Y</b>
    /// </summary>
    CPY,

    #endregion

    #region Bit

    /// <summary>
    /// <b>BIT</b> test bits in memory with accumulator
    /// </summary>
    BIT,

    /// <summary>
    /// <b>T</b>est and <b>S</b>et <b>B</b>its
    /// </summary>
    TSB,

    /// <summary>
    /// <b>T</b>est and <b>R</b>eset <b>B</b>its
    /// </summary>
    TRB,

    #endregion

    #region Branches

    /// <summary>
    /// <b>B</b>ranch if <b>C</b>arry <b>C</b>lear
    /// </summary>
    BCC,

    /// <summary>
    /// <b>B</b>ranch if <b>C</b>arry <b>S</b>et
    /// </summary>
    BCS,

    /// <summary>
    /// <b>B</b>ranch if <b>E</b>qual (zero set)
    /// </summary>
    BEQ,

    /// <summary>
    /// <b>B</b>ranch if <b>M</b>inus
    /// </summary>
    BMI,

    /// <summary>
    /// <b>B</b>ranch if <b>N</b>ot <b>E</b>qual
    /// </summary>
    BNE,

    /// <summary>
    /// <b>B</b>ranch if <b>P</b>lus
    /// </summary>
    BPL,

    /// <summary>
    /// <b>B</b>ranch if o<b>V</b>erflow <b>C</b>lear
    /// </summary>
    BVC,

    /// <summary>
    /// <b>B</b>ranch if o<b>V</b>erflow <b>S</b>et
    /// </summary>
    BVS,

    /// <summary>
    /// <b>BRA</b>nch always
    /// </summary>
    BRA,

    #endregion

    #region Bit Branches

    /// <summary>
    /// <b>B</b>ranch if <b>B</b>it <b>R</b>eset
    /// </summary>
    BBR0, BBR1, BBR2, BBR3, BBR4, BBR5, BBR6, BBR7,

    /// <summary>
    /// <b>B</b>ranch if <b>B</b>it <b>S</b>et
    /// </summary>
    BBS0, BBS1, BBS2, BBS3, BBS4, BBS5, BBS6, BBS7,

    #endregion

    #region Bit Manipulation 

    /// <summary>
    /// <b>R</b>eset <b>M</b>emory <b>B</b>it
    /// </summary>
    RMB0, RMB1, RMB2, RMB3, RMB4, RMB5, RMB6, RMB7,

    /// <summary>
    /// <b>S</b>et <b>M</b>emory <b>B</b>it
    /// </summary>
    SMB0, SMB1, SMB2, SMB3, SMB4, SMB5, SMB6, SMB7,

    #endregion

    #region Jumps / Calls

    /// <summary>
    /// <b>JMP</b> Jump
    /// </summary>
    JMP,

    /// <summary>
    /// <b>J</b>ump to <b>S</b>ub<b>R</b>outine
    /// </summary>
    JSR,

    /// <summary>
    /// <b>R</b>e<b>T</b>urn from <b>S</b>ubroutine
    /// </summary>
    RTS,

    /// <summary>
    /// <b>R</b>e<b>T</b>urn from <b>I</b>nterrupt
    /// </summary>
    RTI,

    #endregion

    #region System

    /// <summary>
    /// <b>BRK</b> Force interrupt
    /// </summary>
    BRK,

    /// <summary>
    /// <b>NOP</b> No operation
    /// </summary>
    NOP,

    /// <summary>
    /// <b>WA</b>it for <b>I</b>nterrupt
    /// </summary>
    WAI,

    /// <summary>
    /// <b>ST</b>o<b>P</b> mode
    /// </summary>
    STP

    #endregion
}
