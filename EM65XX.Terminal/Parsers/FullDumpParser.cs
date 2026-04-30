using EM65XX.Core;
using EM65XX.Core.Abstraction;

namespace EM65XX.Terminal.Parsers;

public class FullDumpParser : IMemoryParser
{
    public IMemory Parse(string filename)
    {
        using var stream = File.OpenRead(filename);
        var memStream = new MemoryStream();

        stream.CopyTo(memStream);

        return new Memory64K(memStream.ToArray());        
    }
}
