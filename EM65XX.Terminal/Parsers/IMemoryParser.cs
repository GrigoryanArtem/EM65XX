using EM65XX.Core.Abstraction;

namespace EM65XX.Terminal.Parsers;

public interface IMemoryParser
{
    void Parse(string filename, IMemory destination);
}
