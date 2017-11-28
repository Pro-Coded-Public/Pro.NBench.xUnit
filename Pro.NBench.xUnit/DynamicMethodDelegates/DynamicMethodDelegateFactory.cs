#region Using Directives

using System;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace Pro.NBench.xUnit.DynamicMethodDelegates
{
    public class DynamicMethodDelegateFactory
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Generates a DynamicMethodDelegate delegate from a MethodInfo object.
        /// </summary>
        public static DynamicMethodDelegate CreateMethodCaller(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var paramaterLength = parameters.Length;

            Type[] argTypes = { typeof(object), typeof(object[]) };

            // Create dynamic method and obtain its IL generator to
            // inject code.

            //DMCQ: this could be typed using Action<,,,> or Func<,,,> with MakeGeneric method
            var dynamicMethod = new DynamicMethod("", typeof(object), argTypes, typeof(DynamicMethodDelegateFactory));
            var ilGenerator = dynamicMethod.GetILGenerator();

            #region IL generation

            #region Argument count check

            // Define a label for succesfull argument count checking.
            var argsOk = ilGenerator.DefineLabel();

            // Check input argument count.
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldlen);
            ilGenerator.Emit(OpCodes.Ldc_I4, paramaterLength);
            ilGenerator.Emit(OpCodes.Beq, argsOk);

            // Argument count was wrong, throw TargetParameterCountException.
            ilGenerator.Emit(OpCodes.Newobj, typeof(TargetParameterCountException).GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Throw);

            // Mark IL with argsOK label.
            ilGenerator.MarkLabel(argsOk);

            #endregion

            #region Instance push

            // If method isn't static push target instance on top
            // of stack.
            if (!methodInfo.IsStatic)
            {
                // Argument 0 of dynamic method is target instance.
                ilGenerator.Emit(OpCodes.Ldarg_0);
            }

            #endregion

            #region Standard argument layout

            // Lay out args array onto stack.
            var i = 0;
            while (i < paramaterLength)
            {
                // Push args array reference onto the stack, followed
                // by the current argument index (i). The Ldelem_Ref opcode
                // will resolve them to args[i].

                // Argument 1 of dynamic method is argument array.
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ldc_I4, i);
                ilGenerator.Emit(OpCodes.Ldelem_Ref);

                //If parameter[i] is a value type perform an unboxing.
                var parameterType = parameters[i].ParameterType;
                if (parameterType.GetTypeInfo().IsValueType) { ilGenerator.Emit(OpCodes.Unbox_Any, parameterType); }

                //OR an alternative suggestion is to unbox all..
                //but this seems to be slower.

                // perform cast
                //var parameterType = parameters[i].ParameterType;
                //ilGenerator.Emit(OpCodes.Unbox_Any, parameterType);

                i++;
            }

            #endregion

            #region Method call

            // Perform actual call.
            // If method is not final a callvirt is required
            // otherwise a normal call will be emitted.
            ilGenerator.Emit(methodInfo.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, methodInfo);

            if (methodInfo.ReturnType != typeof(void))
            {
                // If result is of value type it needs to be boxed
                if (methodInfo.ReturnType.GetTypeInfo().IsValueType) { ilGenerator.Emit(OpCodes.Box, methodInfo.ReturnType); }
            }
            else
            { ilGenerator.Emit(OpCodes.Ldnull); }

            // Emit return opcode.
            ilGenerator.Emit(OpCodes.Ret);

            #endregion

            #endregion

            return (DynamicMethodDelegate)dynamicMethod.CreateDelegate(typeof(DynamicMethodDelegate));
        }

        #endregion
    }
}