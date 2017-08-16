using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallisticsCore.BaseClasses {

    static class SolversFactory {
        static Lazy<Dictionary<string, Func<WBProjectOptions, WBSolver>>> generDict = new Lazy<Dictionary<string, Func<WBProjectOptions, WBSolver>>>(
            LazyFactory, true);
        static Dictionary<string, Func<WBProjectOptions, WBSolver>> LazyFactory() {

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(ass => ass.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(SolversFactoryAttribute), false).Length > 0)
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(SolverGeneratorMethodAttribute), false).Length > 0)
                .ToDictionary(

                    mi => {
                        var attr = (SolverGeneratorMethodAttribute)mi.GetCustomAttributes(typeof(SolverGeneratorMethodAttribute), false)[0];
                        return attr.Name;
                    },
                    mi => {
                        return DelegateBuilder.BuildDelegate<Func<WBProjectOptions, WBSolver>>(mi);

                    }
                );
        }

        public static WBSolver Get(string name, WBProjectOptions options) {
            //var tt = generDict.Value;
            //var sss = typeof(SolversFactory)
            //    .GetMethods()
            //    .Where(m => m.GetCustomAttributes(typeof(SolverGeneratorAttribute), false).Length > 0)
            //    .ToList();

            //var m1 = sss[0];
            //var attr = (SolverGeneratorAttribute)m1.GetCustomAttributes(typeof(SolverGeneratorAttribute), false)[0];
            //var f = DelegateBuilder.BuildDelegate<Func<WBProjectOptions, WBSolver>>(m1);
            return generDict.Value[name](options);
        }

        public static string[] Variants {
            get {
                return generDict.Value.Keys.ToArray();
            }
        }

        static class DelegateBuilder {
            public static T BuildDelegate<T>(MethodInfo method, params object[] missingParamValues) {
                var queueMissingParams = new Queue<object>(missingParamValues);

                var dgtMi = typeof(T).GetMethod("Invoke");
                var dgtRet = dgtMi.ReturnType;
                var dgtParams = dgtMi.GetParameters();

                var paramsOfDelegate = dgtParams
                    .Select(tp => Expression.Parameter(tp.ParameterType, tp.Name))
                    .ToArray();

                var methodParams = method.GetParameters();

                if (method.IsStatic) {
                    var paramsToPass = methodParams
                        .Select((p, i) => CreateParam(paramsOfDelegate, i, p, queueMissingParams))
                        .ToArray();

                    var expr = Expression.Lambda<T>(
                        Expression.Call(method, paramsToPass),
                        paramsOfDelegate);

                    return expr.Compile();
                } else {
                    var paramThis = Expression.Convert(paramsOfDelegate[0], method.DeclaringType);

                    var paramsToPass = methodParams
                        .Select((p, i) => CreateParam(paramsOfDelegate, i + 1, p, queueMissingParams))
                        .ToArray();

                    var expr = Expression.Lambda<T>(
                        Expression.Call(paramThis, method, paramsToPass),
                        paramsOfDelegate);

                    return expr.Compile();
                }
            }

            private static Expression CreateParam(ParameterExpression[] paramsOfDelegate, int i, ParameterInfo callParamType, Queue<object> queueMissingParams) {
                if (i < paramsOfDelegate.Length)
                    return Expression.Convert(paramsOfDelegate[i], callParamType.ParameterType);

                if (queueMissingParams.Count > 0)
                    return Expression.Constant(queueMissingParams.Dequeue());

                if (callParamType.ParameterType.IsValueType)
                    return Expression.Constant(Activator.CreateInstance(callParamType.ParameterType));

                return Expression.Constant(null);
            }
        }
    }
}
