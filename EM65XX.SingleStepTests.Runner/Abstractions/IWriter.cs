namespace EM65XX.SingleStepTests.Runner.Abstractions;

public interface IWriter
{
    void WriteLine(string? str = null);
    void Flush();
}
