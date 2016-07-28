#region Using Directives

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

#endregion

namespace Pro.NBench.xUnit.XunitExtensions
{
    [Serializable]
    public class NBenchTestCase : XunitTestCase
    {
        #region Constructors and Destructors

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", true)]
        public NBenchTestCase()
        {
        }

        public NBenchTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay testMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null)
            : base(diagnosticMessageSink, testMethodDisplay, testMethod, testMethodArguments)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments,
                                                        ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            return
                await
                new NBenchTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, TestMethodArguments, messageBus, aggregator,
                    cancellationTokenSource).RunAsync();
        }

        #endregion
    }
}