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

        private Action<BenchmarkContext> _cleanupAction;
        private Action<BenchmarkContext> _runAction;
        private Action<BenchmarkContext> _setupAction;

        #endregion

        #region Constructors and Destructors

        public XUnitReflectionBenchmarkInvoker(BenchmarkClassMetadata metadata, object testClassInstance)
        {
            _metadata = metadata;
            _testClassInstance = testClassInstance;

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

        #endregion
    }
}