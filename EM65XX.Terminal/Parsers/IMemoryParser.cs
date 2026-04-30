using EM65XX.Core.Abstraction;

namespace EM65XX.Terminal.Parsers;

public interface IMemoryParser
{
    IMemory Parse(string filename);
}
