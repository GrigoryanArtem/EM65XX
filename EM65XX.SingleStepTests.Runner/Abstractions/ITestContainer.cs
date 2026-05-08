using EM65XX.SingleStepTests.Runner.Model;

namespace EM65XX.SingleStepTests.Runner.Abstractions;

public interface ITestContainer
{
    IEnumerable<TestBatch> GetTests();
}
