using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using TupacAmaru.Yacep.Expressions;
using ConstantExpression = TupacAmaru.Yacep.Expressions.ConstantExpression;

namespace TupacAmaru.Yacep.Core
{
    public sealed partial class Compiler
    {
        private sealed class CachableEvaluator<TState> : IEvaluator<TState>
        {
            private readonly object instance;
            private readonly Func<object, TState, object> execute;
            private object cachedValue;
            private bool executed;

            public CachableEvaluator(object instance, Func<object, TState, object> execute)
            {
                this.instance = instance;
                this.execute = execute;
            }
            public object Evaluate(TState state)
            {
                if (executed) return cachedValue;
                executed = true;
                cachedValue = execute(instance, state);
                return cachedValue;
            }
        }
        private sealed class Evaluator<TState> : IEvaluator<TState>
        {
            private readonly object instance;
            private readonly Func<object, TState, object> execute;

            public Evaluator(object instance, Func<object, TState, object> execute)
            {
                this.instance = instance;
                this.execute = execute;
            }
            public object Evaluate(TState state) => execute(instance, state);
        }

        private static Action<ILGenerator> GenerateNestedFunc2(Type stateType, MethodInfo methodInfo)
        {
            var className = $"TupacAmaru.Yacep.DynamicAssembly.Functions.Function{NameCounter.GetCurrentCount()}";
            var typeBuilder = dynamicModule.DefineType(className, TypeAttributes.Public | TypeAttributes.Sealed);
            var target = typeBuilder.DefineField("_target", stateType, FieldAttributes.Private | FieldAttributes.InitOnly);
            var ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, CallingConventions.HasThis, new[] { stateType });
            var ctorIl = ctor.GetILGenerator();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Ldarg_1);
            ctorIl.Emit(OpCodes.Stfld, target);
            ctorIl.Emit(OpCodes.Ret);

            var methodName = $"FuncProxy{NameCounter.GetCurrentCount()}";
            var proxyMethod = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, typeof(object), new[] { typeof(object[]) });
            var proxyMethodIl = proxyMethod.GetILGenerator();
            var parameters = methodInfo.GetParameters();
            var num = parameters.Length;
            proxyMethodIl.Emit(OpCodes.Ldarg_0);
            proxyMethodIl.Emit(OpCodes.Ldfld, target);
            for (var i = 0; i < num; i++)
            {
                var parameterInfo = parameters[i];
                var parameterType = parameterInfo.ParameterType;
                proxyMethodIl.Emit(OpCodes.Ldarg_1);
                proxyMethodIl.Emit(OpCodes.Ldc_I4_S , i);
                proxyMethodIl.Emit(OpCodes.Ldelem_Ref);
                proxyMethodIl.Emit(parameterType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, parameterType);
            }
            proxyMethodIl.Emit(OpCodes.Callvirt, methodInfo);
            if (methodInfo.ReturnType == typeof(void))
                proxyMethodIl.Emit(OpCodes.Ldnull);
            else if (methodInfo.ReturnType.IsValueType)
                proxyMethodIl.Emit(OpCodes.Box, methodInfo.ReturnType);
            proxyMethodIl.Emit(OpCodes.Ret);
            var type = typeBuilder.CreateTypeInfo().AsType();
            var ctorInfo = type.GetConstructor(new[] { stateType });
            var proxyMethodInfo = type.GetMethod(methodName);
            return il =>
            {
                il.Emit(OpCodes.Newobj, ctorInfo);
                il.Emit(OpCodes.Ldftn, proxyMethodInfo);
                il.Emit(OpCodes.Newobj, Delegates.NewObjectArrayArgumentFunc);
            };
        }
        private static void TryGenerateIdentifierIL(Type stateType, ILGenerator il, string member, CompileContext compileContext)
        {
            var propertyInfo = stateType.GetProperty(member);
            if (propertyInfo != null && propertyInfo.CanRead)
            {
                var getter = propertyInfo.GetGetMethod();
                if (getter != null)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, getter);
                    var propertyType = propertyInfo.PropertyType;
                    if (propertyType.IsValueType)
                        il.Emit(OpCodes.Box, propertyType);
                    return;
                }
            }

            var fieldInfo = stateType.GetField(member);
            if (fieldInfo != null)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldType);
                return;
            }

            var method = stateType.GetMethod("get_Item", new[] { typeof(string) }) ?? stateType.GetMethod("Get", new[] { typeof(string) })
                         ?? stateType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                                                                                && x.GetGenericArguments()[0] == typeof(string))?.GetMethod("get_Item");
            if (method != null)
            {
                var valueType = method.ReturnType;
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, member);
                il.Emit(OpCodes.Callvirt, method);
                if (valueType.IsValueType)
                    il.Emit(OpCodes.Box, valueType);
                return;
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, compileContext.ObjectMemberEvaluator);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, member);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Call, Delegates.EvaluateObjectMemberExpression);
        }
        private static EvaluableExpression TryGenerateObjectMemberIL(Type stateType, ObjectMemberExpression objectMemberExpression, ILGenerator iLGenerator, CompileContext compileContext)
        {
            var unresolvedExpression = objectMemberExpression;
            var stack = new Stack<(string memberName, object indexerValue)>();
            while (true)
            {
                var target = unresolvedExpression.Object;
                if (target is IdentifierExpression maybeIdentifier)
                {
                    if (unresolvedExpression.IsIndexer)
                    {
                        if (unresolvedExpression.Member is ConstantExpression constantExpression)
                        {
                            stack.Push(("", constantExpression.Value));
                            stack.Push((maybeIdentifier.Name, null));
                            break;
                        }
                        if (unresolvedExpression.Member is LiteralExpression literalExpression &&
                          literalExpression.LiteralValue.Value != null)
                        {
                            stack.Push(("", literalExpression.LiteralValue.Value));
                            stack.Push((maybeIdentifier.Name, null));
                            break;
                        }
                    }
                    else
                    {
                        var identifierExpression = (IdentifierExpression)unresolvedExpression.Member;
                        stack.Push((identifierExpression.Name, null));
                        stack.Push((maybeIdentifier.Name, null));
                        break;
                    }
                    stack.Clear();
                    return objectMemberExpression;
                }
                var member = unresolvedExpression.Member;
                if (unresolvedExpression.IsIndexer)
                {
                    switch (member)
                    {
                        case ConstantExpression constantExpression:
                            stack.Push(("", constantExpression.Value));
                            break;
                        case LiteralExpression literalExpression when literalExpression.LiteralValue.Value != null:
                            stack.Push(("", literalExpression.LiteralValue.Value));
                            break;
                        default:
                            stack.Clear();
                            return objectMemberExpression;
                    }
                }
                else
                {
                    var identifierExpression = (IdentifierExpression)unresolvedExpression.Member;
                    stack.Push((identifierExpression.Name, null));
                }
                if (target is ObjectMemberExpression objectMember)
                {
                    unresolvedExpression = objectMember;
                }
                else
                {
                    stack.Clear();
                    return objectMemberExpression;
                }
            }
            var currentType = stateType;
            var first = stack.Peek();
            if (string.Equals("this", first.memberName, StringComparison.Ordinal))
                stack.Pop();
            var readers = new List<Action<CompileContext, ILGenerator>> { (c, il) => il.Emit(OpCodes.Ldarg_1) };
            while (stack.Count > 0)
            {
                var (memberName, indexerValue) = stack.Pop();
                if (indexerValue != null)
                {
                    var indexerType = indexerValue.GetType();
                    var method = currentType.GetMethod("Get", new[] { indexerType }) ?? currentType.GetMethod("get_Item", new[] { indexerType });
                    if (method != null)
                    {
                        currentType = method.ReturnType;
                        readers.Add((context, il) =>
                        {
                            var valueName = $"_v{NameCounter.GetCurrentCount()}";
                            GenerateValue(il, indexerValue, context, valueName, false);
                            il.Emit(OpCodes.Callvirt, method);
                        });
                        continue;
                    }
                    if (indexerValue is string stringValue)
                        memberName = stringValue;
                    else
                        return objectMemberExpression;
                }
                if (!string.IsNullOrWhiteSpace(memberName))
                {
                    var propertyInfo = currentType.GetProperty(memberName);
                    if (propertyInfo != null && propertyInfo.CanRead)
                    {
                        var getter = propertyInfo.GetGetMethod();
                        if (getter != null)
                        {
                            currentType = propertyInfo.PropertyType;
                            readers.Add((context, il) => il.Emit(OpCodes.Callvirt, getter));
                            continue;
                        }
                    }
                    var fieldInfo = currentType.GetField(memberName);
                    if (fieldInfo != null)
                    {
                        currentType = fieldInfo.FieldType;
                        readers.Add((context, il) => il.Emit(OpCodes.Ldfld, fieldInfo));
                        continue;
                    }
                    var method = currentType.GetMethod("get_Item", new[] { typeof(string) }) ?? currentType.GetMethod("Get", new[] { typeof(string) })
                                 ?? currentType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                                                                                                  && x.GetGenericArguments()[0] == typeof(string))?.GetMethod("get_Item");
                    if (method != null)
                    {
                        currentType = method.ReturnType;
                        readers.Add((context, il) =>
                        {
                            il.Emit(OpCodes.Ldstr, memberName);
                            il.Emit(OpCodes.Callvirt, method);
                        });
                        continue;
                    }

                    var methodInfo = currentType.GetMethod(memberName);
                    if (methodInfo != null)
                    {
                        var type = currentType;
                        readers.Add((context, il) => GenerateNestedFunc2(type, methodInfo)(il));
                        currentType = typeof(Func<object[], object>);
                        continue;
                    }
                }
                return objectMemberExpression;
            }
            foreach (var reader in readers)
                reader(compileContext, iLGenerator);
            if (currentType.IsValueType)
                iLGenerator.Emit(OpCodes.Box, currentType);
            return null;
        }
        private static void GenerateExecuteMethod<TState>(EvaluableExpression expression, TypeBuilder typeBuilder, string methodName, CompileContext compileContext)
        {
            var stateType = typeof(TState);
            var execute = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, typeof(object), new[] { stateType });
            var executeIl = execute.GetILGenerator();
            GenerateExecuterIL(stateType, executeIl, expression, compileContext);
            executeIl.Emit(OpCodes.Ret);
        }
        private static void GenerateExecuterIL(Type stateType, ILGenerator executeIl, EvaluableExpression expression, CompileContext compileContext)
        {
            void generator(ILGenerator il, EvaluableExpression expr, CompileContext context)
            {
                switch (expr)
                {
                    case IdentifierExpression identifierExpression:
                        context.Cacheable = false;
                        if (string.Equals("this", identifierExpression.Name, StringComparison.Ordinal))
                            il.Emit(OpCodes.Ldarg_1);
                        else
                            TryGenerateIdentifierIL(stateType, il, identifierExpression.Name, context);
                        break;
                    case ObjectMemberExpression objectMemberExpression:
                        var unresolvedExpression = TryGenerateObjectMemberIL(stateType, objectMemberExpression, il, context);
                        if (unresolvedExpression != null)
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldfld, context.ObjectMemberEvaluator);
                            GenerateIL(il, objectMemberExpression.Object, context, GenerateNaturalObjectMember);
                            switch (objectMemberExpression.Member)
                            {
                                case IdentifierExpression identifierExpr:
                                    il.Emit(OpCodes.Ldstr, identifierExpr.Name);
                                    break;
                                case ConstantExpression constant when constant.Value is string stringValue:
                                    il.Emit(OpCodes.Ldstr, stringValue);
                                    break;
                                case LiteralExpression literal when literal.LiteralValue.Value is string stringValue:
                                    il.Emit(OpCodes.Ldstr, stringValue);
                                    break;
                                default:
                                    GenerateIL(il, objectMemberExpression.Member, context, GenerateNaturalObjectMember);
                                    break;
                            }
                            il.Emit(objectMemberExpression.IsIndexer ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                            il.Emit(OpCodes.Call, Delegates.EvaluateObjectMemberExpression);
                        }
                        break;
                    case ObjectsFunctionCallExpression objectsFunctionCallExpression:
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, context.FunctionCaller);
                        var unresolved = TryGenerateObjectMemberIL(stateType, objectsFunctionCallExpression.Callee, il, context);
                        if (unresolved != null)
                            GenerateIL(il, objectsFunctionCallExpression.Callee, context, GenerateNaturalObjectMember);
                        GenerateArray(il, objectsFunctionCallExpression.Arguments, context, GenerateNaturalObjectMember);
                        il.Emit(OpCodes.Call, Delegates.CallFunction);
                        break;
                }
            }
            GenerateIL(executeIl, expression, compileContext, generator);
        }

        private IEvaluator<TState> CreateEvaluator<TState>(Type type, MethodInfo execute, CompileContext compileContext)
        {
            var instance = CreateInstance(type, compileContext);
            var ins = Expression.Parameter(typeof(object), "proxy");
            var state = Expression.Parameter(typeof(TState), "state");
            var callExecute = Expression.Call(Expression.Convert(ins, type), execute, state);
            var func = Expression.Lambda<Func<object, TState, object>>(callExecute, "Evaluate", new[] { ins, state }).Compile();
            return compileContext.Cacheable ? (IEvaluator<TState>)new CachableEvaluator<TState>(instance, func) : new Evaluator<TState>(instance, func);
        }

        public IEvaluator<TState> Compile<TState>(EvaluableExpression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            var className = $"TupacAmaru.Yacep.DynamicAssembly.Workers.Worker{NameCounter.GetCurrentCount()}";
            var typeBuilder = dynamicModule.DefineType(className, TypeAttributes.Public | TypeAttributes.Sealed);
            var compileContext = new CompileContext(typeBuilder);

            var methodName = "Execute";
            GenerateExecuteMethod<TState>(expression, typeBuilder, methodName, compileContext);
            GenerateConstructor(compileContext);

            var workerType = typeBuilder.CreateTypeInfo().AsType();
            return CreateEvaluator<TState>(workerType, workerType.GetMethod(methodName), compileContext);
        }
    }
}
