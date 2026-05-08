using EM65XX.SingleStepTests.Runner.Abstractions;
using EM65XX.SingleStepTests.Runner.Model;
using System.Text.Json;

namespace EM65XX.SingleStepTests.Runner.Containers;

public class FileTests(string testsDirectory, string? pattern) : ITestContainer
{
    private static readonly JsonSerializerOptions JSON_OPTIONS = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public IEnumerable<TestBatch> GetTests()
    {
        if(!Directory.Exists(testsDirectory))
            throw new DirectoryNotFoundException($"The directory '{testsDirectory}' does not exist.");

        var files = Directory.GetFiles(testsDirectory, $"{pattern}.json");

        foreach (var file in files)
        {
            TestData[]? tests;

            try
            {
                var json = File.ReadAllText(file);
                tests = JsonSerializer.Deserialize<TestData[]?>(json, JSON_OPTIONS);

                if(tests is null) 
                {    
                    Console.Error.WriteLine($"No tests found in {file}.");
                    continue;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to read or parse {file}: {ex.Message}");
                continue;
            }

            yield return new TestBatch
            {
                Name = Path.GetFileNameWithoutExtension(file),
                Tests = tests
            };
        }
    }
}
