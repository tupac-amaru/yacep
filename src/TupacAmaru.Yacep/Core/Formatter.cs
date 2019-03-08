using System.Linq;
using System.Text;
using TupacAmaru.Yacep.Expressions;

namespace TupacAmaru.Yacep.Core
{
    public sealed class Formatter : IFormatter
    {
        public static readonly IFormatter Default = new Formatter();

        private static string AddPrefix(int prefixLength, string value) => $"{(prefixLength > 0 ? string.Join("", Enumerable.Repeat(" ", prefixLength)) : "")}{value}";
        private static void Format(EvaluableExpression expression, int prefixLength, StringBuilder builder)
        {
            const int count = 4;
            builder.AppendLine(AddPrefix(prefixLength, $"type: {expression.TypeName}"));
            switch (expression)
            {
                case ArrayExpression arrayExpression:
                    builder.AppendLine(AddPrefix(prefixLength, "elements:"));
                    var elements = arrayExpression.Elements;
                    for (var i = 0; i < elements.Length; i++)
                    {
                        builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                        var element = elements[i];
                        Format(element, prefixLength + 2 * count, builder);
                    }
                    break;
                case BinaryExpression binaryExpression:
                    builder.AppendLine(AddPrefix(prefixLength, $"operator: {binaryExpression.BinaryOperator.Operator}"));
                    builder.AppendLine(AddPrefix(prefixLength, "left:"));
                    Format(binaryExpression.Left, prefixLength + count, builder);
                    builder.AppendLine(AddPrefix(prefixLength, "right:"));
                    Format(binaryExpression.Right, prefixLength + count, builder);
                    break;
                case ConditionalExpression conditionalExpression:
                    builder.AppendLine(AddPrefix(prefixLength, "condition:"));
                    Format(conditionalExpression.Condition, prefixLength + count, builder);
                    builder.AppendLine(AddPrefix(prefixLength, "trueValue:"));
                    Format(conditionalExpression.ValueIfTrue, prefixLength + count, builder);
                    builder.AppendLine(AddPrefix(prefixLength, "falseValue:"));
                    Format(conditionalExpression.ValueIfFalse, prefixLength + count, builder);
                    break;
                case ConstantExpression constantExpression:
                    builder.AppendLine(AddPrefix(prefixLength,
                        $"value: {constantExpression.Value}({constantExpression.ValueType.Name})"));
                    builder.AppendLine(AddPrefix(prefixLength, $"raw: {constantExpression.Raw}"));
                    break;
                case IdentifierExpression identifierExpression:
                    builder.AppendLine(AddPrefix(prefixLength, $"name: {identifierExpression.Name}"));
                    break;
                case InExpression inExpression:
                    builder.AppendLine(AddPrefix(prefixLength, "value:"));
                    Format(inExpression.Value, prefixLength + 2 * count, builder);
                    builder.AppendLine(AddPrefix(prefixLength, "values:"));
                    var values = inExpression.Values;
                    for (var i = 0; i < values.Length; i++)
                    {
                        builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                        Format(values[i], prefixLength + 2 * count, builder);
                    }
                    break;
                case LiteralExpression literalExpression:
                    builder.AppendLine(literalExpression.LiteralValue.Value == null ?
                        AddPrefix(prefixLength, "value: null") :
                        AddPrefix(prefixLength,
                            $"value: {literalExpression.LiteralValue.Value}({literalExpression.ValueType.Name})"));
                    builder.AppendLine(AddPrefix(prefixLength, $"raw: {literalExpression.LiteralValue.Literal}"));
                    break;
                case NakedFunctionCallExpression nakedFunctionCallExpression:
                    builder.AppendLine(AddPrefix(prefixLength, $"functionName:{nakedFunctionCallExpression.NakedFunction.Name}"));
                    builder.AppendLine(AddPrefix(prefixLength, "arguments:"));
                    var nakedFunctionArguments = nakedFunctionCallExpression.Arguments;
                    for (var i = 0; i < nakedFunctionArguments.Length; i++)
                    {
                        builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                        Format(nakedFunctionArguments[i], prefixLength + 2 * count, builder);
                    }
                    break;
                case ObjectMemberExpression objectMemberExpression:
                    builder.AppendLine(AddPrefix(prefixLength, "object:"));
                    Format(objectMemberExpression.Object, prefixLength + count, builder);
                    builder.AppendLine(AddPrefix(prefixLength, $"isIndexer:{objectMemberExpression.IsIndexer}"));
                    Format(objectMemberExpression.Member, prefixLength + count, builder);
                    break;
                case ObjectsFunctionCallExpression objectsFunctionCallExpression:
                    builder.AppendLine(AddPrefix(prefixLength, "callee:"));
                    Format(objectsFunctionCallExpression.Callee, prefixLength + 2 * count, builder);
                    builder.AppendLine(AddPrefix(prefixLength, "arguments:"));
                    var objectsFunctionArguments = objectsFunctionCallExpression.Arguments;
                    for (var i = 0; i < objectsFunctionArguments.Length; i++)
                    {
                        builder.AppendLine(AddPrefix(prefixLength + count, $"{i}:"));
                        Format(objectsFunctionArguments[i], prefixLength + 2 * count, builder);
                    }
                    break;
                case UnaryExpression unaryExpression:
                    builder.AppendLine(AddPrefix(prefixLength, $"operator:{unaryExpression.UnaryOperator.Operator}"));
                    builder.AppendLine(AddPrefix(prefixLength, "argument:"));
                    Format(unaryExpression.Argument, prefixLength + count, builder);
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