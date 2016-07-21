#region Using Directives

using System.Collections.Generic;
using System.Linq;

using NBench;

#endregion

namespace Pro.NBench.xUnit.Tests
{
    public class GarbageCollectionTests
    { 
        #region Fields

        private readonly List<int[]> _dataCache = new List<int[]>();

        #endregion

        #region Public Methods and Operators

        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void MeasureGarbageCollections()
        {
            RunTest();
        }

        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.LessThan, 300)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.LessThan, 150)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 20)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 50)]
        public void TestGarbageCollections()
        {
            RunTest();
        }

        #endregion

        #region Methods

        private void RunTest()
        {
            for (var i = 0; i < 500; i++)
            {
                for (var j = 0; j < 10000; j++)
                {
                    var data = new int[100];
                    _dataCache.Add(data.ToArray());
                }

                _dataCache.Clear();
            }
        }

        #endregion
    }
}