using CommandLine;
using EM65XX.Terminal;
using EM65XX.Terminal.CommandLine;
using EM65XX.Terminal.Parsers;


Parser.Default.ParseArguments<Options>(args)
            .WithParsed(HandleOptions)
            .WithNotParsed(HandleParsingErrors);

static void HandleOptions(Options options)
{
    IMemoryParser parser = options.CompressFromat
        ? new CompressFormatParser()
        : new BinParser();

    var runner = new CpuRunner(parser, options.StepMode);

    runner.Run(options.Input);
}

static void HandleParsingErrors(IEnumerable<Error> errors)
{
    Console.Error.WriteLine("Failed to parse command line arguments.");
    Environment.Exit(1);
}