#region Using Directives

using System.Collections.Generic;
using System.Linq;

using Xunit.Abstractions;
using Xunit.Sdk;
#endregion

namespace Pro.NBench.xUnit.XunitExtensions
{
    public class NBenchFactDiscoverer : IXunitTestCaseDiscoverer
    {
        #region Fields

        private readonly IMessageSink _diagnosticMessageSink;

        #endregion

        #region Constructors and Destructors

        public NBenchFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

            #endregion

        #region Public Methods and Operators

        //public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        //{
        //    yield return new NBenchTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        //}

        /// <summary>
/// Creates a single <see cref="XunitTestCase"/> for the given test method.
/// </summary>
        /// <param name="discoveryOptions">The discovery options to be used.</param>
        /// <param name="testMethod">The test method.</param>
        /// <param name="factAttribute">The attribute that decorates the test method.</param>
        /// <returns></returns>
        protected virtual IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions,
                                                        ITestMethod testMethod,
                                                        IAttributeInfo factAttribute)
        {
            return new NBenchTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }

        /// <summary>
        /// Discover test cases from a test method. By default, inspects the test method's argument list to ensure it's
        /// empty, and if not, returns a single <see cref="ExecutionErrorTestCase"/>; otherwise, it returns the result
        /// of calling <see cref="CreateTestCase"/>.
        /// </summary>
        /// <param name="discoveryOptions">The discovery options to be used.</param>
        /// <param name="testMethod">The test method the test cases belong to.</param>
        /// <param name="factAttribute">The fact attribute attached to the test method.</param>
        /// <returns>Returns zero or more test cases represented by the test method.</returns>
        public virtual IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions,
                                                            ITestMethod testMethod,
                                                            IAttributeInfo factAttribute)
        {
            var testCase =
                testMethod.Method.GetParameters().Any()
                ? (new ExecutionErrorTestCase(_diagnosticMessageSink,
                                              TestMethodDisplay.Method,
                                              TestMethodDisplayOptions.None,
                                              testMethod,
                                              "[Fact] methods are not allowed to have parameters. Did you mean to use [Theory]?"))
                : CreateTestCase(discoveryOptions, testMethod, factAttribute);

            return new[] { testCase };
        }
        #endregion
    }
}