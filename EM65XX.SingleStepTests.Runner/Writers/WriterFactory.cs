using EM65XX.SingleStepTests.Runner.Abstractions;

namespace EM65XX.SingleStepTests.Runner.Writers;

public class WriterFactory
{
    private const string DT_FORMAT = "yyyy-MM-dd_HH-mm-ss-ffff";

    private static readonly VoidWriter _void = new();
    private readonly string? _outputDir;
    public WriterFactory(string? outputDirectory)
    {
        if (outputDirectory is null)
            return;
        
        _outputDir = Path.Combine(outputDirectory, DateTime.Now.ToString(DT_FORMAT));
        Directory.CreateDirectory(_outputDir);        
    }

    public IWriter CreateWriter(string name) => _outputDir is not null
        ? new FileWriter(Path.Combine(_outputDir, name))
        : _void;
}
