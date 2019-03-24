using System;
using System.Linq;
using TupacAmaru.Yacep.Expressions;
using TupacAmaru.Yacep.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace TupacAmaru.Yacep.Test.Formatter
{
    

    public class UnitTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

       
        [Fact(DisplayName = "format long expression test")]
        public void FormatLongExpression()
        {
            Assert.Equal("", (null as EvaluableExpression).ToPrettyString());
            var expr = "(10.5+23E4+2/max(a,b,c))||(true&&false)&&(12%5+x.a+x.b+y['a']+len(['1',2,null,'3'])?+(-(!a)):(null||(1 in (1,2+2,avg(3,m['a'](2),m.func(11,2))))))";
            var expression = expr.ToEvaluableExpression();
            var expected = @"type: Binary
operator: ||
left:
    type: Binary
    operator: +
    left:
        type: Binary
        operator: +
        left:
            type: Constant
            value: 10.5(Decimal)
            raw: 10.5
        right:
            type: Constant
            value: 230000(Decimal)
            raw: 23E4
    right:
        type: Binary
        operator: /
        left:
            type: Constant
            value: 2(Int32)
            raw: 2
        right:
            type: NakedFunctionCall
            functionName:max
            arguments:
                0:
                    type: Identifier
                    name: a
                1:
                    type: Identifier
                    name: b
                2:
                    type: Identifier
                    name: c
right:
    type: Binary
    operator: &&
    left:
        type: Binary
        operator: &&
        left:
            type: Literal
            value: True(Boolean)
            raw: true
        right:
            type: Literal
            value: False(Boolean)
            raw: false
    right:
        type: Conditional
        condition:
            type: Binary
            operator: +
            left:
                type: Binary
                operator: +
                left:
                    type: Binary
                    operator: +
                    left:
                        type: Binary
                        operator: +
                        left:
                            type: Binary
                            operator: %
                            left:
                                type: Constant
                                value: 12(Int32)
                                raw: 12
                            right:
                                type: Constant
                                value: 5(Int32)
                                raw: 5
                        right:
                            type: ObjectMember
                            object:
                                type: Identifier
                                name: x
                            member:indexer-False
                                type: Identifier
                                name: a
                    right:
                        type: ObjectMember
                        object:
                            type: Identifier
                            name: x
                        member:indexer-False
                            type: Identifier
                            name: b
                right:
                    type: ObjectIndexer
                    object:
                        type: Identifier
                        name: y
                    member:indexer-True
                        type: Constant
                        value: a(String)
                        raw: a
            right:
                type: NakedFunctionCall
                functionName:len
                arguments:
                    0:
                        type: Array
                        elements:
                            0:
                                type: Constant
                                value: 1(String)
                                raw: 1
                            1:
                                type: Constant
                                value: 2(Int32)
                                raw: 2
                            2:
                                type: Literal
                                value: null
                                raw: null
                            3:
                                type: Constant
                                value: 3(String)
                                raw: 3
        trueValue:
            type: Unary
            operator:+
            argument:
                type: Unary
                operator:-
                argument:
                    type: Unary
                    operator:!
                    argument:
                        type: Identifier
                        name: a
        falseValue:
            type: Binary
            operator: ||
            left:
                type: Literal
                value: null
                raw: null
            right:
                type: In
                value:
                        type: Constant
                        value: 1(Int32)
                        raw: 1
                values:
                    0:
                        type: Constant
                        value: 1(Int32)
                        raw: 1
                    1:
                        type: Binary
                        operator: +
                        left:
                            type: Constant
                            value: 2(Int32)
                            raw: 2
                        right:
                            type: Constant
                            value: 2(Int32)
                            raw: 2
                    2:
                        type: NakedFunctionCall
                        functionName:avg
                        arguments:
                            0:
                                type: Constant
                                value: 3(Int32)
                                raw: 3
                            1:
                                type: ObjectsFunctionCall
                                callee:
                                        type: ObjectIndexer
                                        object:
                                            type: Identifier
                                            name: m
                                        member:indexer-True
                                            type: Constant
                                            value: a(String)
                                            raw: a
                                arguments:
                                    0:
                                        type: Constant
                                        value: 2(Int32)
                                        raw: 2
                            2:
                                type: ObjectsFunctionCall
                                callee:
                                        type: ObjectMember
                                        object:
                                            type: Identifier
                                            name: m
                                        member:indexer-False
                                            type: Identifier
                                            name: func
                                arguments:
                                    0:
                                        type: Constant
                                        value: 11(Int32)
                                        raw: 11
                                    1:
                                        type: Constant
                                        value: 2(Int32)
                                        raw: 2
";
            var expectedArray = expected.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var actualArray = expression.ToPrettyString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Assert.True(actualArray.Select((current, index) => current.Equals(expectedArray[index])).All(x => x));
        }
    }
}
