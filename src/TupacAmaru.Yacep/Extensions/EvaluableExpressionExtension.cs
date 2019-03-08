using TupacAmaru.Yacep.Core;
using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep.Extensions
{
    public static class EvaluableExpressionExtension
    {
        public static string ToPrettyString(this EvaluableExpression expr, IFormatter formatter = null)
            => (formatter ?? Formatter.Default).Format(expr);

        public static IEvaluator Compile(this EvaluableExpression expr, ICompiler compiler = null)
            => (compiler ?? Compiler.Default).Compile(expr);
    }
}
