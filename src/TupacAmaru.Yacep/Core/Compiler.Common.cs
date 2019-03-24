using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using TupacAmaru.Yacep.Evaluators;
using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Core
{
    public sealed partial class Compiler : ICompiler
    {
        public static readonly ICompiler Default = new Compiler();
        private delegate object ObjectCreator(
            Dictionary<string, object> literals,
            Dictionary<string, UnaryOperatorHandler> unary,
            Dictionary<string, BinaryOperatorHandler> binarys,
            Dictionary<string, NakedFunctionHandler> functions,
            ConditionExpressionEvaluator conditionEvaluator,
            InExpressionEvaluator inEvaluator,
            ObjectMemberExpressionEvaluator objectMemberEvaluator,
            FunctionEvaluator functionEvaluator);

        private readonly ConditionExpressionEvaluator conditionExpressionEvaluator;
        private readonly InExpressionEvaluator inExpressionEvaluator;
        private readonly ObjectMemberExpressionEvaluator objectMemberExpressionEvaluator;
        private readonly FunctionEvaluator functionEvaluator;

        public Compiler() : this(null, null, null, null) { }
        public Compiler(ConditionExpressionEvaluator conditionExpressionEvaluator, InExpressionEvaluator inExpressionEvaluator, ObjectMemberExpressionEvaluator objectMemberExpressionEvaluator, FunctionEvaluator functionEvaluator)
        {
            this.conditionExpressionEvaluator = conditionExpressionEvaluator;
            this.inExpressionEvaluator = inExpressionEvaluator;
            this.objectMemberExpressionEvaluator = objectMemberExpressionEvaluator;
            this.functionEvaluator = functionEvaluator;
        }

        private static readonly ModuleBuilder dynamicModule = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("TupacAmaru.Yacep.DynamicAssembly"), AssemblyBuilderAccess.Run)
            .DefineDynamicModule("<main>");

        private static void SetField(ILGenerator ctorIl, Action<ILGenerator> postion, FieldInfo target)
        {
            ctorIl.Emit(OpCodes.Ldarg_0);
            postion(ctorIl);
            ctorIl.Emit(OpCodes.Stfld, target);
        }
        private static void SetFields<V>(ILGenerator ctorIl, Action<ILGenerator> postion, Conjunction<V> items)
        {
            var getItem = typeof(Dictionary<string, V>).GetMethod("get_Item");
            foreach (var item in items.Fields)
            {
                ctorIl.Emit(OpCodes.Ldarg_0);
                postion(ctorIl);
                ctorIl.Emit(OpCodes.Ldstr, item.Key);
                ctorIl.Emit(OpCodes.Call, getItem);
                ctorIl.Emit(OpCodes.Stfld, item.Value);
            }
        }
        private static FieldBuilder GetOrAddValue<V>(TypeBuilder typeBuilder, Conjunction<V> conjunction, string symbol, Type fieldType, V value)
        {
            if (conjunction.TryGetValue(symbol, out var definedValue)) return definedValue;
            var field = typeBuilder.DefineField($"_f{NameCounter.GetCurrentCount()}", fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
            conjunction.Add(symbol, field, value);
            return field;
        }
        private static void GenerateIntValue(ILGenerator il, int intValue, bool boxValue)
        {
            switch (intValue)
            {
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    il.Emit(intValue < 128 ? OpCodes.Ldc_I4_S : OpCodes.Ldc_I4, intValue);
                    break;
            }
            if (boxValue) il.Emit(OpCodes.Box, typeof(int));
        }
        private static void GenerateValue(ILGenerator il, object value, CompileContext compileContext, string valueName, bool boxValue)
        {
            switch (value)
            {
                case bool boolValue:
                    il.Emit(boolValue ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    if (boxValue) il.Emit(OpCodes.Box, typeof(bool));
                    break;
                case int intValue:
                    GenerateIntValue(il, intValue, boxValue);
                    break;
                case decimal decimalValue:
                    var bits = decimal.GetBits(decimalValue);
                    var sign = (bits[3] & 0x80000000) != 0;
                    int scale = (byte)((bits[3] >> 16) & 0x7f);
                    il.Emit(OpCodes.Ldc_I4, bits[0]);
                    il.Emit(OpCodes.Ldc_I4, bits[1]);
                    il.Emit(OpCodes.Ldc_I4, bits[2]);
                    il.Emit(sign ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ldc_I4, scale);
                    il.Emit(OpCodes.Newobj, Delegates.NewDecimal);
                    if (boxValue) il.Emit(OpCodes.Box, typeof(decimal));
                    break;
                case long longValue:
                    if (longValue < int.MaxValue)
                    {
                        GenerateIntValue(il, (int)longValue, boxValue);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I8, longValue);
                        if (boxValue) il.Emit(OpCodes.Box, typeof(long));
                    }
                    break;
                case string stringValue:
                    il.Emit(OpCodes.Ldstr, stringValue);
                    break;
                default:
                    if (value == null)
                    {
                        il.Emit(OpCodes.Ldnull);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        var field = GetOrAddValue(compileContext.TypeBuilder, compileContext.Literals, valueName, typeof(object), value);
                        il.Emit(OpCodes.Ldfld, field);
                    }
                    break;
            }
        }
        private static void GenerateArray(ILGenerator il, EvaluableExpression[] values, CompileContext compileContext, Action<ILGenerator, EvaluableExpression, CompileContext> objectExpressionGenerater)
        {
            var valuesLength = values.Length;
            il.Emit(OpCodes.Ldc_I4, valuesLength);
            il.Emit(OpCodes.Newarr, typeof(object));
            for (var i = 0; i < valuesLength; i++)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(i < 128 ? OpCodes.Ldc_I4_S : OpCodes.Ldc_I4, i);
                GenerateIL(il, values[i], compileContext, objectExpressionGenerater);
                il.Emit(OpCodes.Stelem_Ref);
            }
        }
        private static void GenerateConstructor(CompileContext compileContext)
        {
            var ctor = compileContext.TypeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, new[]
            {
                typeof(Dictionary<string, object>),
                typeof(Dictionary<string, UnaryOperatorHandler>),
                typeof(Dictionary<string, BinaryOperatorHandler>),
                typeof(Dictionary<string, NakedFunctionHandler>),
                typeof(ConditionExpressionEvaluator),
                typeof(InExpressionEvaluator),
                typeof(ObjectMemberExpressionEvaluator),
                typeof(FunctionEvaluator)
            });
            var ctorIl = ctor.GetILGenerator();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            SetFields(ctorIl, il => il.Emit(OpCodes.Ldarg_1), compileContext.Literals);
            SetFields(ctorIl, il => il.Emit(OpCodes.Ldarg_2), compileContext.Unarys);
            SetFields(ctorIl, il => il.Emit(OpCodes.Ldarg_3), compileContext.Binarys);
            SetFields(ctorIl, il => il.Emit(OpCodes.Ldarg_S, 4), compileContext.Functions);
            if (compileContext.HaveConditionEvaluator)
                SetField(ctorIl, il => il.Emit(OpCodes.Ldarg_S, 5), compileContext.ConditionEvaluator);
            if (compileContext.HaveInEvaluator)
                SetField(ctorIl, il => il.Emit(OpCodes.Ldarg_S, 6), compileContext.InEvaluator);
            if (compileContext.HaveObjectMemberEvaluator)
                SetField(ctorIl, il => il.Emit(OpCodes.Ldarg_S, 7), compileContext.ObjectMemberEvaluator);
            if (compileContext.HaveFunctionCaller)
                SetField(ctorIl, il => il.Emit(OpCodes.Ldarg_S, 8), compileContext.FunctionCaller);
            ctorIl.Emit(OpCodes.Ret);
        }
        private object CreateInstance(Type type, CompileContext compileContext)
        {
            var arguments = new[]
            {
                Expression.Parameter(typeof(Dictionary<string, object>),"literals"),
                Expression.Parameter(typeof(Dictionary<string, UnaryOperatorHandler>),"unarys"),
                Expression.Parameter(typeof(Dictionary<string, BinaryOperatorHandler>),"binarys"),
                Expression.Parameter(typeof(Dictionary<string, NakedFunctionHandler>),"functions"),
                Expression.Parameter(typeof(ConditionExpressionEvaluator),"conditionExpressionEvaluator"),
                Expression.Parameter(typeof(InExpressionEvaluator),"inExpressionEvaluator"),
                Expression.Parameter(typeof(ObjectMemberExpressionEvaluator),"objectMemberExpressionEvaluator"),
                Expression.Parameter(typeof(FunctionEvaluator),"functionEvaluator")
            };
            var creater = Expression.Lambda<ObjectCreator>(Expression.New(type.GetConstructors().First(),
                arguments.Select(x => x as Expression)), "CreateInstance", arguments).Compile();
            return creater(compileContext.Literals.Mappings, compileContext.Unarys.Mappings, compileContext.Binarys.Mappings, compileContext.Functions.Mappings,
                conditionExpressionEvaluator ?? ConditionEvaluator.Evaluate,
                inExpressionEvaluator ?? InEvaluator.Evaluate,
                objectMemberExpressionEvaluator ?? ObjectMemberEvaluator.Evaluate,
                functionEvaluator ?? SimpleFunctionEvaluator.CallFunction);
        }
        private static void GenerateIL(ILGenerator il, EvaluableExpression expression, CompileContext compileContext, Action<ILGenerator, EvaluableExpression, CompileContext> objectExpressionGenerater)
        {
            switch (expression)
            {
                case ArrayExpression arrayExpression:
                    GenerateArray(il, arrayExpression.Elements, compileContext, objectExpressionGenerater);
                    break;
                case Expressions.BinaryExpression binaryExpression:
                    il.Emit(OpCodes.Ldarg_0);
                    var binaryOperator = binaryExpression.BinaryOperator;
                    var binaryOperatorField = GetOrAddValue(compileContext.TypeBuilder, compileContext.Binarys, binaryOperator.Operator, typeof(BinaryOperatorHandler), binaryOperator.Handler);
                    il.Emit(OpCodes.Ldfld, binaryOperatorField);
                    GenerateIL(il, binaryExpression.Left, compileContext, objectExpressionGenerater);
                    GenerateIL(il, binaryExpression.Right, compileContext, objectExpressionGenerater);
                    il.Emit(OpCodes.Call, Delegates.HandleBinaryOperator);
                    break;
                case Expressions.ConstantExpression constantExpression:
                    var constantValueName = $"_c{constantExpression.Raw.GetHashCode()}";
                    GenerateValue(il, constantExpression.Value, compileContext, constantValueName, true);
                    break;
                case Expressions.ConditionalExpression conditionalExpression:
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, compileContext.ConditionEvaluator);
                    GenerateIL(il, conditionalExpression.Condition, compileContext, objectExpressionGenerater);
                    GenerateIL(il, conditionalExpression.ValueIfTrue, compileContext, objectExpressionGenerater);
                    GenerateIL(il, conditionalExpression.ValueIfFalse, compileContext, objectExpressionGenerater);
                    il.Emit(OpCodes.Call, Delegates.EvaluateConditionExpression);
                    break;
                case InExpression inExpression:
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, compileContext.InEvaluator);
                    GenerateIL(il, inExpression.Value, compileContext, objectExpressionGenerater);
                    GenerateArray(il, inExpression.Values, compileContext, objectExpressionGenerater);
                    il.Emit(OpCodes.Call, Delegates.EvaluateInExpression);
                    break;
                case LiteralExpression literalExpression:
                    var literalValue = literalExpression.LiteralValue;
                    var literalValueName = $"_l{literalValue.Literal.GetHashCode()}";
                    GenerateValue(il, literalValue.Value, compileContext, literalValueName, true);
                    break;
                case Expressions.UnaryExpression unaryExpression:
                    il.Emit(OpCodes.Ldarg_0);
                    var unaryOperator = unaryExpression.UnaryOperator;
                    var unaryOperatorField = GetOrAddValue(compileContext.TypeBuilder, compileContext.Unarys, unaryOperator.Operator, typeof(UnaryOperatorHandler), unaryOperator.Handler);
                    il.Emit(OpCodes.Ldfld, unaryOperatorField);
                    GenerateIL(il, unaryExpression.Argument, compileContext, objectExpressionGenerater);
                    il.Emit(OpCodes.Call, Delegates.HandleUnaryOperator);
                    break;
                case NakedFunctionCallExpression nakedFunctionCallExpression:
                    if (!nakedFunctionCallExpression.NakedFunction.Cacheable)
                        compileContext.Cacheable = false;
                    il.Emit(OpCodes.Ldarg_0);
                    var nakedFunction = nakedFunctionCallExpression.NakedFunction;
                    var nakedFunctionField = GetOrAddValue(compileContext.TypeBuilder, compileContext.Functions, nakedFunction.Name, typeof(NakedFunctionHandler), nakedFunction.Handler);
                    il.Emit(OpCodes.Ldfld, nakedFunctionField);
                    GenerateArray(il, nakedFunctionCallExpression.Arguments, compileContext, objectExpressionGenerater);
                    il.Emit(OpCodes.Call, Delegates.HandleNakedFunction);
                    break;
                case IdentifierExpression _:
                case ObjectMemberExpression _:
                case ObjectsFunctionCallExpression _:
                    objectExpressionGenerater(il, expression, compileContext);
                    break;
            }
        }

        private static class NameCounter
        {
            private static long _counter;
            public static long GetCurrentCount()
            {
                long initialValue, computedValue;
                do
                {
                    initialValue = _counter;
                    computedValue = initialValue + 1;
                } while (initialValue != Interlocked.CompareExchange(ref _counter, computedValue, initialValue));
                return computedValue;
            }
        }

        private class Conjunction<V>
        {
            public Dictionary<string, V> Mappings { get; } = new Dictionary<string, V>(); //symbol->handler
            public Dictionary<string, FieldBuilder> Fields { get; } = new Dictionary<string, FieldBuilder>(); //symbol->field

            public bool TryGetValue(string symbol, out FieldBuilder value) => Fields.TryGetValue(symbol, out value);

            public void Add(string key, FieldBuilder field, V value)
            {
                Mappings.Add(key, value);
                Fields.Add(key, field);
            }
        }

        private class CompileContext
        {
            private FieldInfo conditionEvaluator;
            private FieldInfo inEvaluator;
            private FieldInfo objectMemberEvaluator;
            private FieldInfo functionCaller;

            public bool Cacheable { get; set; }
            public TypeBuilder TypeBuilder { get; }
            public FieldInfo ConditionEvaluator
            {
                get
                {
                    if (conditionEvaluator == null)
                    {
                        conditionEvaluator = TypeBuilder.DefineField("_conditionEvaluator", typeof(ConditionExpressionEvaluator), FieldAttributes.Private | FieldAttributes.InitOnly);
                    }
                    return conditionEvaluator;
                }
            }
            public bool HaveConditionEvaluator => conditionEvaluator != null;
            public FieldInfo InEvaluator
            {
                get
                {
                    if (inEvaluator == null)
                    {
                        inEvaluator = TypeBuilder.DefineField("_inEvaluator", typeof(InExpressionEvaluator), FieldAttributes.Private | FieldAttributes.InitOnly);
                    }
                    return inEvaluator;
                }
            }
            public bool HaveInEvaluator => inEvaluator != null;
            public FieldInfo ObjectMemberEvaluator
            {
                get
                {
                    if (objectMemberEvaluator == null)
                    {
                        objectMemberEvaluator = TypeBuilder.DefineField("_objectMemberEvaluator", typeof(ObjectMemberExpressionEvaluator), FieldAttributes.Private | FieldAttributes.InitOnly);
                    }
                    return objectMemberEvaluator;
                }
            }
            public bool HaveObjectMemberEvaluator => objectMemberEvaluator != null;
            public FieldInfo FunctionCaller
            {
                get
                {
                    if (functionCaller == null)
                    {
                        functionCaller = TypeBuilder.DefineField("_functionCaller", typeof(FunctionEvaluator), FieldAttributes.Private | FieldAttributes.InitOnly);
                    }
                    return functionCaller;
                }
            }
            public bool HaveFunctionCaller => functionCaller != null;

            public Conjunction<object> Literals { get; } = new Conjunction<object>();
            public Conjunction<UnaryOperatorHandler> Unarys { get; } = new Conjunction<UnaryOperatorHandler>();
            public Conjunction<BinaryOperatorHandler> Binarys { get; } = new Conjunction<BinaryOperatorHandler>();
            public Conjunction<NakedFunctionHandler> Functions { get; } = new Conjunction<NakedFunctionHandler>();
            public CompileContext(TypeBuilder typeBuilder)
            {
                Cacheable = true;
                TypeBuilder = typeBuilder;
            }
        }

        private static class Delegates
        {
            public static readonly MethodInfo HandleBinaryOperator = typeof(BinaryOperatorHandler).GetMethod("Invoke");
            public static readonly MethodInfo HandleUnaryOperator = typeof(UnaryOperatorHandler).GetMethod("Invoke");
            public static readonly MethodInfo HandleNakedFunction = typeof(NakedFunctionHandler).GetMethod("Invoke");
            public static readonly MethodInfo EvaluateConditionExpression = typeof(ConditionExpressionEvaluator).GetMethod("Invoke");
            public static readonly MethodInfo EvaluateInExpression = typeof(InExpressionEvaluator).GetMethod("Invoke");
            public static readonly MethodInfo EvaluateObjectMemberExpression = typeof(ObjectMemberExpressionEvaluator).GetMethod("Invoke");
            public static readonly MethodInfo CallFunction = typeof(FunctionEvaluator).GetMethod("Invoke");
            public static readonly ConstructorInfo NewDecimal = typeof(decimal).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) });
            public static readonly ConstructorInfo NewObjectArrayArgumentFunc = typeof(Func<object[], object>).GetConstructors().First();
        }
    }
}
