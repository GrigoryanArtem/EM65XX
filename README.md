# EM65XX

An emulator for the W65C02S processor

## Supported modes

| Mode                                     | supported |
| ---------------------------------------- | --------: |
| Absolute a                               |         ✅ |
| Absolute Indexed Indirect (a,x)          |         ❌ |
| Absolute Indexed with X a,x              |         ❌ |
| Absolute Indexed with Y a,y              |         ❌ |
| Absolute Indirect (a)                    |         ❌ |
| Accumulator A                            |         ❌ |
| Immediate #                              |         ✅ |
| Implied i                                |         ❌ |
| Program Counter Relative r               |         ❌ |
| Stack s                                  |         ❌ |
| Zero Page zp                             |         ❌ |
| Zero Page Indexed Indirect (zp,x)        |         ❌ |
| Zero Page Indexed with X zp,x            |         ❌ |
| Zero Page Indexed with Y zp,y            |         ❌ |
| Zero Page Indirect (zp)                  |         ❌ |
| Zero Page Indirect Indexed with Y (zp),y |         ❌ |

## Supported operations

| Mnemonic | supported |
| -------- | --------: |
| ADC      |         ❌ |
| SBC      |         ❌ |
| AND      |         ❌ |
| ORA      |         ❌ |
| EOR      |         ❌ |
| ASL      |         ❌ |
| LSR      |         ❌ |
| ROL      |         ❌ |
| ROR      |         ❌ |
| INC      |         ❌ |
| INX      |         ❌ |
| INY      |         ❌ |
| DEC      |         ❌ |
| DEX      |         ❌ |
| DEY      |         ❌ |
| LDA      |         ✅ |
| LDX      |         ✅ |
| LDY      |         ✅ |
| STA      |         ❌ |
| STX      |         ❌ |
| STY      |         ❌ |
| STZ      |         ❌ |
| TAX      |         ❌ |
| TAY      |         ❌ |
| TXA      |         ❌ |
| TYA      |         ❌ |
| TSX      |         ❌ |
| TXS      |         ❌ |
| PHA      |         ❌ |
| PHP      |         ❌ |
| PHX      |         ❌ |
| PHY      |         ❌ |
| PLA      |         ❌ |
| PLP      |         ❌ |
| PLX      |         ❌ |
| PLY      |         ❌ |
| CLC      |         ❌ |
| SEC      |         ❌ |
| CLI      |         ❌ |
| SEI      |         ❌ |
| CLV      |         ❌ |
| CLD      |         ❌ |
| SED      |         ❌ |
| CMP      |         ❌ |
| CPX      |         ❌ |
| CPY      |         ❌ |
| BIT      |         ❌ |
| TSB      |         ❌ |
| TRB      |         ❌ |
| BCC      |         ❌ |
| BCS      |         ❌ |
| BEQ      |         ❌ |
| BMI      |         ❌ |
| BNE      |         ❌ |
| BPL      |         ❌ |
| BVC      |         ❌ |
| BVS      |         ❌ |
| BRA      |         ❌ |
| BBR0     |         ❌ |
| BBR1     |         ❌ |
| BBR2     |         ❌ |
| BBR3     |         ❌ |
| BBR4     |         ❌ |
| BBR5     |         ❌ |
| BBR6     |         ❌ |
| BBR7     |         ❌ |
| BBS0     |         ❌ |
| BBS1     |         ❌ |
| BBS2     |         ❌ |
| BBS3     |         ❌ |
| BBS4     |         ❌ |
| BBS5     |         ❌ |
| BBS6     |         ❌ |
| BBS7     |         ❌ |
| RMB0     |         ❌ |
| RMB1     |         ❌ |
| RMB2     |         ❌ |
| RMB3     |         ❌ |
| RMB4     |         ❌ |
| RMB5     |         ❌ |
| RMB6     |         ❌ |
| RMB7     |         ❌ |
| SMB0     |         ❌ |
| SMB1     |         ❌ |
| SMB2     |         ❌ |
| SMB3     |         ❌ |
| SMB4     |         ❌ |
| SMB5     |         ❌ |
| SMB6     |         ❌ |
| SMB7     |         ❌ |
| JMP      |         ❌ |
| JSR      |         ❌ |
| RTS      |         ❌ |
| RTI      |         ❌ |
| BRK      |         ❌ |
| NOP      |         ✅ |
| WAI      |         ❌ |
| STP      |         ❌ |
