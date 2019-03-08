using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep
{
    public sealed class ReadOnlyParseOption
    {
        public ReadOnlyParseOption(bool notAllowedArrayExpression, bool notAllowedConditionExpression, bool notAllowedMemberExpression, bool notAllowedIndexerExpression, bool notAllowedInExpression, bool notAllowedConvertUnsignedInteger, IEnumerable<UnaryOperator> unaryOperators, IEnumerable<BinaryOperator> binaryOperators, IEnumerable<LiteralValue> literalValues, IEnumerable<NakedFunction> nakedFunctions)
        {
            NotAllowedArrayExpression = notAllowedArrayExpression;
            NotAllowedConditionExpression = notAllowedConditionExpression;
            NotAllowedMemberExpression = notAllowedMemberExpression;
            NotAllowedIndexerExpression = notAllowedIndexerExpression;
            NotAllowedInExpression = notAllowedInExpression;
            NotAllowedConvertUnsignedInteger = notAllowedConvertUnsignedInteger;
            UnaryOperators = new ReadOnlyCollection<UnaryOperator>(unaryOperators.Select(x => x).ToList());
            BinaryOperators = new ReadOnlyCollection<BinaryOperator>(binaryOperators.Select(x => x).ToList());
            LiteralValues = new ReadOnlyCollection<LiteralValue>(literalValues.Select(x => x).ToList());
            NakedFunctions = new ReadOnlyCollection<NakedFunction>(nakedFunctions.Select(x => x).ToList());
        }

        public bool NotAllowedArrayExpression { get; }
        public bool NotAllowedConditionExpression { get; }
        public bool NotAllowedMemberExpression { get; }
        public bool NotAllowedIndexerExpression { get; }
        public bool NotAllowedInExpression { get; }
        public bool NotAllowedConvertUnsignedInteger { get; }
        public ReadOnlyCollection<UnaryOperator> UnaryOperators { get; }
        public ReadOnlyCollection<BinaryOperator> BinaryOperators { get; }
        public ReadOnlyCollection<LiteralValue> LiteralValues { get; }
        public ReadOnlyCollection<NakedFunction> NakedFunctions { get; }
    }
}