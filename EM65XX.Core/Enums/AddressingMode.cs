namespace EM65XX.Core.Enums;

public enum AddressingMode
{
    /// <summary>    
    /// With Absolute addressing the second and third bytes of the instruction from the 16-bit address.
    /// <para><b>a</b></para>
    /// </summary>
    Absolute,

    /// <summary>
    /// With the Absolute Indexed Indirect addressing mode, the X Index Register is added to the second and
    /// third byes of the instruction to form an address to a pointer. This address mode is only used with the JMP
    /// instruction and the program Counter is loaded with the first and second bytes at this pointer
    /// <para><b>(a, x)</b></para>
    /// </summary>
    AbsoluteIndexedIndirect,

    /// <summary>
    /// With the Absolute Indexed with X addressing mode, the X Index Register is added to the second and third 
    /// bytes of the instruction to form the 16-bits of the effective address.
    /// <para><b>a, x</b></para>
    /// </summary>
    AbsoluteIndexedX,

    /// <summary>
    /// With the Absolute Indexed with Y addressing mode, the Y Index Register is added to the second and third 
    /// bytes of the instruction to form the 16-bit effective address.
    /// <para><b>a, y</b></para>
    /// </summary>
    AbsoluteIndexedY,

    /// <summary>
    /// With the Absolute Indirect addressing mode, the second and third bytes of the instruction form an address 
    /// to a pointer. This address mode is only used with the JMP instruction and the Program Counter is loaded
    /// with the first and second bytes at this pointer.
    /// <para><b>a</b></para>
    /// </summary>
    AbsoluteIndirect,

    /// <summary>
    /// With Accumulator addressing the operand is implied as the Accumulator and therefore only a single byte 
    /// forms the instruction.
    /// <para><b>A</b></para>
    /// </summary>
    Accumulator,

    /// <summary>
    /// With Immediate Addressing the operand is the second byte of the instruction.
    /// <para><b>#</b></para>
    /// </summary>
    Immediate,

    /// <summary>
    /// Implied addressing uses a single byte instruction. The operand is implicitly defined by the instruction.
    /// <para><b>i</b></para>
    /// </summary>
    Implied,

    /// <summary>
    /// The Program Counter relative addressing mode, sometimes referred to as Relative Addressing, is used 
    /// with the Branch instructions. If the condition being tested is met, the second byte of the instruction is 
    /// added to the Program Counter and program control is transferred to this new memory location.
    /// <para><b>r</b></para>
    /// </summary>
    ProgramCounterRelative,

    /// <summary>
    /// The Stack may use memory from <i>0100</i> to <i>01FF</i> and the effective address of the Stack address mode will 
    /// always be within this range. Stack addressing refers to all instructions that push or pull data from the
    /// stack, such as Push, Pull, Jump to Subroutine, Return from Subroutine, Interrupts and Return from
    /// Interrupt.
    /// <para><b>s</b></para>
    /// </summary>
    Stack,

    /// <summary>
    /// With Zero Page (zp) addressing the second byte of the instruction is the address of the operand in page 
    /// zero. 
    /// <para><b>zp</b></para>
    /// </summary>
    ZeroPage,

    /// <summary>
    /// The Zero Page Indexed Indirect addressing mode is often referred to as Indirect,X. The second byte of 
    /// the instruction is the zero page address to which the X Index Register is added and the result points to
    /// the low byte of the indirect address.
    /// <para><b>(zp,x)</b></para>
    /// </summary>
    ZeroPageIndexedIndirect,

    /// <summary>
    /// With Zero Page Indexed with X addressing mode, the X Index Register is added to the second byte of 
    /// instruction to form the effective address.
    /// <para><b>zp,x</b></para>
    /// </summary>
    ZeroPageIndexedX,

    /// <summary>
    /// With Zero Page Indexed with Y addressing, the second byte of the instruction is the zero page address to 
    /// which the Y Index Register is added to form the page zero effective address.
    /// <para><b>zp,y</b></para>
    /// </summary>
    ZeroPageIndexedY,

    /// <summary>
    /// With Zero Page Indirect addressing mode, the second byte of the instruction is a zero page indirect 
    /// address that points to the low byte of a two byte effective address.
    /// <para><b>(zp)</b></para>
    /// </summary>
    ZeroPageIndirect,

    /// <summary>
    /// The Zero Page Indirect Indexed with Y addressing mode is often referred to as Indirect Y. The second 
    /// byte of the instruction points to the low byte of a two byte (16-bit) base address in page zero. Y Index
    /// Register is added to the base address to form the effective address.
    /// <para><b>(zp), y</b></para>
    /// </summary>
    ZeroPageIndirectIndexedY,
}
