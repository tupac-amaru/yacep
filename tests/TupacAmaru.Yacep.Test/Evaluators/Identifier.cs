using System;
using System.Collections.Generic;
using System.Diagnostics;
using TupacAmaru.Yacep.Evaluators;
using TupacAmaru.Yacep.Exceptions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Evaluators
{
    public class EvaluateIdentifierUnitTest
    {
        [Fact(DisplayName = "can't evaluate error identifier expression")]
        public void ErrorInput()
        {
            Assert.Null(IdentifierEvaluator.Evaluate(null, null, "Item1"));
            Assert.Throws<ArgumentNullException>(() => IdentifierEvaluator.Evaluate(new object(), typeof(object), ""));
        }

        [Fact(DisplayName = "can't evaluate tuple's identifier expression")]
        public void CannotGetFromTuple()
        {
            var tuple = (x: 189, y: "codemonk");
            Assert.Equal(189, IdentifierEvaluator.Evaluate(tuple, tuple.GetType(), "Item1"));
            Assert.Equal("Not found member(x) in (ValueTuple`2)", Assert.Throws<MemberNotFoundException>(() => IdentifierEvaluator.Evaluate(tuple, tuple.GetType(), nameof(tuple.x))).Message);
            Assert.Equal("Not found member(x) in (ValueTuple`2)", Assert.Throws<MemberNotFoundException>(() => IdentifierEvaluator.Evaluate(tuple, tuple.GetType(), "x")).Message);
        }

        [Fact(DisplayName = "evaluate object property identifier expression")]
        public void GetFromObjectProperty()
        {
            object state = new { X = 12, N = "demo", x = true };
            Assert.Equal(12, IdentifierEvaluator.Evaluate(state, state.GetType(), "X"));
            Assert.Equal("demo", IdentifierEvaluator.Evaluate(state, state.GetType(), "N"));
            Assert.Equal(true, IdentifierEvaluator.Evaluate(state, state.GetType(), "x"));
            Assert.Equal(3, IdentifierEvaluator.Evaluate("abc", typeof(string), "Length"));

            state = new Fixture() { x = 12, y = "aaa" };
            Assert.Equal("Not found member(Y) in (Fixture)", Assert.Throws<MemberNotFoundException>(() => IdentifierEvaluator.Evaluate(state, state.GetType(), "Y")).Message);
            Assert.Equal("Not found member(abc) in (String)", Assert.Throws<MemberNotFoundException>(() => IdentifierEvaluator.Evaluate("abc", typeof(string), "abc")).Message);
        }

        [Fact(DisplayName = "evaluate object field identifier expression")]
        public void GetFromObjectField()
        {
            object state = new Fixture() { x = 12, y = "aaa" };
            Assert.Equal(12, IdentifierEvaluator.Evaluate(state, state.GetType(), "x"));
            Assert.Equal("aaa", IdentifierEvaluator.Evaluate(state, state.GetType(), "y"));
            Assert.Equal("Not found member(abc) in (Fixture)", Assert.Throws<MemberNotFoundException>(() => IdentifierEvaluator.Evaluate(state, state.GetType(), "abc")).Message);
        }

        [Fact(DisplayName = "evaluate object method identifier expression")]
        public void GetFromObjectMethod()
        {
            object state = "this is a string";
            Assert.Throws<System.Reflection.AmbiguousMatchException>(() => IdentifierEvaluator.Evaluate(state, state.GetType(), "ToString"));
            var fixture = new Fixture() { x = 12, y = "aaa" };
            var toString = IdentifierEvaluator.Evaluate(fixture, fixture.GetType(), "ToString") as Func<object[], object>;
            Assert.NotNull(toString);
            Assert.Equal(fixture.ToString(), toString(null));

            var getString = IdentifierEvaluator.Evaluate(fixture, fixture.GetType(), "GetString") as Func<object[], object>;
            Assert.NotNull(getString);
            Assert.Equal(fixture.GetString("demo"), getString(new object[] { "demo" }));

            var add = IdentifierEvaluator.Evaluate(fixture, fixture.GetType(), "Add") as Func<object[], object>;
            Assert.NotNull(add);
            Assert.Equal(fixture.Add(13), add(new object[] { 13 }));

            fixture.DoSomething();
            Assert.Equal("function called", fixture.y);
            fixture.y = "function not call";
            Assert.Equal("function not call", fixture.y);

            var watch = Stopwatch.StartNew();
            watch.Start();
            var doSomething = IdentifierEvaluator.Evaluate(fixture, fixture.GetType(), "DoSomething") as Func<object[], object>;
            watch.Stop();
            var elapsedTicks = watch.ElapsedTicks;
            Assert.NotNull(doSomething);
            doSomething(null);
            Assert.Equal("function called", fixture.y);
            fixture.y = "function not call";
            watch.Restart();
            var doSomethingAgain = IdentifierEvaluator.Evaluate(fixture, fixture.GetType(), "DoSomething") as Func<object[], object>;
            watch.Stop();
            Assert.True(watch.ElapsedTicks < elapsedTicks);
            Assert.NotNull(doSomethingAgain);
            doSomethingAgain(null);
            Assert.Equal("function called", fixture.y);

            Assert.Equal("Not found member(abc) in (Fixture)", Assert.Throws<MemberNotFoundException>(() => IdentifierEvaluator.Evaluate(fixture, fixture.GetType(), "abc")).Message);
        }

        [Fact(DisplayName = "evaluate dictionary key identifier expression")]
        public void GetFromDictionary()
        {
            var dict = new Dictionary<string, object>() { ["x"] = 12, ["y"] = "aaa" };
            Assert.Equal(dict["x"], IdentifierEvaluator.Evaluate(dict, dict.GetType(), "x"));
            Assert.Equal(dict["y"], IdentifierEvaluator.Evaluate(dict, dict.GetType(), "y"));
            Assert.Equal(99, IdentifierEvaluator.Evaluate(new Dictionary<string, int> { ["x"] = 11, ["y"] = 99 }, typeof(Dictionary<string, int>), "y"));
            Assert.Equal(2, IdentifierEvaluator.Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = "b"
            }, typeof(Dictionary<int, object>), "Count"));
            Assert.Throws<KeyNotFoundException>(() => IdentifierEvaluator.Evaluate(dict, dict.GetType(), "abc"));
            Assert.Throws<MemberNotFoundException>(() => IdentifierEvaluator.Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = "b"
            }, typeof(Dictionary<int, object>), "1"));
        }
    }
}
