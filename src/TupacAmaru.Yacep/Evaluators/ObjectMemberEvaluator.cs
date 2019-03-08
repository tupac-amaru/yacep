using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object ObjectMemberExpressionEvaluator(object @object, object member, bool isIndexer);
    public static class ObjectMemberEvaluator
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> cache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>>();

        private static Func<object, object, object> CreateGetter(Type type, Type indexerType)
        {
            var method = type.GetMethod("Get", new[] { indexerType }) ?? type.GetMethod("get_Item", new[] { indexerType });
            if (method == null)
            {
                throw new UnsupportedObjectIndexerException(type, indexerType);
            }
            var obj = Expression.Parameter(typeof(object), "collection");
            var value = Expression.Parameter(typeof(object), "indexerValue");
            Expression getValue = Expression.Call(Expression.Convert(obj, type), method, Expression.Convert(value, indexerType));
            var valueType = method.ReturnType;
            if (valueType.IsValueType)
            {
                getValue = Expression.Convert(getValue, typeof(object));
                return Expression.Lambda<Func<object, object, object>>(getValue, "GetValue", new[] { obj, value }).Compile();
            }
            var getIndexerValue = Expression.Lambda<Func<object, object, object>>(getValue, "GetValue", new[] { obj, value }).Compile();
            return (o, s) =>
            {
                var indexerValue = getIndexerValue(o, s);
                if (indexerValue is Delegate @delegate)
                    return new Func<object[], object>(args => @delegate.Method.CreateCaller()(@delegate.Target, args));
                return indexerValue;
            };
        }

        public static object Evaluate(object @object, object member, bool isIndexer)
        {
            if (@object == null) return null;
            if (member == null)
            {
                throw new UnsupportedNullValueIndexerException(@object.GetType());
            }
            if (isIndexer)
            {
                if (member is string memberName)
                {
                    if (string.IsNullOrWhiteSpace(memberName))
                    {
                        throw new UnsupportedEmptyNameMembeException(@object.GetType(), true);
                    }
                    return IdentifierEvaluator.Evaluate(@object, @object.GetType(), memberName);
                }
                var type = @object.GetType();
                var indexerType = member.GetType();
                return cache
                    .GetOrAdd(type, t => new ConcurrentDictionary<Type, Func<object, object, object>>())
                    .GetOrAdd(indexerType, it => CreateGetter(type, it))(@object, member);
            }
            else
            {
                var memberName = member as string;
                if (string.IsNullOrWhiteSpace(memberName))
                    throw new UnsupportedEmptyNameMembeException(@object.GetType(), false);
                return IdentifierEvaluator.Evaluate(@object, @object.GetType(), memberName);
            }
        }
    }
}
