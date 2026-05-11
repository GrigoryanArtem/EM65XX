using CommandLine;

namespace EM65XX.Terminal.CommandLine;

public class Options
{
    [Option('i', "input", Required = true, HelpText = "path to file")]
    public string Input { get; set; }

    [Option('c', "compress", Required = false, HelpText = "use compress format")]
    public bool CompressFromat { get; set; }

    [Option('s', "step", Required = false, HelpText = "run by steps")]
    public bool StepMode { get; set; }
}
