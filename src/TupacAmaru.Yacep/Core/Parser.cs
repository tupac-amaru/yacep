using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Core
{
    public sealed class Parser : IParser
    {
        public static readonly IParser Default = new Parser();
        private static readonly ReadOnlyParseOption defaultParseOption = ParseOption.CreateOption().AsReadOnly();
        private readonly ref struct SourceExpression
        {
            private readonly ReadOnlySpan<char> readOnlySpan;
            private readonly int sourceExpressionLength;
            public SourceExpression(string expr)
            {
                sourceExpressionLength = expr.Length;
                readOnlySpan = expr.AsSpan();
            }
            private ReadOnlySpan<char> Slice(int start, int length) => readOnlySpan.Slice(start, length);
            [Pure]
            public bool IsInBounds(int index) => index < sourceExpressionLength;
            [Pure]
            public bool SectionEqual(int start, int length, string value)
            {
                var end = start + length;
                if (!IsInBounds(end - 1)) return false;
                for (var i = start; i < end; i++)
                {
                    if (value[i - start] != this[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            [Pure]
            public unsafe (string raw, object value) ParseNumber(bool isDecimal, int start, int end, bool notAllowedConvertUnsignedInteger)
            {
                var len = end - start;
                fixed (char* chars = &Slice(start, len).GetPinnableReference())
                {
                    var bytes = stackalloc byte[len];
                    Encoding.UTF8.GetBytes(chars, len, bytes, len);
                    var buffer = new ReadOnlySpan<byte>(bytes, len);
                    if (isDecimal)
                    {
                        var success = Utf8Parser.TryParse(buffer, out decimal decimalValue, out _);
                        if (!success)
                            throw new InvalidCastException($"Can not parse value({this[start, len]}) to decimal at (start:{start},end:{end})");
                        return (this[start, len], decimalValue);
                    }
                    else
                    {
                        var success = Utf8Parser.TryParse(buffer, out ulong ulongValue, out _);
                        if (!success)
                            throw new InvalidCastException($"Can not parse value({this[start, len]}) to ulong at (start:{start},end:{end})");
                        if (notAllowedConvertUnsignedInteger)
                            return (this[start, len], ulongValue);
                        if (ulongValue <= int.MaxValue)
                            return (this[start, len], (int)ulongValue);
                        return ulongValue <= uint.MaxValue ? (this[start, len], (uint)ulongValue) : (this[start, len], ulongValue);
                    }
                }
            }
            [Pure]
            public char this[int idx] => IsInBounds(idx) ? readOnlySpan[idx] : '\0';
            [Pure]
            public string this[int idx, int len] => !IsInBounds(idx + len - 1) ? string.Empty : new string(Slice(idx, len).ToArray());
        }

        private ref struct ExpressionParser
        {
            private static readonly char period = '.';
            private static readonly char comma = ',';
            private static readonly char singleQuote = '\'';
            private static readonly char doubleQuote = '"';
            private static readonly char openParen = '(';
            private static readonly char closeParen = ')';
            private static readonly char openSquareBracket = '[';
            private static readonly char closeSquareBracket = ']';
            private static readonly char questionMark = '?';
            private static readonly char colon = ':';
            private readonly SourceExpression se;
            private readonly ReadOnlyParseOption option;
            private readonly int maxUnaryOperatorLength;
            private readonly int maxBinaryOperatorLength;
            private int index;
            public ExpressionParser(SourceExpression sourceExpression, ReadOnlyParseOption option)
            {
                index = 0;
                se = sourceExpression;
                this.option = option;
                maxUnaryOperatorLength = (from item in option.UnaryOperators select item.Operator.Length).Max(x => x);
                maxBinaryOperatorLength = (from item in option.BinaryOperators select item.Operator.Length).Max(x => x);
            }
            private static bool IsIdentifierStart(char chr) => chr == '$' || chr == '_' || char.IsLetter(chr);
            private static bool IsIdentifierPart(char chr) => IsIdentifierStart(chr) || char.IsDigit(chr);

            private bool TryFindOperator<T>(ReadOnlyCollection<T> operators, int maxLength, out T @operator, out int start)
            where T : class, IOperator
            {
                SkipSpaces();
                var chr = se[index];
                start = index;
                var checkLength = maxLength;
                while (true)
                {
                    var length = checkLength;
                    foreach (var item in operators.Where(x => x.Operator.Length == length).Reverse())
                    {
                        var canFindOperatorInExpression = se.SectionEqual(index, checkLength, item.Operator);
                        var isFindedExpressionOperator = canFindOperatorInExpression &&
                            (!IsIdentifierStart(chr) || (se.IsInBounds(index + checkLength) && !IsIdentifierPart(se[index + checkLength])));
                        if (isFindedExpressionOperator)
                        {
                            index += checkLength;
                            @operator = item;
                            return true;
                        }
                    }
                    --checkLength;
                    if (checkLength == 0) break;
                }
                @operator = null;
                return false;
            }
            private void SkipSpaces()
            {
                var chr = se[index];
                while (chr.IsSpace())
                    chr = se[++index];
            }
            private bool TryFindBinaryOperator(out BinaryOperator @operator, out int start)
                => TryFindOperator(option.BinaryOperators, maxBinaryOperatorLength, out @operator, out start);
            private bool TryFindUnaryOperator(out UnaryOperator @operator, out int start)
                => TryFindOperator(option.UnaryOperators, maxUnaryOperatorLength, out @operator, out start);
            private ConstantExpression GetNumericLiteral()
            {
                var start = index;
                var chr = se[index];
                var isDecimal = false;
                while (char.IsDigit(chr))
                {
                    chr = se[++index];
                }
                if (chr == period)
                {
                    isDecimal = true;
                    chr = se[++index];
                    while (char.IsDigit(chr))
                    {
                        chr = se[++index];
                    }
                }
                if (chr == 'E' || chr == 'e')
                {
                    isDecimal = true;
                    chr = se[++index];
                    if (chr == '+' || chr == '-')
                    {
                        chr = se[++index];
                    }
                    while (char.IsDigit(chr))
                    {
                        chr = se[++index];
                    }
                    if (!char.IsDigit(se[index - 1]))
                    {
                        throw new ParseException($"Expected exponent ({se[start, index - start]}) at character {index  }");
                    }
                }
                if (IsIdentifierStart(chr))
                {
                    throw new ParseException($"Variable names cannot start with a number ({se[start, index - start + 1]}) at character {index  }");
                }
                if (chr == period)
                {
                    throw new ParseException($"Unexpected period at character {index}");
                }
                var (raw, value) = se.ParseNumber(isDecimal, start, index, option.NotAllowedConvertUnsignedInteger);
                return new ConstantExpression(raw, value, start, index);
            }
            private ConstantExpression GetStringLiteral()
            {
                var buffer = new List<char>();
                var quote = se[index++];
                var close = false;
                var start = index;
                while (se.IsInBounds(index))
                {
                    var chr = se[index++];
                    if (chr == quote)
                    {
                        close = true;
                        break;
                    }
                    if (chr == '\\')
                    {
                        if (se.IsInBounds(index))
                        {
                            chr = se[index++];
                            switch (chr)
                            {
                                case 'n':
                                    buffer.Add('\n');
                                    break;
                                case 'r':
                                    buffer.Add('\r');
                                    break;
                                case 't':
                                    buffer.Add('\t');
                                    break;
                                case 'b':
                                    buffer.Add('\b');
                                    break;
                                case 'f':
                                    buffer.Add('\f');
                                    break;
                                case 'v':
                                    buffer.Add('\v');
                                    break;
                                default:
                                    buffer.Add('\\');
                                    buffer.Add(chr);
                                    break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        buffer.Add(chr);
                    }
                }
                if (!close)
                {
                    throw new ParseException($"Unclosed quote after \"{new string(buffer.ToArray())}\" at character {index}");
                }
                var value = new string(buffer.ToArray());
                return new ConstantExpression(value, value, start, index);
            }
            private EvaluableExpression GetIdentifier()
            {
                var chr = se[index];
                var start = index;
                if (IsIdentifierStart(chr))
                {
                    index++;
                }
                else
                {
                    throw new ParseException($"Unexpected {chr} at character {index}");
                }
                while (se.IsInBounds(index))
                {
                    chr = se[index];
                    if (IsIdentifierPart(chr))
                    {
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
                var literal = se[start, index - start];
                var value = option.LiteralValues.FirstOrDefault(x => x.Literal == literal);
                return value == null
                    ? (EvaluableExpression)new IdentifierExpression(literal, start, index)
                    : new LiteralExpression(value, start, index);
            }
            private EvaluableExpression[] GetArguments(char termination)
            {
                var list = new List<EvaluableExpression>();
                var closed = false;
                var separatorCount = 0;
                while (se.IsInBounds(index))
                {
                    SkipSpaces();
                    var chr = se[index];
                    if (chr == termination)
                    {
                        closed = true;
                        index++;
                        if (termination == closeParen && separatorCount > 0 && separatorCount >= list.Count)
                        {
                            throw new ParseException($"Unexpected token {termination} at character {index}");
                        }
                        break;
                    }
                    if (chr == comma)
                    {
                        index++;
                        separatorCount++;
                        if (separatorCount != list.Count)
                        {
                            throw new ParseException($"Unexpected token , at character {index}");
                        }
                    }
                    else
                    {
                        var expr = GetExpression();
                        if (expr == null)
                        {
                            throw new ParseException($"Expected , at character {index}");
                        }
                        list.Add(expr);
                    }
                }
                if (!closed)
                {
                    throw new ParseException($"Expected {termination} at character {index}");
                }
                return list.ToArray();
            }
            private EvaluableExpression GetExpression()
            {
                var start = index;
                var condition = GetBinaryExpression();
                SkipSpaces();
                var chr = se[index];
                if (chr == questionMark)
                {
                    if (option.NotAllowedConditionExpression)
                    {
                        throw new ParseException($"Condition expression is not allowed at character {start}");
                    }
                    index++;
                    var valueIfTrue = GetExpression();
                    if (valueIfTrue == null)
                    {
                        throw new ParseException($"Expected expression at character {index}");
                    }
                    SkipSpaces();
                    chr = se[index];
                    if (chr == colon)
                    {
                        index++;
                        var valueIfFalse = GetExpression();
                        if (valueIfFalse == null)
                        {
                            throw new ParseException($"Expected expression at character {index}");
                        }
                        return new ConditionalExpression(condition, valueIfTrue, valueIfFalse, start, index);
                    }
                    throw new ParseException($"Expected : at character {index}");
                }
                return condition;
            }
            private EvaluableExpression GetGroup()
            {
                index++;
                var expr = GetExpression();
                SkipSpaces();
                if (se[index] == closeParen)
                {
                    index++;
                    return expr;
                }
                throw new ParseException($"Unclosed ( at character {index}");
            }
            private ArrayExpression GetArray()
            {
                if (option.NotAllowedArrayExpression)
                {
                    throw new ParseException($"Array expression is not allowed at character {index}");
                }
                var start = index++;
                return new ArrayExpression(GetArguments(closeSquareBracket), start, index);
            }
            private EvaluableExpression GetBinaryExpression()
            {
                var begin = index;
                var left = DispatchExpression();
                if (!TryFindBinaryOperator(out var binaryOperator, out _))
                {
                    if (se[index, 2] == "in")
                    {
                        index += 2;
                        SkipSpaces();
                        var chr = se[index];
                        if (chr == openParen)
                        {
                            if (option.NotAllowedInExpression)
                            {
                                throw new ParseException($"In expression is not allowed at character {index}");
                            }
                            index++;
                            return new InExpression(left, GetArguments(closeParen), begin, index);
                        }
                    }
                    return left;
                }
                var right = DispatchExpression();
                if (right == null)
                {
                    throw new ParseException($"Expected expression after {binaryOperator.Operator} at character {index}");
                }
                var list = new List<object> { left, binaryOperator, right };
                while (TryFindBinaryOperator(out binaryOperator, out var start))
                {
                    var precedence = binaryOperator.Precedence;
                    if (precedence == 0) break;
                    var currentBinaryOperator = binaryOperator;
                    while ((list.Count > 2) && (precedence <= ((BinaryOperator)list[list.Count - 2]).Precedence))
                    {
                        right = list[list.Count - 1] as EvaluableExpression;
                        list.RemoveAt(list.Count - 1);
                        var @operator = (BinaryOperator)list[list.Count - 1];
                        list.RemoveAt(list.Count - 1);
                        left = list[list.Count - 1] as EvaluableExpression;
                        list.RemoveAt(list.Count - 1);
                        var binaryExpression = new BinaryExpression(@operator, left, right, start, index);
                        list.Add(binaryExpression);
                    }
                    var token = DispatchExpression();
                    if (token == null)
                    {
                        throw new ParseException($"Expected expression after {currentBinaryOperator.Operator} at character {index}");
                    }
                    list.Add(binaryOperator);
                    list.Add(token);
                }
                var i = list.Count - 1;
                var expr = list[i] as EvaluableExpression;
                while (i > 1)
                {
                    var @operator = (BinaryOperator)list[i - 1];
                    left = list[i - 2] as EvaluableExpression;
                    expr = new BinaryExpression(@operator, left, expr, begin, index);
                    i -= 2;
                }
                return expr;
            }
            private EvaluableExpression GetVariable()
            {
                var chr = se[index];
                var expr = chr == openParen ? GetGroup() : GetIdentifier();
                SkipSpaces();
                chr = se[index];
                var start = index;
                while (true)
                {
                    if (chr == period)
                    {
                        if (option.NotAllowedMemberExpression)
                        {
                            throw new ParseException($"Member expression is not allowed at character {index}");
                        }
                        index++;
                        SkipSpaces();
                        expr = new ObjectMemberExpression(expr, GetIdentifier(), start, index, false);
                        SkipSpaces();
                        chr = se[index];
                    }
                    else if (chr == openSquareBracket)
                    {
                        if (option.NotAllowedIndexerExpression)
                        {
                            throw new ParseException($"Indexer expression is not allowed at character {index}");
                        }
                        index++;
                        expr = new ObjectMemberExpression(expr, GetExpression(), start, index, true);
                        SkipSpaces();
                        chr = se[index];
                        if (chr != closeSquareBracket)
                        {
                            throw new ParseException($"Unclosed [ at character {index}");
                        }
                        index++;
                        SkipSpaces();
                        chr = se[index];
                    }
                    else if (chr == openParen)
                    {
                        index++;
                        switch (expr)
                        {
                            case ObjectMemberExpression callee:
                                expr = new ObjectsFunctionCallExpression(callee, GetArguments(closeParen), start, index);
                                break;
                            case IdentifierExpression identifier:
                                var name = identifier.Name;
                                var function = option.NakedFunctions.FirstOrDefault(f => f.Name == name);
                                if (function == null)
                                {
                                    throw new ParseException($"Can not find naked function ({name}) at character {index - name.Length - 1}");
                                }
                                expr = new NakedFunctionCallExpression(function, GetArguments(closeParen), start, index);
                                break;
                        }
                        SkipSpaces();
                        chr = se[index];
                    }
                    else
                    {
                        break;
                    }
                }
                return expr;
            }
            private EvaluableExpression DispatchExpression()
            {
                SkipSpaces();
                var chr = se[index];
                if (char.IsDigit(chr) || chr == period)
                {
                    return GetNumericLiteral();
                }
                if (chr == singleQuote || chr == doubleQuote)
                {
                    return GetStringLiteral();
                }
                if (chr == openSquareBracket)
                {
                    return GetArray();
                }
                if (TryFindUnaryOperator(out var @operator, out var s))
                {
                    return new UnaryExpression(@operator, DispatchExpression(), s, index);
                }
                if (IsIdentifierStart(chr) || chr == openParen)
                {
                    return GetVariable();
                }
                return null;
            }
            public EvaluableExpression Parse()
            {
                var expr = GetExpression();
                if (se.IsInBounds(index))
                {
                    throw new ParseException($"Unexpected \"{se[index]}\" at character {index}");
                }
                return expr;
            }
        }

        public EvaluableExpression Parse(string expr, ReadOnlyParseOption option = null)
        {
            if (string.IsNullOrWhiteSpace(expr))
                throw new ArgumentException($"{nameof(expr)} can not be null, empty, and consists only of white-space");
            var sourceExpression = new SourceExpression(expr);
            var parser = new ExpressionParser(sourceExpression, option ?? defaultParseOption);
            var expression = parser.Parse();
            return expression;
        }
    }
}

