#region Using Directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

#endregion

namespace Pro.NBench.xUnit.XunitExtensions
{
    /// <summary>
    ///     The test runner for xUnit.net v2 tests.
    /// </summary>
    public class NBenchTestRunner : TestRunner<IXunitTestCase>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NBenchTestRunner" /> class.
        /// </summary>
        /// <param name="test">The test that this invocation belongs to.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="testClass">The test class that the test method belongs to.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <param name="testMethod">The test method that will be invoked.</param>
        /// <param name="testMethodArguments">The arguments to be passed to the test method.</param>
        /// <param name="skipReason">The skip reason, if the test is to be skipped.</param>
        /// <param name="beforeAfterAttributes">The list of <see cref="BeforeAfterTestAttribute" />s for this test.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public NBenchTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod,
                                object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
                                ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, aggregator, cancellationTokenSource)
        {
            BeforeAfterAttributes = beforeAfterAttributes;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the list of <see cref="BeforeAfterTestAttribute" />s for this test.
        /// </summary>
        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterAttributes { get; }

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var output = string.Empty;

            TestOutputHelper testOutputHelper = null;
            foreach (var obj in ConstructorArguments)
            {
                testOutputHelper = obj as TestOutputHelper;
                if (testOutputHelper != null) break;
            }

            testOutputHelper?.Initialize(MessageBus, Test);

            //TODO: Add execution time, if strictly necessary - the NBench values are certainly more accurate.
            var executionTime = await InvokeTestMethodAsync(aggregator);

            if (testOutputHelper != null)
            {
                output = testOutputHelper.Output;
                testOutputHelper.Uninitialize();
            }

            return Tuple.Create(executionTime, output);
        }

        /// <summary>
        ///     Override this method to invoke the test method.
        /// </summary>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <returns>Returns the execution time (in seconds) spent running the test method.</returns>
        protected virtual Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            return
                new NBenchTestInvoker(Test, MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, BeforeAfterAttributes, aggregator,
                    CancellationTokenSource).RunAsync();
        }

        #endregion
    }
}