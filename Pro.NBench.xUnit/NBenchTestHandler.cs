#region Using Directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NBench.Reporting.Targets;
using NBench.Sdk.Compiler;

using Xunit;

#endregion

namespace Pro.NBench.xUnit
{
    public static class NBenchTestHandler<T>
    {
        #region Public Methods and Operators

        public static IEnumerable<object[]> Benchmarks()
        {
            var discovery = new ReflectionDiscovery(new ActionBenchmarkOutput(report => { }, results =>
                {
                    Trace.WriteLine("");

                    if (results.AssertionResults.Count > 0)
                    {
                        foreach (var assertion in results.AssertionResults)
                        {
                            Assert.True(assertion.Passed, results.BenchmarkName + " " + assertion.Message);
                            Trace.WriteLine(results.BenchmarkName + " " + assertion.Message);
                            Trace.WriteLine("");
                        }
                    }
                    else
                    { Trace.WriteLine("No assertions returned."); }

                    Trace.WriteLine("");
                    Trace.WriteLine("---------- Measurements ----------");
                    Trace.WriteLine("");

                    if (results.Data.StatsByMetric.Count > 0)
                    {
                        foreach (var measurement in results.Data.StatsByMetric)
                        {
                            Trace.WriteLine("Metric : " + measurement.Key.ToHumanFriendlyString());
                            Trace.WriteLine("");
                            Trace.WriteLine($"     Per Second ( {measurement.Value.Unit} )");

                            Trace.WriteLine($"        Average : {measurement.Value.PerSecondStats.Average}");
                            Trace.WriteLine($"            Max : {measurement.Value.PerSecondStats.Max}");
                            Trace.WriteLine($"            Min : {measurement.Value.PerSecondStats.Min}");
                            Trace.WriteLine($" Std. Deviation : {measurement.Value.PerSecondStats.StandardDeviation}");
                            Trace.WriteLine($"     Std. Error : {measurement.Value.PerSecondStats.StandardError}");
                            Trace.WriteLine("");

                            Trace.WriteLine($"      Per Test ( {measurement.Value.Unit} )");
                            Trace.WriteLine($"       Average : {measurement.Value.Stats.Average}");
                            Trace.WriteLine($"           Max : {measurement.Value.Stats.Max}");
                            Trace.WriteLine($"           Min : {measurement.Value.Stats.Min}");
                            Trace.WriteLine($"Std. Deviation : {measurement.Value.Stats.StandardDeviation}");
                            Trace.WriteLine($"    Std. Error : {measurement.Value.Stats.StandardError}");

                            Trace.WriteLine("");
                            Trace.WriteLine("----------");
                            Trace.WriteLine("");
                        }
                    }
                    else
                    { Trace.WriteLine("No measurements returned."); }
                }));

            var benchmarks = discovery.FindBenchmarks(typeof(T)).ToList();

            foreach (var benchmark in benchmarks)
            {
                var name = benchmark.BenchmarkName.Split('+')[1];

                yield return new object[] { new BenchmarkTestData(name, benchmark) };
            }
        }

        #endregion
    }
}