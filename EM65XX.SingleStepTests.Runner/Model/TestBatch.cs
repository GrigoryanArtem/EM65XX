namespace EM65XX.SingleStepTests.Runner.Model;

public class TestBatch
{
    public string Name { get; set; }
    public TestData[] Tests { get; set; } = [];
    public int Count => Tests.Length;
}
