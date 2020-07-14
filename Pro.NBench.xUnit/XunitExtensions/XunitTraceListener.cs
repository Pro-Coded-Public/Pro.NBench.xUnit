using System.Diagnostics;
using Xunit.Abstractions;

namespace Pro.NBench.xUnit.XunitExtensions
{
    public class XunitTraceListener : TraceListener
    {
        #region Fields

        private readonly ITestOutputHelper _output;

        #endregion

        #region Constructors and Destructors

        public XunitTraceListener(ITestOutputHelper output)
        {
            _output = output;
        }

            #endregion

        #region Public Methods and Operators

        public override void Write(string message)
        {
            _output.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            _output.WriteLine(message);
        }
        #endregion
    }
}