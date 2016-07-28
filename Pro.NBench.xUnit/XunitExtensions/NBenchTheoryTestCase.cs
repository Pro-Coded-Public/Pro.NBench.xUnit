//using System;
//using System.ComponentModel;
//using System.Threading;
////using System.Threading.Tasks;

////using Xunit.Abstractions;
////using Xunit.Sdk;

////namespace Pro.NBench.xUnit.XunitExtensions
////{
////    /// <summary>
////    /// Represents a test case which runs multiple tests for theory data, either because the
////    /// data was not enumerable or because the data was not serializable.
////    /// </summary>
////    public class NBenchTheoryTestCase : XunitTestCase
////    {
////        /// <summary/>
////        [EditorBrowsable(EditorBrowsableState.Never)]
////        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
////        public NBenchTheoryTestCase() { }

////        /// <summary>
////        /// Initializes a new instance of the <see cref="NBenchTheoryTestCase"/> class.
////        /// </summary>
////        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
////        /// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
////        /// <param name="testMethod">The method under test.</param>
////        public NBenchTheoryTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
////            : base(diagnosticMessageSink, defaultMethodDisplay, testMethod) { }

////        /// <inheritdoc />
////        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
////                                                  IMessageBus messageBus,
////                                                  object[] constructorArguments,
////                                                  ExceptionAggregator aggregator,
////                                                  CancellationTokenSource cancellationTokenSource)
////        {
////            return new XunitTheoryTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource).RunAsync();
////        }
////    }
////}