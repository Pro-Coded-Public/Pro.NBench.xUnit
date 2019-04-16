#region Using Directives

using System;
using System.Reflection;

using NBench;
using NBench.Sdk;
using NBench.Sdk.Compiler;

using Pro.NBench.xUnit.DynamicMethodDelegates;

#endregion

namespace Pro.NBench.xUnit.NBenchExtensions
{
    /// <summary>
    ///     <see cref="IBenchmarkInvoker" /> implementation specific to integration with xUnit, that uses reflection to invoke
    ///     setup / run / cleanup methods found on classes decorated with the appropriate <see cref="PerfBenchmarkAttribute" />
    ///     s.
    ///     Works with both xUnit Facts, and Theories.
    /// </summary>
    public class XUnitReflectionBenchmarkInvoker : IBenchmarkInvoker
    {
        #region Static Fields

        private static object[] _testParamaters;

        #endregion

        #region Fields

        private readonly BenchmarkClassMetadata _metadata;
        private readonly object _testClassInstance;

        private Action<BenchmarkContext> _cleanupAction;
        private Action<BenchmarkContext> _runAction;
        private Action<BenchmarkContext> _setupAction;

        #endregion

        #region Constructors and Destructors

        public XUnitReflectionBenchmarkInvoker(BenchmarkClassMetadata metadata, object testClassInstance, object[] testParamaters)
        {
            _metadata = metadata;
            _testClassInstance = testClassInstance;
            _testParamaters = testParamaters;

            BenchmarkName = $"{metadata.BenchmarkClass.FullName}+{metadata.Run.InvocationMethod.Name}";
        }

        #endregion

        #region Public Properties

        public string BenchmarkName { get; }

        #endregion

        #region Public Methods and Operators

        public void InvokePerfCleanup(BenchmarkContext context)
        {
            // cleanup method
            _cleanupAction(context);

            _cleanupAction = null;
            _setupAction = null;
            _runAction = null;
        }

        public void InvokePerfSetup(BenchmarkContext context)
        {
            if (_testClassInstance == null) { throw new Exception($"{BenchmarkName} : Test Class Instance is null"); }

            _cleanupAction = Compile(_metadata.Cleanup);
            _setupAction = Compile(_metadata.Setup);
            _runAction = Compile(_metadata.Run);

            _setupAction(context);
        }

        public void InvokePerfSetup(long runCount, BenchmarkContext context)
        {
            InvokePerfSetup(context);

            var previousRunAction = _runAction;

            _runAction = ctx =>
                {
                    for (var i = runCount; i != 0;)
                    {
                        previousRunAction(ctx);
                        --i;
                    }
                };
        }

        public void InvokeRun(BenchmarkContext context)
        {
            _runAction(context);
        }

        #endregion

        #region Methods

        internal static Action<BenchmarkContext> CreateDelegateWithContext(object target, MethodInfo invocationMethod)
        {
            // As the base method DOES accept a BenchmarkContext as a parameter, we test to see if any other method parameters are required.
            if (invocationMethod.GetParameters().Length == 1)
            {
                // If no additional parameters are required, we create a simple Delegate to the method, that already matches the signature
                // Action<BenchmarkContext>
                var simpleDelegate = (Action<BenchmarkContext>)invocationMethod.CreateDelegate(typeof(Action<BenchmarkContext>), target);
                return simpleDelegate;
            }

            // If additional parameters are required, we use a DynamicMethod to build a Delegate capable of handling 1-n parameters, of any type.
            var paramaterisedDelegate = DynamicMethodDelegateFactory.CreateMethodCaller(invocationMethod);
            // As we require a known Action signature, we wrap the delegate in one that specifies Action<BenchmarkContext>  
            Action<BenchmarkContext> wrappedParamaterisedDelegate = context => paramaterisedDelegate(target, _testParamaters);
            return wrappedParamaterisedDelegate;
        }

        internal static Action<BenchmarkContext> CreateDelegateWithoutContext(object target, MethodInfo invocationMethod)
        {
            // As the base method does not accept a BenchmarkContext as a parameter, and a known Action delegate signature is 
            // required for the sake of performance, we test to see if any method parameters are required.
            if (invocationMethod.GetParameters().Length == 0)
            {
                // If no parameters are required, we create a simple Delegate to the method.
                var simpleDelegate = (Action)invocationMethod.CreateDelegate(typeof(Action), target);
                // As we require a knownAction signature, we wrap the simple delegate in one that specifies Action<BenchmarkContext>                
                Action<BenchmarkContext> wrappedSimpleDelegate = context => simpleDelegate();
                return wrappedSimpleDelegate;
            }

            // If parameters are required, we use a DynamicMethod to build a Delegate capable of handling 1-n parameters, of any type.
            var paramaterisedDelegate = DynamicMethodDelegateFactory.CreateMethodCaller(invocationMethod);
            // Again, as we require a known Action signature, we wrap the delegate in one that specifies Action<BenchmarkContext> 
            Action<BenchmarkContext> wrappedParamaterisedDelegate = context => paramaterisedDelegate(target, _testParamaters);
            return wrappedParamaterisedDelegate;
        }

        private Action<BenchmarkContext> Compile(BenchmarkMethodMetadata metadata)
        {
            if (metadata.Skip) return ActionBenchmarkInvoker.NoOp;
            return metadata.TakesBenchmarkContext
                       ? CreateDelegateWithContext(_testClassInstance, metadata.InvocationMethod)
                       : CreateDelegateWithoutContext(_testClassInstance, metadata.InvocationMethod);
        }

        #endregion
    }
}