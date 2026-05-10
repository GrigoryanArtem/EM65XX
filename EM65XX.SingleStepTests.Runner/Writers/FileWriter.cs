using EM65XX.SingleStepTests.Runner.Abstractions;
using System.Text;

namespace EM65XX.SingleStepTests.Runner.Writers;

public class FileWriter(string path) : IWriter
{
    private readonly StringBuilder _builder = new();

    public void WriteLine(string? str = null)
        => _builder.AppendLine(str);    

    public void Flush()
    {
        File.WriteAllText(path, _builder.ToString());
        _builder.Clear();
    }
}
