using EM65XX.Core.Abstraction;
using System.Globalization;

namespace EM65XX.Terminal.Parsers;

public class CompressFormatParser : IMemoryParser
{
    private readonly char[] SEPARATORS = [' ', ',', '\t'];

    private const char ADDRESS_MARK = '>';
    private const char COMMENT_MARK = '#';

    public void Parse(string filename, IMemory destination)
    {        
        destination.Clear(0xEA);

        using var stream = File.OpenRead(filename);
        var reader = new StreamReader(stream);

        ushort currentAddress = 0x00;

        string? line;
        var data = new List<byte>();

        while ((line = reader.ReadLine()) is not null)
        {
            var commentIndex = line.IndexOf(COMMENT_MARK);
            
            if(commentIndex >= 0)
                line = line[..commentIndex];

            line = line.Trim();

            if (line.FirstOrDefault() == ADDRESS_MARK)
            {
                destination.Load(currentAddress, data);
                data.Clear();

                var newAddress = line[1..];
                if (!UInt16.TryParse(newAddress, NumberStyles.HexNumber, null, out currentAddress))
                    throw new FormatException($"Invalid i16 '{newAddress}' in line: {line}");

                continue;
            }

            foreach (var token in line.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!Byte.TryParse(token, NumberStyles.HexNumber, null, out var value))
                    throw new FormatException($"Invalid i8 '{token}' in line: {line}");

                data.Add(value);
            }
        }

        destination.Load(currentAddress, data);        
    }
}
