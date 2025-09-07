using BenchmarkDotNet.Running;
using Xunit;

namespace TaskNET.Test.Performance;

public class PerformanceTests
{
    [Fact]
    [Trait("Category", "Performance")]
    public void RunDataProviderBenchmarks()
    {
        // This test will run the benchmarks defined in the AppDataProviderBenchmarks class
        BenchmarkRunner.Run<AppDataProviderBenchmarks>();
    }
}
