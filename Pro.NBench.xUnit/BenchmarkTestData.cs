#region Using Directives

using NBench.Sdk;

#endregion

namespace Pro.NBench.xUnit
{
    public class BenchmarkTestData
    {
        #region Constructors and Destructors

        public BenchmarkTestData(string name, Benchmark benchmark)
        {
            Name = name;
            Benchmark = benchmark;
        }

        #endregion

        #region Public Properties

        public Benchmark Benchmark { get; }

        public string Name { get; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}