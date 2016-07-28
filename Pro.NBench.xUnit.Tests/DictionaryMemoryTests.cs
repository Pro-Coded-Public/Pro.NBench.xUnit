#region Using Directives

using System.Collections.Generic;
using System.Diagnostics;

using NBench;

using Pro.NBench.xUnit.XunitExtensions;

using Xunit;
using Xunit.Abstractions;

#endregion

//Important - disable test parallelization at assembly or collection level
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Pro.NBench.xUnit.Tests
{
    public class DictionaryMemoryTests
    {
        #region Constants

        private const int DictionaryEntrySize = 24;
        private const int MaxExpectedMemory = NumberOfAdds * DictionaryEntrySize;
        private const int NumberOfAdds = 1000000;

        #endregion

        #region Constructors and Destructors

        public DictionaryMemoryTests(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        #endregion

        #region Public Methods and Operators

        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Test, Description = "Dictionary without capacity, add memory test.")]
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThan, MaxExpectedMemory)]
        [NBenchFact]
        public void AddMemory_FailingTest()
        {
            var dictionary = new Dictionary<int, int>();

            Populate(dictionary, NumberOfAdds);
        }

        [NBenchFact]
        [PerfBenchmark(Description = "AddMemoryMeasurement", RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void AddMemory_Measurement()
        {
            var dictionary = new Dictionary<int, int>();

            Populate(dictionary, NumberOfAdds);
        }


        [PerfBenchmark(Description = "AddMemoryMeasurement_Theory", RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        [NBenchTheory]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(1000000)]
        public void AddMemory_Measurement_Theory(int numberOfAdds)
        {
            var dictionary = new Dictionary<int, int>();

            Populate(dictionary, numberOfAdds);
        }

        [PerfBenchmark(Description = "AddMemory_PassingTest", RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThan, MaxExpectedMemory * 2)]
        [NBenchFact]
        public void AddMemory_PassingTest()
        {
            var dictionary = new Dictionary<int, int>(NumberOfAdds);

            Populate(dictionary, NumberOfAdds);
        }

        public void Populate(Dictionary<int, int> dictionary, int n)
        {
            for (var i = 0; i < n; i++) { dictionary.Add(i, i); }
        }

        #endregion
    }
}