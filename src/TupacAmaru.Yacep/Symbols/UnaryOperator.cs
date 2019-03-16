using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Symbols
{
    public delegate object UnaryOperatorHandler(object value);

    public sealed class UnaryOperator : IOperator
    {
        public UnaryOperator(string @operator, UnaryOperatorHandler handler)
        {
            if (@operator.ContainsSpace())
                throw new CannotContainsSpacesException("Unary operator");
            Operator = @operator;
            Handler = handler;
        }
        public string Operator { get; }
        public UnaryOperatorHandler Handler { get; }
    }
}
