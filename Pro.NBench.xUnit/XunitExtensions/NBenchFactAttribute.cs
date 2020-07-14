#region Using Directives

using Xunit;
using Xunit.Sdk;
#endregion

namespace Pro.NBench.xUnit.XunitExtensions
{
    /// <summary>
    /// Indicates that the current test should be treated as an NBench Benchmark
    /// </summary>
    [XunitTestCaseDiscoverer("Pro.NBench.xUnit.XunitExtensions.NBenchFactDiscoverer", "Pro.NBench.xUnit")]
    public class NBenchFactAttribute : FactAttribute
    {
    }
}