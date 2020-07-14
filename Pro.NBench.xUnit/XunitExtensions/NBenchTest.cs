#region Using Directives

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
#endregion

namespace Pro.NBench.xUnit.XunitExtensions
{
    public class NBenchTest : LongLivedMarshalByRefObject, ITest
    {
        #region Constructors and Destructors

        public NBenchTest(IXunitTestCase testCase, string displayName)
        {
            TestCase = testCase;
            DisplayName = displayName;
        }

            #endregion

        #region Explicit Interface Properties

        ITestCase ITest.TestCase => TestCase;

        #endregion

        #region Public Properties

        public string DisplayName { get; }

        public IXunitTestCase TestCase { get; }
        #endregion
    }
}