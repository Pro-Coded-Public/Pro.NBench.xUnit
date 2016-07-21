#region Using Directives

using System.Collections.Generic;

using NBench;

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

        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(AddCounterName, MustBe.GreaterThan, AcceptableMinAddThroughput)]
        public void AddThroughput_IterationsMode(BenchmarkContext context)
        {
            for (var i = 0; i < AcceptableMinAddThroughput; i++)
            {
                _dictionary.Add(i, i);
                _addCounter.Increment();
            }
        }

        [PerfBenchmark(RunMode = RunMode.Throughput, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(AddCounterName, MustBe.GreaterThan, AcceptableMinAddThroughput)]
        public void AddThroughput_ThroughputMode(BenchmarkContext context)
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