#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using NBench.Reporting.Targets;
using NBench.Sdk;
using NBench.Sdk.Compiler;

using Pro.NBench.xUnit.NBenchExtensions;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

#endregion

namespace Pro.NBench.xUnit.XunitExtensions
{
    /// <summary>
    ///     The test invoker for xUnit.net v2 tests.
    /// </summary>
    public class NBenchTestInvoker : TestInvoker<IXunitTestCase>
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="XunitTestInvoker" /> class.
        /// </summary>
        /// <param name="test">The test that this invocation belongs to.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="testClass">The test class that the test method belongs to.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <param name="testMethod">The test method that will be invoked.</param>
        /// <param name="testMethodArguments">The arguments to be passed to the test method.</param>
        /// <param name="beforeAfterAttributes">The list of <see cref="BeforeAfterTestAttribute" />s for this test invocation.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public NBenchTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod,
                                 object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator,
                                 CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, aggregator, cancellationTokenSource)
        {
            BeforeAfterAttributes = beforeAfterAttributes;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the list of <see cref="BeforeAfterTestAttribute" />s for this test invocation.
        /// </summary>
        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterAttributes { get; }

        #endregion

        #region Methods

        protected override async Task<decimal> InvokeTestMethodAsync(object testClassInstance)
        {
            var runSummary = await Task.Run(() => RunNBenchTest(testClassInstance));

            return runSummary.Time;
        }

        private RunSummary RunNBenchTest(object testClassInstance)
        {
            //TODO: It is not strictly reuired to use a RunSummary at the moment - needs more investigation to see
            //if we can provide more useful information via the standard xUnit mechanism. For now, what we have is sufficient.
            var summary = new RunSummary();

            var discovery = new ReflectionDiscovery(new ActionBenchmarkOutput(report => { }, results =>
                {
                    Trace.WriteLine("");

                    if (results.AssertionResults.Count > 0)
                    {
                        //TODO: We should determine the accurate elapsed time at this point
                        summary.Time = (decimal)results.Data.StatsByMetric.Values.First().Runs.First().ElapsedSeconds;

                        foreach (var assertion in results.AssertionResults)
                        {
                            //TODO: Maybe it is bubble to bubble this up?
                            Assert.True(assertion.Passed, assertion.Message);

                            //summary.Total++;
                            if (!assertion.Passed) { summary.Failed++; }
                            Trace.WriteLine(assertion.Message);
                            Trace.WriteLine("");
                        }
                    }
                    else
                    {
                        Trace.WriteLine("No assertions returned.");
                    }

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
                    {
                        Trace.WriteLine("No measurements returned.");
                    }
                }));

            var testClassType = TestClass;

            //TODO: At the moment this is performing work that is not required, but is pragmatic in that a change is not required to the NBench core.
            var benchmarkMetaData = ReflectionDiscovery.CreateBenchmarksForClass(testClassType).First(b => b.Run.InvocationMethod.Name == TestMethod.Name);

            var invoker = new XUnitReflectionBenchmarkInvoker(benchmarkMetaData, testClassInstance, TestMethodArguments);
            var settings = discovery.CreateSettingsForBenchmark(benchmarkMetaData);
            var benchmark = new Benchmark(settings, invoker, discovery.Output, discovery.BenchmarkAssertions);

            Benchmark.PrepareForRun();
            benchmark.Run();
            benchmark.Finish();

            return summary;
        }

        #endregion
    }
}