using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TupacAmaru.Yacep.Utils
{
    internal static class MethodInfoHelper
    {
        private static readonly ConcurrentDictionary<MethodInfo, Func<object, object[], object>> cache
            = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>(MethodInfoEqualityComparer.Instance);

        private static Func<object, object[], object> BuildCaller(MethodInfo methodInfo)
        {
            Func<object, object[], object> callable;
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameter = Expression.Parameter(typeof(object[]), "parameters");
            var args = methodInfo.GetParameters().Select((p, i) =>
            {
                var conditional = Expression.Condition(
                   Expression.IsTrue(Expression.And(Expression.NotEqual(parameter, Expression.Constant(null)),
                      Expression.GreaterThan(Expression.ArrayLength(parameter), Expression.Constant(i)))),
                     Expression.Convert(Expression.ArrayIndex(parameter, Expression.Constant(i)), p.ParameterType),
                    Expression.Default(p.ParameterType));
                return conditional;
            });
            if (methodInfo.ReturnType == typeof(void))
            {
                var type = methodInfo.IsStatic ? null : methodInfo.DeclaringType;
                var call = type == null ? Expression.Call(methodInfo, args) : Expression.Call(Expression.Convert(instance, type), methodInfo, args);
                var block = Expression.Block(call, Expression.Constant(null));
                var lambda = Expression.Lambda<Func<object, object[], object>>(block, "CallMethod", new[] { instance, parameter });
                callable = lambda.Compile();
            }
            else
            {
                var type = methodInfo.IsStatic ? null : methodInfo.DeclaringType;
                Expression getValue = type == null ? Expression.Call(methodInfo, args) : Expression.Call(Expression.Convert(instance, type), methodInfo, args);
                if (methodInfo.ReturnType.IsValueType)
                    getValue = Expression.Convert(getValue, typeof(object));
                var lambda = Expression.Lambda<Func<object, object[], object>>(getValue, "CallMethod", new[] { instance, parameter });
                callable = lambda.Compile();
            }
            return callable;
        }

        public static Func<object, object[], object> CreateCaller(this MethodInfo methodInfo) => cache.GetOrAdd(methodInfo, BuildCaller);
    }
}
