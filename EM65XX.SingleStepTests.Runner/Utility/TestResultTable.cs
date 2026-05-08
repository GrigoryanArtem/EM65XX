using EM65XX.Core;
using EM65XX.SingleStepTests.Runner.Table;

namespace EM65XX.SingleStepTests.Runner.Utility;

public static class TestResultTable
{
    private static readonly char[] HEX = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'];

    public static string CreateTable(Dictionary<string, double> results)
    {
        var tb = TableBuilder.Create(TableOptions.Header)
            .SetVSeparator(" ")
            .AddColumn(new() { Header = "", Align = Align.Left, Width = 2 });

        foreach (var symbol in HEX)
            tb.AddColumn(new() { Header = symbol.ToString().ToUpperInvariant(), Align = Align.Right, Width = 5 });

        var buffer = new object?[HEX.Length + 1];
        var nameBuffer = new object?[HEX.Length + 1];
        foreach (var f in HEX)
        {

            nameBuffer[0] = f.ToString().ToUpperInvariant();

            foreach (var (i, s) in HEX.Index())
            {
                var code = new string([f, s]);

                if (results.TryGetValue(code, out var value) && value < 100.0)
                {
                    var instr = InstructionsTable.Get(Convert.ToByte(code, 16));

                    nameBuffer[i + 1] = instr.Mnemonic.ToString();
                    buffer[i + 1] = value.ToString("f1");
                }
                else
                {
                    nameBuffer[i + 1] = null!;
                    buffer[i + 1] = null!;
                }
            }

            tb.AddRow(nameBuffer);
            tb.AddRow(buffer);
        }

        return tb.Build();
    }
}
