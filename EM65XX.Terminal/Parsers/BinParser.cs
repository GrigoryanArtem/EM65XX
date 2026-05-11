using EM65XX.Core.Abstraction;

namespace EM65XX.Terminal.Parsers;

public class BinParser(ushort offset = 0) : IMemoryParser
{
    public void Parse(string filename, IMemory memory)
    {
        using var stream = File.OpenRead(filename);
        var memStream = new MemoryStream();

        stream.CopyTo(memStream);

        memory.Load(offset, memStream.ToArray());        
    }
}
