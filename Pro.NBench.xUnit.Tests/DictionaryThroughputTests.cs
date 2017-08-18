#region Using Directives

using System.Collections.Generic;
using System.Diagnostics;

using NBench;

using Pro.NBench.xUnit.XunitExtensions;

using Xunit.Abstractions;

#endregion

namespace Pro.NBench.xUnit.Tests
{
    public class DictionaryThroughputTests
    {
        #region Constants

        private const int AcceptableMinAddThroughput = 20000000;

        private const string AddCounterName = "AddCounter";

        #endregion

        #region Fields

        private readonly Dictionary<int, int> _dictionary = new Dictionary<int, int>();
        private Counter _addCounter;
        private int _key;

        #endregion


        #region Public Methods and Operators

        [NBenchFact]
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(AddCounterName, MustBe.GreaterThan, AcceptableMinAddThroughput)]
        public void AddThroughput_IterationsMode()
        {
            for (var i = 0; i < AcceptableMinAddThroughput; i++)
            {
                _dictionary.Add(i, i);
                _addCounter.Increment();
            }
        }

        [NBenchFact]
        [PerfBenchmark(RunMode = RunMode.Throughput, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(AddCounterName, MustBe.GreaterThan, AcceptableMinAddThroughput)]
        public void AddThroughput_ThroughputMode()
        {
            _dictionary.Add(_key++, _key);
            _addCounter.Increment();
        }

        [PerfCleanup]
        public void Cleanup(BenchmarkContext context)
        {
            _dictionary.Clear();
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _addCounter = context.GetCounter(AddCounterName);
            _key = 0;
        }

        #endregion
    }
}