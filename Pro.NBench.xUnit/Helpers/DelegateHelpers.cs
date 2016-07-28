//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pro.NBench.xUnit.Helpers
//{
//    public static class DelegateHelpers
//    {
//        public static void Test()
//        {
//            MethodInfo tempMethodInfo = new DynamicMethod("CreatDelegate", typeof(string), null);

//            var del = tempMethodInfo.CreateDelegate();

//            del.();
//        }
//        public static Delegate CreateDelegate(this MethodInfo method)
//        {
//            return Delegate.CreateDelegate
//            (
//                Expression.GetDelegateType
//                (
//                    method.GetParameters()
//                        .Select(p => p.ParameterType)
//                        .Concat(new Type[] { method.ReturnType })
//                        .ToArray()
//                ),
//                null,
//                method
//            );
//        }
//    }
//}
