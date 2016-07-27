#region Using Directives

using System.Collections.Generic;

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

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            yield return new NBenchTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }

        #endregion
    }
}