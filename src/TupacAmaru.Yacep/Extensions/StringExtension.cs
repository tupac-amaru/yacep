using TupacAmaru.Yacep.Core;
using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep.Extensions
{
    public static class StringExtension
    {
        public static EvaluableExpression ToEvaluableExpression(this string expr, IParser parser, ReadOnlyParseOption option)
            => (parser ?? Parser.Default).Parse(expr, option);
        public static EvaluableExpression ToEvaluableExpression(this string expr, IParser parser)
            => expr.ToEvaluableExpression(parser, null);
        public static EvaluableExpression ToEvaluableExpression(this string expr, ReadOnlyParseOption option)
            => expr.ToEvaluableExpression(null, option);
        public static EvaluableExpression ToEvaluableExpression(this string expr)
            => expr.ToEvaluableExpression(null, null);

        public static IEvaluator Compile(this string expr, ICompiler compiler = null)
            => (compiler ?? Compiler.Default).Compile(expr.ToEvaluableExpression());
        public static IEvaluator Compile(this string expr, ReadOnlyParseOption option, ICompiler compiler = null)
            => expr.ToEvaluableExpression(option).Compile(compiler);
        public static IEvaluator Compile(this string expr, IParser parser, ICompiler compiler = null)
            => expr.ToEvaluableExpression(parser).Compile(compiler);
        public static IEvaluator Compile(this string expr, IParser parser, ReadOnlyParseOption option, ICompiler compiler = null)
            => expr.ToEvaluableExpression(parser, option).Compile(compiler);
    }
}