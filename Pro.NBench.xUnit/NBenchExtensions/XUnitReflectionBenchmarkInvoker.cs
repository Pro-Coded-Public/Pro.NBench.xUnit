// Copyright (c) Petabridge <https://petabridge.com/>. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

#region Using Directives

using System;
using System.Reflection;

using NBench;
using NBench.Sdk;
using NBench.Sdk.Compiler;

#endregion

namespace Pro.NBench.xUnit.NBenchExtensions
{
    /// <summary>
    ///     <see cref="IBenchmarkInvoker" /> implementaton specific to integration with xUnit,, that uses reflection to invoke
    ///     setup / run / cleanup methods
    ///     found on classes decorated with the appropriate <see cref="PerfBenchmarkAttribute" />s.
    /// </summary>
    public class XUnitReflectionBenchmarkInvoker : IBenchmarkInvoker
    {
        #region Fields

        private readonly BenchmarkClassMetadata _metadata;
        private readonly object _testClassInstance;
        private readonly object[] _testMethodArguments;

        private MethodInfo _cleanupAction;
        private MethodInfo _runAction;
        private MethodInfo _setupAction;
        private long _runCount;

        #endregion

        #region Constructors and Destructors

        public XUnitReflectionBenchmarkInvoker(BenchmarkClassMetadata metadata, object testClassInstance, object[] testMethodArguments)
        {
            _metadata = metadata;
            _testClassInstance = testClassInstance;
            _testMethodArguments = testMethodArguments;

            BenchmarkName = $"{metadata.BenchmarkClass.FullName}+{metadata.Run.InvocationMethod.Name}";
        }

        #endregion

        #region Public Properties

        public string BenchmarkName { get; }

        #endregion

        #region Public Methods and Operators

        public void InvokeRun(BenchmarkContext context)
        {
            for (var i = _runCount; i != 0;)
            {
                if (_testMethodArguments.Length > 0)
                {
                    //var args = new object[] { context };
                    //args.me_testMethodArguments }
                    _runAction.Invoke(_testClassInstance, _testMethodArguments );
                }
                else
                {
                    _runAction.Invoke(_testClassInstance, null);
                }
                --i;
            }
        }

        public void InvokePerfCleanup(BenchmarkContext context)
        {
            // cleanup method
            _cleanupAction?.Invoke(_testClassInstance, new object[] { context });

            _cleanupAction = null;
            _setupAction = null;
            _runAction = null;
        }

        public void InvokePerfSetup(BenchmarkContext context)
        {
            if (_testClassInstance == null) { throw new Exception($"{BenchmarkName} : Test Class Instance is null"); }

            _runCount = 1;
            _cleanupAction = ConstructMethod(_metadata.Cleanup);
            _setupAction = ConstructMethod(_metadata.Setup);
            _runAction = ConstructMethod(_metadata.Run);

            _setupAction?.Invoke(_testClassInstance, new object[] { context });
        }

        public void InvokePerfSetup(long runCount, BenchmarkContext context)
        {
            InvokePerfSetup(context);
            _runCount = runCount;
        }

        #endregion

        #region Methods

        internal static Action<BenchmarkContext> CreateDelegateWithContext(object target, MethodInfo invocationMethod)
        {
            var del = (Action<BenchmarkContext>)Delegate.CreateDelegate(typeof(Action<BenchmarkContext>), target, invocationMethod);
            return del;
        }

        internal static Action<BenchmarkContext> CreateDelegateWithoutContext(object target, MethodInfo invocationMethod)
        {
            var del = (Action)Delegate.CreateDelegate(typeof(Action), target, invocationMethod);

            Action<BenchmarkContext> wrappedDelegate = context => del();
            return wrappedDelegate;
        }

        private Action<BenchmarkContext> Compile(BenchmarkMethodMetadata metadata)
        {
            if (metadata.Skip) return ActionBenchmarkInvoker.NoOp;
            return metadata.TakesBenchmarkContext
                       ? CreateDelegateWithContext(_testClassInstance, metadata.InvocationMethod)
                       : CreateDelegateWithoutContext(_testClassInstance, metadata.InvocationMethod);
        }

        private MethodInfo ConstructMethod(BenchmarkMethodMetadata metadata)
        {
            return metadata.Skip ? null : _testClassInstance.GetType().GetMethod(metadata.InvocationMethod.Name);
        }

        #endregion
    }
}