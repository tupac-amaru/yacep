using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep.Core
{
    public sealed partial class Compiler
    {
        private sealed class CachableEvaluator : IEvaluator
        {
            private readonly object instance;
            private readonly Func<object, object, object> execute;
            private object cachedValue;
            private bool executed;

            public CachableEvaluator(object instance, Func<object, object, object> execute)
            {
                this.instance = instance;
                this.execute = execute;
            }
            public object Evaluate(object state)
            {
                if (executed) return cachedValue;
                executed = true;
                cachedValue = execute(instance, state);
                return cachedValue;
            }
        }
        private sealed class Evaluator : IEvaluator
        {
            private readonly object instance;
            private readonly Func<object, object, object> execute;

            public Evaluator(object instance, Func<object, object, object> execute)
            {
                this.instance = instance;
                this.execute = execute;
            }

            public object Evaluate(object state) => execute(instance, state);
        }

        private static void GenerateExecuteMethod(EvaluableExpression expression, TypeBuilder typeBuilder, string methodName, CompileContext compileContext)
        {
            var execute = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard,
                typeof(object), new[] { typeof(object) });
            var executeIl = execute.GetILGenerator();
            GenerateIL(executeIl, expression, compileContext, GenerateNaturalObjectMember);
            executeIl.Emit(OpCodes.Ret);
        }

        private static void GenerateNaturalObjectMember(ILGenerator il, EvaluableExpression expr, CompileContext context)
        {
            switch (expr)
            {
                case IdentifierExpression identifierExpression:
                    context.Cacheable = false;
                    if (string.Equals("this", identifierExpression.Name, StringComparison.Ordinal))
                    {
                        il.Emit(OpCodes.Ldarg_1);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, context.ObjectMemberEvaluator);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldstr, identifierExpression.Name);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Call, Delegates.EvaluateObjectMemberExpression);
                    }

                    break;
                case ObjectMemberExpression objectMemberExpression:
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, context.ObjectMemberEvaluator);
                    GenerateIL(il, objectMemberExpression.Object, context, GenerateNaturalObjectMember);
                    switch (objectMemberExpression.Member)
                    {
                        case IdentifierExpression identifierExpr:
                            il.Emit(OpCodes.Ldstr, identifierExpr.Name);
                            break;
                        case Expressions.ConstantExpression constant when constant.Value is string stringValue:
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
                    break;
                case ObjectsFunctionCallExpression objectsFunctionCallExpression:
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, context.FunctionCaller);
                    GenerateIL(il, objectsFunctionCallExpression.Callee, context, GenerateNaturalObjectMember);
                    GenerateArray(il, objectsFunctionCallExpression.Arguments, context, GenerateNaturalObjectMember);
                    il.Emit(OpCodes.Call, Delegates.CallFunction);
                    break;
            }
        }

        private IEvaluator CreateEvaluator(Type type, MethodInfo execute, CompileContext compileContext)
        {
            var instance = CreateInstance(type, compileContext);
            var ins = Expression.Parameter(typeof(object), "proxy");
            var state = Expression.Parameter(typeof(object), "state");
            var callExecute = Expression.Call(Expression.Convert(ins, type), execute, state);
            var func = Expression.Lambda<Func<object, object, object>>(callExecute, "Evaluate", new[] { ins, state }).Compile();
            return compileContext.Cacheable ? (IEvaluator)new CachableEvaluator(instance, func) : new Evaluator(instance, func);
        }
        public IEvaluator Compile(EvaluableExpression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            var className = $"TupacAmaru.Yacep.DynamicAssembly.Executers.Executer{NameCounter.GetCurrentCount()}";
            var typeBuilder = dynamicModule.DefineType(className, TypeAttributes.Public | TypeAttributes.Sealed);
            var compileContext = new CompileContext(typeBuilder);
            var methodName = "Execute";
            GenerateExecuteMethod(expression, typeBuilder, methodName, compileContext);
            GenerateConstructor(compileContext);

            var workerType = typeBuilder.CreateTypeInfo().AsType();
            return CreateEvaluator(workerType, workerType.GetMethod(methodName), compileContext);
        }
    }
}