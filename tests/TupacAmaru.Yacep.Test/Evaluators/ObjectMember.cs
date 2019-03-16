using System;
using System.Collections.Generic;
using System.Diagnostics;
using TupacAmaru.Yacep.Evaluators;
using TupacAmaru.Yacep.Exceptions;
using Xunit;

namespace TupacAmaru.Yacep.Test.Evaluators
{
    public class EvaluateObjectMemberUnitTest
    {
        [Fact(DisplayName = "evaluate object member expression")]
        public void ObjectMemberExpressionEvaluator()
        {
            Assert.Null(ObjectMemberEvaluator.Evaluate(null, 3, true));
            Assert.Equal(2, ObjectMemberEvaluator.Evaluate(new object[] { 1, 2, 3, 4 }, 1, true));
            Assert.Equal(4, ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, 3, true));
            Assert.Equal("abc", ObjectMemberEvaluator.Evaluate(new Dictionary<int, object>
            {
                [1] = "abc",
                [2] = "b"
            }, 1, true));
            Assert.Equal("aaa", ObjectMemberEvaluator.Evaluate(new Fixture() { x = 12, y = "aaa" }, "y", true));
            Assert.Throws<UnsupportedNullValueIndexerException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, null, true));
            Assert.Throws<UnsupportedEmptyNameMembeException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, "", true));
            Assert.Throws<UnsupportedObjectIndexerException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, true, true));

            Assert.Equal("aaa", ObjectMemberEvaluator.Evaluate(new Fixture() { x = 12, y = "aaa" }, "y", false));
            Assert.Throws<UnsupportedEmptyNameMembeException>(() => ObjectMemberEvaluator.Evaluate(new List<int> { 1, 2, 3, 4 }, "", false));
        }

        [Fact(DisplayName = "can't evaluate error identifier expression")]
        public void ErrorInput()
        {
            Assert.Null(ObjectMemberEvaluator.Evaluate(null, "Item1", false));
            Assert.Throws<UnsupportedEmptyNameMembeException>(() => ObjectMemberEvaluator.Evaluate(new object(), "", false));
        }

        [Fact(DisplayName = "can't evaluate tuple's identifier expression")]
        public void CannotGetFromTuple()
        {
            var tuple = (x: 189, y: "codemonk");
            Assert.Equal(189, ObjectMemberEvaluator.Evaluate(tuple, "Item1", false));
            Assert.Equal("Not found member(x) in (ValueTuple`2)", Assert.Throws<MemberNotFoundException>(() => ObjectMemberEvaluator.Evaluate(tuple, nameof(tuple.x), false)).Message);
            Assert.Equal("Not found member(x) in (ValueTuple`2)", Assert.Throws<MemberNotFoundException>(() => ObjectMemberEvaluator.Evaluate(tuple, "x", false)).Message);
        }

        [Fact(DisplayName = "evaluate object property identifier expression")]
        public void GetFromObjectProperty()
        {
            object state = new { X = 12, N = "demo", x = true };
            Assert.Equal(12, ObjectMemberEvaluator.Evaluate(state, "X", false));
            Assert.Equal("demo", ObjectMemberEvaluator.Evaluate(state, "N", false));
            Assert.Equal(true, ObjectMemberEvaluator.Evaluate(state, "x", false));
            Assert.Equal(3, ObjectMemberEvaluator.Evaluate("abc", "Length", false));

            state = new Fixture() { x = 12, y = "aaa" };
            Assert.Equal("Not found member(Y) in (Fixture)", Assert.Throws<MemberNotFoundException>(() => ObjectMemberEvaluator.Evaluate(state, "Y", false)).Message);
            Assert.Equal("Not found member(abc) in (String)", Assert.Throws<MemberNotFoundException>(() => ObjectMemberEvaluator.Evaluate("abc", "abc", false)).Message);
        }

        [Fact(DisplayName = "evaluate object field identifier expression")]
        public void GetFromObjectField()
        {
            object state = new Fixture() { x = 12, y = "aaa" };
            Assert.Equal(12, ObjectMemberEvaluator.Evaluate(state, "x", false));
            Assert.Equal("aaa", ObjectMemberEvaluator.Evaluate(state, "y", false));
            Assert.Equal("Not found member(abc) in (Fixture)", Assert.Throws<MemberNotFoundException>(() => ObjectMemberEvaluator.Evaluate(state, "abc", false)).Message);
        }

        [Fact(DisplayName = "evaluate object method identifier expression")]
        public void GetFromObjectMethod()
        {
            object state = "this is a string";
            Assert.Throws<System.Reflection.AmbiguousMatchException>(() => ObjectMemberEvaluator.Evaluate(state, "ToString", false));
            var fixture = new Fixture() { x = 12, y = "aaa" };
            var toString = ObjectMemberEvaluator.Evaluate(fixture, "ToString", false) as Func<object[], object>;
            Assert.NotNull(toString);
            Assert.Equal(fixture.ToString(), toString(null));

            var getString = ObjectMemberEvaluator.Evaluate(fixture, "GetString", false) as Func<object[], object>;
            Assert.NotNull(getString);
            Assert.Equal(fixture.GetString("demo"), getString(new object[] { "demo" }));

            var add = ObjectMemberEvaluator.Evaluate(fixture, "Add", false) as Func<object[], object>;
            Assert.NotNull(add);
            Assert.Equal(fixture.Add(13), add(new object[] { 13 }));

            fixture.DoSomething();
            Assert.Equal("function called", fixture.y);
            fixture.y = "function not call";
            Assert.Equal("function not call", fixture.y);

            var watch = Stopwatch.StartNew();
            watch.Start();
            var doSomething = ObjectMemberEvaluator.Evaluate(fixture, "DoSomething", false) as Func<object[], object>;
            watch.Stop();
            var elapsedTicks = watch.ElapsedTicks;
            Assert.NotNull(doSomething);
            doSomething(null);
            Assert.Equal("function called", fixture.y);
            fixture.y = "function not call";
            watch.Restart();
            var doSomethingAgain = ObjectMemberEvaluator.Evaluate(fixture, "DoSomething", false) as Func<object[], object>;
            watch.Stop();
            Assert.True(watch.ElapsedTicks < elapsedTicks);
            Assert.NotNull(doSomethingAgain);
            doSomethingAgain(null);
            Assert.Equal("function called", fixture.y);

            Assert.Equal("Not found member(abc) in (Fixture)", Assert.Throws<MemberNotFoundException>(() => ObjectMemberEvaluator.Evaluate(fixture, "abc", false)).Message);
        }

        [Fact(DisplayName = "evaluate dictionary key identifier expression")]
        public void GetFromDictionary()
        {
            var dict = new Dictionary<string, object>() { ["x"] = 12, ["y"] = "aaa" };
            Assert.Equal(dict["x"], ObjectMemberEvaluator.Evaluate(dict, "x", false));
            Assert.Equal(dict["y"], ObjectMemberEvaluator.Evaluate(dict, "y", false));
            Assert.Equal(99, ObjectMemberEvaluator.Evaluate(new Dictionary<string, int> { ["x"] = 11, ["y"] = 99 }, "y", false));
            Assert.Equal(2, ObjectMemberEvaluator.Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = "b"
            }, "Count", false));
            Assert.Throws<KeyNotFoundException>(() => ObjectMemberEvaluator.Evaluate(dict, "abc", false));
            Assert.Throws<MemberNotFoundException>(() => ObjectMemberEvaluator.Evaluate(new Dictionary<int, object>
            {
                [1] = "a",
                [2] = "b"
            }, "1", false));
        }
    }
}
