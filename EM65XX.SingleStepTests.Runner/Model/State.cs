using System.Text.Json.Serialization;

namespace EM65XX.SingleStepTests.Runner.Model;

public class State
{
    [JsonPropertyName("pc")]
    public int PC { get; init; }
    public int S { get; init; }
    public int A { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public int P { get; init; }

    public required int[][] Ram { get; init; }
}
