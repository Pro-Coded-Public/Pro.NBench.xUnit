#region Using Directives

using System.Diagnostics;

using Xunit;
using Xunit.Abstractions;

#endregion

// Optional - disable parralel test
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Pro.NBench.xUnit.Tests
{
    public class PerformanceTests
    {
        #region Constructors and Destructors

        public PerformanceTests(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        #endregion

        #region Public Methods and Operators

        [Theory, MemberData("Benchmarks", MemberType = typeof(NBenchTestHandler<DictionaryMemoryTests>))]
        public void DictionaryMemory(BenchmarkTestData benchmarkTest)
        {
            NBenchTestHelper.RunBenchmark(benchmarkTest);
        }

        [Theory, MemberData("Benchmarks", MemberType = typeof(NBenchTestHandler<DictionaryThroughputTests>))]
        public void DictionaryThroughput(BenchmarkTestData benchmarkTest)
        {
            NBenchTestHelper.RunBenchmark(benchmarkTest);
        }

        [Theory, MemberData("Benchmarks", MemberType = typeof(NBenchTestHandler<GarbageCollectionTests>))]
        public void GarbageCollection(BenchmarkTestData benchmarkTest)
        {
            NBenchTestHelper.RunBenchmark(benchmarkTest);
        }

        #endregion
    }
}