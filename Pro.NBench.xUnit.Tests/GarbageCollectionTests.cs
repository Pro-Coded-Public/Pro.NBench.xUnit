#region Using Directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NBench;

using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;
#endregion

namespace Pro.NBench.xUnit.Tests
{
    public class GarbageCollectionTests
    {
        #region Fields

        private readonly List<int[]> _dataCache = new List<int[]>();

        #endregion

        #region Constructors and Destructors

        public GarbageCollectionTests(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

            #endregion

        #region Methods

        private void RunTest()
        {
            for(var i = 0; i < 500; i++)
            {
                for(var j = 0; j < 10000; j++)
                {
                    var data = new int[100];
                    _dataCache.Add(data.ToArray());
                }

                _dataCache.Clear();
            }
        }

            #endregion

        #region Public Methods and Operators

        [NBenchFact]
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void GarbageCollections_Measurement()
        {
            RunTest();
        }

        [NBenchFact]
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.LessThan, 600)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.LessThan, 300)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 20)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 50)]
        public void GarbageCollections_Test()
        {
            RunTest();
        }
        #endregion
    }
}