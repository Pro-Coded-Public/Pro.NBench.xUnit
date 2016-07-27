#region Using Directives

using System.Threading;
using System.Threading.Tasks;

using Xunit.Sdk;

#endregion

namespace Pro.NBench.xUnit.XunitExtensions
{
    public class NBenchTestCaseRunner : XunitTestCaseRunner
    {
        #region Fields

        private readonly string _displayName;

        private readonly IXunitTestCase _testCase;

        #endregion

        #region Constructors and Destructors

        public NBenchTestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, object[] testMethodArguments,
                                    IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, testMethodArguments, messageBus, aggregator, cancellationTokenSource)
        {
            _testCase = testCase;
            _displayName = displayName;
        }

        #endregion

        #region Methods

        protected override Task<RunSummary> RunTestAsync()
        {
            TestClass = TestCase.TestMethod.TestClass.Class.ToRuntimeType();
            TestMethod = TestCase.TestMethod.Method.ToRuntimeMethod();
            var test = new NBenchTest(_testCase, _displayName);

            return
                new NBenchTestRunner(test, MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, BeforeAfterAttributes,
                    Aggregator, CancellationTokenSource).RunAsync();
        }

        #endregion
    }
}