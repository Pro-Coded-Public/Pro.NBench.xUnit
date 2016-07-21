#region Using Directives

using System.Diagnostics;

using NBench.Sdk;

#endregion

namespace Pro.NBench.xUnit
{
    public static class NBenchTestHelper
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static void RunBenchmark(BenchmarkTestData benchmarkTest)
        {
            Benchmark.PrepareForRun();
            benchmarkTest.Benchmark.Run();
            benchmarkTest.Benchmark.Finish();
        }

        #endregion
    }
}