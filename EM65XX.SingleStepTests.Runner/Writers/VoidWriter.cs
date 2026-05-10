using EM65XX.SingleStepTests.Runner.Abstractions;

namespace EM65XX.SingleStepTests.Runner.Writers;

public class VoidWriter : IWriter
{
    public void WriteLine(string? _ = null) { }
    public void Flush() { }
}
