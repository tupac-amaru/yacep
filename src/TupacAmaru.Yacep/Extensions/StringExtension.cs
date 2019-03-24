using TupacAmaru.Yacep.Core;
using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep.Extensions
{
    public static class StringExtension
    {
        public static EvaluableExpression ToEvaluableExpression(this string expr, IParser parser, ReadOnlyParseOption option)
            => (parser ?? Parser.Default).Parse(expr, option);
        public static EvaluableExpression ToEvaluableExpression(this string expr, IParser parser)
            => ToEvaluableExpression(expr, parser, null);
        public static EvaluableExpression ToEvaluableExpression(this string expr, ReadOnlyParseOption option)
            => ToEvaluableExpression(expr, null, option);
        public static EvaluableExpression ToEvaluableExpression(this string expr)
            => ToEvaluableExpression(expr, null, null);

        public static IEvaluator Compile(this string expr, IParser parser, ReadOnlyParseOption option, ICompiler compiler = null)
            => expr.ToEvaluableExpression(parser ?? Parser.Default, option).Compile(compiler ?? Compiler.Default);
        public static IEvaluator Compile(this string expr, ReadOnlyParseOption option, ICompiler compiler = null)
            => Compile(expr, null, option, compiler);
        public static IEvaluator Compile(this string expr, IParser parser, ICompiler compiler = null)
            => Compile(expr, parser, null, compiler);
        public static IEvaluator Compile(this string expr, ICompiler compiler = null)
            => Compile(expr, null, null, compiler);


        public static IEvaluator<TState> Compile<TState>(this string expr, IParser parser, ReadOnlyParseOption option, ICompiler compiler = null)
            => expr.ToEvaluableExpression(parser, option).Compile<TState>(compiler ?? Compiler.Default);
        public static IEvaluator<TState> Compile<TState>(this string expr, ICompiler compiler = null)
            => Compile<TState>(expr, null, null, compiler);
        public static IEvaluator<TState> Compile<TState>(this string expr, ReadOnlyParseOption option, ICompiler compiler = null)
            => Compile<TState>(expr, null, option, compiler);
        public static IEvaluator<TState> Compile<TState>(this string expr, IParser parser, ICompiler compiler = null)
            => Compile<TState>(expr, parser, null, compiler);
    }
}