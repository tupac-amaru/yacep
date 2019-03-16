using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object ObjectMemberExpressionEvaluator(object @object, object member, bool isIndexer);
    public static class ObjectMemberEvaluator
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>> indexerCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Func<object, object, object>>>(TypeEqualityComparer.Instance);
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> cache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>>(TypeEqualityComparer.Instance);
        private static readonly ConcurrentDictionary<Type, Func<object, string, object>> dictionaryGetters
            = new ConcurrentDictionary<Type, Func<object, string, object>>(TypeEqualityComparer.Instance);
        private static Func<object, object> CreateGetterFromField(Type type, FieldInfo field)
        {
            var obj = Expression.Parameter(typeof(object), "obj");
            Expression getValue = Expression.Field(Expression.Convert(obj, type), field);
            var fieldType = field.FieldType;
            if (fieldType.IsValueType)
            {
                getValue = Expression.Convert(getValue, typeof(object));
                return Expression.Lambda<Func<object, object>>(getValue, "ReadObjectField", new[] { obj }).Compile();
            }
            var readObjectField = Expression.Lambda<Func<object, object>>(getValue, "ReadObjectField", new[] { obj }).Compile();
            return readObjectField;
        }
        private static Func<object, object> CreateGetterFromProperty(Type type, MethodInfo getter)
        {
            var obj = Expression.Parameter(typeof(object), "obj");
            Expression getValue = Expression.Call(Expression.Convert(obj, type), getter);
            var propertyType = getter.ReturnType;
            if (propertyType.IsValueType)
            {
                getValue = Expression.Convert(getValue, typeof(object));
                return Expression.Lambda<Func<object, object>>(getValue, "GetObjectProperty", new[] { obj }).Compile();
            }
            var getObjectProperty = Expression.Lambda<Func<object, object>>(getValue, "GetObjectProperty", new[] { obj }).Compile();
            return getObjectProperty;
        }
        private static Func<object, string, object> CreateGetterForDictionary(Type type, Type valueType, MethodInfo getItemMethodInfo)
        {
            var obj = Expression.Parameter(typeof(object), "dict");
            var key = Expression.Parameter(typeof(string), "key");
            Expression getValue = Expression.Call(Expression.Convert(obj, type), getItemMethodInfo, key);
            if (valueType.IsValueType)
            {
                getValue = Expression.Convert(getValue, typeof(object));
                return Expression.Lambda<Func<object, string, object>>(getValue, "GetValueFromDictionary", new[] { obj, key }).Compile();
            }
            var getValueFromDictionary = Expression.Lambda<Func<object, string, object>>(getValue, "GetValueFromDictionary", new[] { obj, key }).Compile();
            return getValueFromDictionary;
        }
        private static Func<object, object> CreateGetter(Type type, string identifier)
        {
            var propertyInfo = type.GetProperty(identifier);
            if (propertyInfo != null && propertyInfo.CanRead)
            {
                var getter = propertyInfo.GetGetMethod();
                if (getter != null)
                {
                    return CreateGetterFromProperty(type, getter);
                }
            }

            var fieldInfo = type.GetField(identifier);
            if (fieldInfo != null && fieldInfo.IsPublic)
            {
                return CreateGetterFromField(type, fieldInfo);
            }

            var methodInfo = type.GetMethod(identifier);
            if (methodInfo != null && methodInfo.IsPublic)
            {
                var callable = methodInfo.CreateCaller();
                return obj => new Func<object[], object>(arguments => callable(obj, arguments));
            }

            throw new MemberNotFoundException(type, identifier);
        }
        private static object Evaluate(object state, Type stateType, string identifier)
        {
            if (dictionaryGetters.TryGetValue(stateType, out var dictionaryReader))
            {
                return dictionaryReader(state, identifier);
            }
            if (cache.TryGetValue(stateType, out var members) && members.TryGetValue(identifier, out var func))
            {
                return func(state);
            }
            var dictionaryType = stateType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (dictionaryType != null)
            {
                var genericArguments = dictionaryType.GetGenericArguments();
                if (genericArguments[0] == typeof(string))
                {
                    var valueType = genericArguments[1];
                    var reader = dictionaryGetters.GetOrAdd(stateType, t =>
                    {
                        var getItemMethodInfo = dictionaryType.GetMethod("get_Item");
                        return CreateGetterForDictionary(t, valueType, getItemMethodInfo);
                    });
                    return reader(state, identifier);
                }
            }
            return cache
                .GetOrAdd(stateType, t => new ConcurrentDictionary<string, Func<object, object>>(StringEqualityComparer.Instance))
                .GetOrAdd(identifier, id => CreateGetter(stateType, id))(state);
        }
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
            return getIndexerValue;
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
                    return Evaluate(@object, @object.GetType(), memberName);
                }
                var type = @object.GetType();
                var indexerType = member.GetType();
                return indexerCache
                    .GetOrAdd(type, t => new ConcurrentDictionary<Type, Func<object, object, object>>())
                    .GetOrAdd(indexerType, it => CreateGetter(type, it))(@object, member);
            }
            else
            {
                var memberName = member as string;
                if (string.IsNullOrWhiteSpace(memberName))
                    throw new UnsupportedEmptyNameMembeException(@object.GetType(), false);
                return Evaluate(@object, @object.GetType(), memberName);
            }
        }
    }
}
