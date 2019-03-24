using System.Linq;
using System.Text;
using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep.Core
{
    public sealed class Formatter : IFormatter
    {
        public static readonly IFormatter Default = new Formatter();

        private static void FormatUnaryExpression(int prefixLength, StringBuilder builder, UnaryExpression unaryExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, $"operator:{unaryExpression.UnaryOperator.Operator}"));
            builder.AppendLine(AddPrefix(prefixLength, "argument:"));
            Format(unaryExpression.Argument, prefixLength + count, builder);
        }

        private static void FormatObjectsFunctionCallExpression(int prefixLength, StringBuilder builder, ObjectsFunctionCallExpression objectsFunctionCallExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, "callee:"));
            Format(objectsFunctionCallExpression.Callee, prefixLength + 2 * count, builder);
            builder.AppendLine(AddPrefix(prefixLength, "arguments:"));
            var objectsFunctionArguments = objectsFunctionCallExpression.Arguments;
            for (var i = 0; i < objectsFunctionArguments.Length; i++)
            {
                builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                Format(objectsFunctionArguments[i], prefixLength + 2 * count, builder);
            }
        }

        private static void FormatObjectMemberExpression(int prefixLength, StringBuilder builder, ObjectMemberExpression objectMemberExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, "object:"));
            Format(objectMemberExpression.Object, prefixLength + count, builder);
            builder.AppendLine(AddPrefix(prefixLength, $"member:indexer-{objectMemberExpression.IsIndexer}"));
            Format(objectMemberExpression.Member, prefixLength + count, builder);
        }

        private static void FormatNakedFunctionCallExpression(int prefixLength, StringBuilder builder, NakedFunctionCallExpression nakedFunctionCallExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, $"functionName:{nakedFunctionCallExpression.NakedFunction.Name}"));
            builder.AppendLine(AddPrefix(prefixLength, "arguments:"));
            var nakedFunctionArguments = nakedFunctionCallExpression.Arguments;
            for (var i = 0; i < nakedFunctionArguments.Length; i++)
            {
                builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                Format(nakedFunctionArguments[i], prefixLength + 2 * count, builder);
            }
        }

        private static void FormatLiteralExpression(int prefixLength, StringBuilder builder, LiteralExpression literalExpression)
        {
            builder.AppendLine(literalExpression.LiteralValue.Value == null
                ? AddPrefix(prefixLength, "value: null")
                : AddPrefix(prefixLength,
                    $"value: {literalExpression.LiteralValue.Value}({literalExpression.ValueType.Name})"));
            builder.AppendLine(AddPrefix(prefixLength, $"raw: {literalExpression.LiteralValue.Literal}"));
        }

        private static void FormatInExpression(int prefixLength, StringBuilder builder, InExpression inExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, "value:"));
            Format(inExpression.Value, prefixLength + 2 * count, builder);
            builder.AppendLine(AddPrefix(prefixLength, "values:"));
            var values = inExpression.Values;
            for (var i = 0; i < values.Length; i++)
            {
                builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                Format(values[i], prefixLength + 2 * count, builder);
            }
        }

        private static void FormatConstantExpression(int prefixLength, StringBuilder builder, ConstantExpression constantExpression)
        {
            builder.AppendLine(AddPrefix(prefixLength,
                $"value: {constantExpression.Value}({constantExpression.ValueType.Name})"));
            builder.AppendLine(AddPrefix(prefixLength, $"raw: {constantExpression.Raw}"));
        }

        private static void FormatConditionalExpression(int prefixLength, StringBuilder builder, ConditionalExpression conditionalExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, "condition:"));
            Format(conditionalExpression.Condition, prefixLength + count, builder);
            builder.AppendLine(AddPrefix(prefixLength, "trueValue:"));
            Format(conditionalExpression.ValueIfTrue, prefixLength + count, builder);
            builder.AppendLine(AddPrefix(prefixLength, "falseValue:"));
            Format(conditionalExpression.ValueIfFalse, prefixLength + count, builder);
        }

        private static void FormatArrayExpression(int prefixLength, StringBuilder builder, ArrayExpression arrayExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, "elements:"));
            var elements = arrayExpression.Elements;
            for (var i = 0; i < elements.Length; i++)
            {
                builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                var element = elements[i];
                Format(element, prefixLength + 2 * count, builder);
            }
        }

        private static void FormatBinaryExpression(int prefixLength, StringBuilder builder, BinaryExpression binaryExpression, int count)
        {
            builder.AppendLine(AddPrefix(prefixLength, $"operator: {binaryExpression.BinaryOperator.Operator}"));
            builder.AppendLine(AddPrefix(prefixLength, "left:"));
            Format(binaryExpression.Left, prefixLength + count, builder);
            builder.AppendLine(AddPrefix(prefixLength, "right:"));
            Format(binaryExpression.Right, prefixLength + count, builder);
        }

        private static string AddPrefix(int prefixLength, string value) => $"{(prefixLength > 0 ? string.Join("", Enumerable.Repeat(" ", prefixLength)) : "")}{value}";

        private static void Format(EvaluableExpression expression, int prefixLength, StringBuilder builder)
        {
            const int count = 4;
            builder.AppendLine(AddPrefix(prefixLength, $"type: {expression.TypeName}"));
            switch (expression)
            {
                case ArrayExpression arrayExpression:
                    FormatArrayExpression(prefixLength, builder, arrayExpression, count);
                    break;
                case BinaryExpression binaryExpression:
                    FormatBinaryExpression(prefixLength, builder, binaryExpression, count);
                    break;
                case ConditionalExpression conditionalExpression:
                    FormatConditionalExpression(prefixLength, builder, conditionalExpression, count);
                    break;
                case ConstantExpression constantExpression:
                    FormatConstantExpression(prefixLength, builder, constantExpression);
                    break;
                case IdentifierExpression identifierExpression:
                    builder.AppendLine(AddPrefix(prefixLength, $"name: {identifierExpression.Name}"));
                    break;
                case InExpression inExpression:
                    FormatInExpression(prefixLength, builder, inExpression, count);
                    break;
                case LiteralExpression literalExpression:
                    FormatLiteralExpression(prefixLength, builder, literalExpression);
                    break;
                case NakedFunctionCallExpression nakedFunctionCallExpression:
                    FormatNakedFunctionCallExpression(prefixLength, builder, nakedFunctionCallExpression, count);
                    break;
                case ObjectMemberExpression objectMemberExpression:
                    FormatObjectMemberExpression(prefixLength, builder, objectMemberExpression, count);
                    break;
                case ObjectsFunctionCallExpression objectsFunctionCallExpression:
                    FormatObjectsFunctionCallExpression(prefixLength, builder, objectsFunctionCallExpression, count);
                    break;
                case UnaryExpression unaryExpression:
                    FormatUnaryExpression(prefixLength, builder, unaryExpression, count);
                    break;
            }
        }

        public string Format(EvaluableExpression expression)
        {
            if (expression == null) return "";
            var builder = new StringBuilder();
            Format(expression, 0, builder);
            return builder.ToString();
        }
    }
}