using CommandLine;

namespace EM65XX.SingleStepTests.Runner.CommandLine;

public class Options
{
    [Option('d', "dir", Required = true, HelpText = "test data directory")]
    public string Directory { get; set; }

    [Option('o', "output", Required = false, HelpText = "output directory")]
    public string Output { get; set; }

    [Option('t', "table", Required = false, HelpText = "print table")]
    public bool Table { get; set; }

    [Option('i', "instr", Required = false, HelpText = "instruction pattern")]
    public string Instruction { get; set; }
}
