# EM65XX

An emulator for the W65C02S processor

## EM65XX.Terminal (em65)

`EM65XX.Terminal` builds an executable named `em65`. It can load either a raw binary file or the repository's compressed text format.

Run it from the repository root with:

```
dotnet run --project EM65XX.Terminal -- --input <path-to-program>
```

Examples:

```
em65 --input EM65XX.Terminal\examples\extbin\test --compress
em65 -i EM65XX.Terminal\examples\extbin\add_decimal -c
em65 -i program.bin
em65 -i program.bin --step
```

Options:

| option             | required | description                                                                                                                        |
| ------------------ | -------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| `-i`, `--input`    | yes      | Path to the program file to load.                                                                                                  |
| `-c`, `--compress` | no       | Read the input as compressed text format instead of raw binary.                                                                    |
| `-s`, `--step`     | no       | Run interactively one instruction at a time. Press any key for the next instruction, `I` to print registers, or `Q`/`Esc` to quit. |
| `--help`           | no       | Print command-line help.                                                                                                           |
| `--version`        | no       | Print version information.                                                                                                         |

Raw binary input is loaded at address `0x0000`.

Compressed input is a text format with hexadecimal bytes. Address markers start with `>`, comments start with `#`, and byte tokens can be separated by spaces, commas, or tabs. Memory is cleared with `0xEA` before loading compressed input.

Example compressed file:

```
>FFFC
00 80

>8000
18        # CLC
A9 15     # LDA #$15
69 17     # ADC #$17
DB        # STP
```

## EM65XX.SingleStepTests.Runner

`EM65XX.SingleStepTests.Runner` runs JSON single-step tests. Each matching `*.json` file in the input directory is treated as one test batch. The file name, without `.json`, is used as the batch name.

Run it from the repository root with:

```powershell
dotnet run --project EM65XX.SingleStepTests.Runner -- --dir <path-to-test-data>
```

Examples:

```powershell
dotnet run --project EM65XX.SingleStepTests.Runner -- --dir .\ProcessorTests\65x02
dotnet run --project EM65XX.SingleStepTests.Runner -- -d .\ProcessorTests\65x02 --instr adc
dotnet run --project EM65XX.SingleStepTests.Runner -- -d .\ProcessorTests\65x02 --instr "a9" --table
dotnet run --project EM65XX.SingleStepTests.Runner -- -d .\ProcessorTests\65x02 --output .\test-results
```

Options:

| option           | required | description                                                                                                                |
| ---------------- | -------- | -------------------------------------------------------------------------------------------------------------------------- |
| `-d`, `--dir`    | yes      | Directory containing JSON test files.                                                                                      |
| `-o`, `--output` | no       | Directory for per-batch result logs. The runner creates a timestamped subdirectory and writes one log file per test batch. |
| `-t`, `--table`  | no       | Print a summary table with pass percentages for all loaded batches.                                                        |
| `-i`, `--instr`  | no       | File-name pattern for selecting test batches. Defaults to `*`, so all `*.json` files are used.                             |
| `--help`         | no       | Print command-line help.                                                                                                   |
| `--version`      | no       | Print version information.                                                                                                 |

Each test entry is expected to contain a `name`, an `initial` CPU state, and a `final` CPU state. The runner writes the initial RAM bytes, sets `PC`, `S`, `A`, `X`, `Y`, and `P`, executes one CPU tick, and compares those registers with the expected final state.

## Supported modes

| mode                                     | supported |
| ---------------------------------------- | --------: |
| Absolute a                               |         ✅ |
| Absolute Indexed Indirect (a,x)          |         ✅ |
| Absolute Indexed with X a,x              |         ✅ |
| Absolute Indexed with Y a,y              |         ✅ |
| Absolute Indirect (a)                    |         ✅ |
| Accumulator A                            |         ✅ |
| Immediate #                              |         ✅ |
| Implied i                                |         ✅ |
| Program Counter Relative r               |         ✅ |
| Stack s                                  |         ✅ |
| Zero Page zp                             |         ✅ |
| Zero Page Indexed Indirect (zp,x)        |         ✅ |
| Zero Page Indexed with X zp,x            |         ✅ |
| Zero Page Indexed with Y zp,y            |         ✅ |
| Zero Page Indirect (zp)                  |         ✅ |
| Zero Page Indirect Indexed with Y (zp),y |         ✅ |

## Supported operations

| mnemonic | supported | mnemonic | supported |
| -------- | --------: | -------- | --------: |
| ADC      |         ✅ | SBC      |         ✅ |
| AND      |         ✅ | ORA      |         ✅ |
| EOR      |         ✅ | ASL      |         ✅ |
| LSR      |         ✅ | ROL      |         ✅ |
| ROR      |         ✅ | INC      |         ✅ |
| INX      |         ✅ | INY      |         ✅ |
| DEC      |         ✅ | DEX      |         ✅ |
| DEY      |         ✅ | LDA      |         ✅ |
| LDX      |         ✅ | LDY      |         ✅ |
| STA      |         ✅ | STX      |         ✅ |
| STY      |         ✅ | STZ      |         ✅ |
| TAX      |         ✅ | TAY      |         ✅ |
| TXA      |         ✅ | TYA      |         ✅ |
| TSX      |         ✅ | TXS      |         ✅ |
| PHA      |         ✅ | PHP      |         ✅ |
| PHX      |         ✅ | PHY      |         ✅ |
| PLA      |         ✅ | PLP      |         ✅ |
| PLX      |         ✅ | PLY      |         ✅ |
| CLC      |         ✅ | SEC      |         ✅ |
| CLI      |         ✅ | SEI      |         ✅ |
| CLV      |         ✅ | CLD      |         ✅ |
| SED      |         ✅ | CMP      |         ✅ |
| CPX      |         ✅ | CPY      |         ✅ |
| BIT      |         ✅ | TSB      |         ✅ |
| TRB      |         ✅ | BCC      |         ✅ |
| BCS      |         ✅ | BEQ      |         ✅ |
| BMI      |         ✅ | BNE      |         ✅ |
| BPL      |         ✅ | BVC      |         ✅ |
| BVS      |         ✅ | BRA      |         ✅ |
| BBR0     |         ✅ | BBR1     |         ✅ |
| BBR2     |         ✅ | BBR3     |         ✅ |
| BBR4     |         ✅ | BBR5     |         ✅ |
| BBR6     |         ✅ | BBR7     |         ✅ |
| BBS0     |         ✅ | BBS1     |         ✅ |
| BBS2     |         ✅ | BBS3     |         ✅ |
| BBS4     |         ✅ | BBS5     |         ✅ |
| BBS6     |         ✅ | BBS7     |         ✅ |
| RMB0     |         ✅ | RMB1     |         ✅ |
| RMB2     |         ✅ | RMB3     |         ✅ |
| RMB4     |         ✅ | RMB5     |         ✅ |
| RMB6     |         ✅ | RMB7     |         ✅ |
| SMB0     |         ✅ | SMB1     |         ✅ |
| SMB2     |         ✅ | SMB3     |         ✅ |
| SMB4     |         ✅ | SMB5     |         ✅ |
| SMB6     |         ✅ | SMB7     |         ✅ |
| JMP      |         ✅ | JSR      |         ✅ |
| RTS      |         ✅ | RTI      |         ✅ |
| BRK      |         ✅ | NOP      |         ✅ |
| WAI      |         ❌ | STP      |         ✅ |
