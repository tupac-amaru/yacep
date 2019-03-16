using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Symbols
{ 
    public delegate object BinaryOperatorHandler(object leftValue, object rightValue);

    public sealed class BinaryOperator : IOperator
    {
        public BinaryOperator(string @operator, BinaryOperatorHandler handler, uint precedence)
        {
            if (@operator.ContainsSpace())
                throw new CannotContainsSpacesException("Binary operator");
            Operator = @operator;
            Handler = handler;
            Precedence = precedence;
        }
        public uint Precedence { get; }
        public string Operator { get; }
        public BinaryOperatorHandler Handler { get; }
    }
}
