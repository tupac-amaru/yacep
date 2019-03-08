using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Symbols
{
    internal interface IOperator
    {
        string Operator { get; }
    }

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
