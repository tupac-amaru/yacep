using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object IdentifierExpressionEvaluator(object state, Type stateType, string identifier);
    public static class IdentifierEvaluator
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> cache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>>();
        private static readonly ConcurrentDictionary<Type, Func<object, string, object>> dictionaryGetters
            = new ConcurrentDictionary<Type, Func<object, string, object>>();
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
            if (fieldType == typeof(object))
            {
                return ob =>
                {
                    var fieldValue = readObjectField(ob);
                    if (fieldValue is Delegate @delegate)
                        return new Func<object[], object>(args => @delegate.Method.CreateCaller()(@delegate.Target, args));
                    return fieldValue;
                };
            }
            if (fieldType.IsSubclassOf(typeof(Delegate)))
            {
                return ob =>
                {
                    var @delegate = (Delegate)readObjectField(ob);
                    return new Func<object[], object>(args => @delegate.Method.CreateCaller()(@delegate.Target, args));
                };
            }
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
            if (propertyType == typeof(object))
            {
                return ob =>
                {
                    var propertyValue = getObjectProperty(ob);
                    if (propertyValue is Delegate @delegate)
                        return new Func<object[], object>(args => @delegate.Method.CreateCaller()(@delegate.Target, args));
                    return propertyValue;
                };
            }
            if (propertyType.IsSubclassOf(typeof(Delegate)))
            {
                return ob =>
                {
                    var @delegate = (Delegate)getObjectProperty(ob);
                    return new Func<object[], object>(args => @delegate.Method.CreateCaller()(@delegate.Target, args));
                };
            }
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
            return (o, s) =>
            {
                var value = getValueFromDictionary(o, s);
                if (value is Delegate @delegate)
                    return new Func<object[], object>(args => @delegate.Method.CreateCaller()(@delegate.Target, args));
                return value;
            };
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
        public static object Evaluate(object state, Type stateType, string identifier)
        {
            if (state == null) return null;
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }
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
                .GetOrAdd(stateType, t => new ConcurrentDictionary<string, Func<object, object>>())
                .GetOrAdd(identifier, id => CreateGetter(stateType, id))(state);
        }
    }
}
