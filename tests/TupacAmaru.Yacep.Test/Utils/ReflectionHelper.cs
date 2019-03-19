using System;
using System.Collections;
using TupacAmaru.Yacep.Exceptions;
using System.Collections.Generic;
using Xunit;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Test.Utils
{
    public class ReflectionHelper
    {
        [Fact(DisplayName = "get array indexer")]
        public void GetArrayIndexer()
        {
            var type = typeof(int[]);
            var indexer = type.CreateIndexer(typeof(int), false);
            var array = new[] { 34, 5, 78, 121 };
            Assert.Equal(array[1], indexer(array, 1));
        }

        [Fact(DisplayName = "get object function")]
        public void GetFunction()
        {
            var type = typeof(Fixture);
            var add = type.GetFunction("Add", false);
            var fixture = new Fixture { x = 11 };
            Assert.Equal(23, add(fixture, new object[] { 12 }));
            Assert.Throws<MethodNotFoundException>(() => type.GetFunction("Add1"));

            var valueReader = type.GetValueReaderWithNoCache("Add");
            Assert.NotNull(valueReader);
            var adder = valueReader(fixture) as Func<object[], object>;
            Assert.NotNull(adder);
            Assert.Equal(23, adder(new object[] { 12 }));
        }
        [Fact(DisplayName = "get object field")]
        public void GetField()
        {
            var type = typeof(Fixture);
            var reader = type.GetFieldReader("a", false);
            var fixture = new Fixture { a = "abasdasss" };
            Assert.Equal(fixture.a, reader(fixture));
            Assert.Throws<FieldNotFoundException>(() => type.GetFieldReader("yu"));

            var valueReader = type.GetValueReaderWithNoCache("a");
            Assert.NotNull(valueReader);
            Assert.Equal(fixture.a, valueReader(fixture));
        }
        [Fact(DisplayName = "get object property")]
        public void GetProperty()
        {
            var type = typeof(Fixture);
            var getter = type.GetPropertyGetter("D", false);
            var fixture = new Fixture { D = "yyyyyyyyyyyyyyyyyxxxxxxxx" };
            Assert.Equal(fixture.D, getter(fixture));
            Assert.Throws<PropertyNotFoundException>(() => type.GetPropertyGetter("Y"));
            Assert.Throws<PropertyNotFoundException>(() => type.GetPropertyGetter("yu"));

            var valueReader = type.GetValueReaderWithNoCache("D");
            Assert.NotNull(valueReader);
            Assert.Equal(fixture.D, valueReader(fixture));
        }
        [Fact(DisplayName = "get dict indexer")]
        public void GetDictionaryIndexer()
        {
            var type = typeof(Dictionary<string, int>);
            var indexer = type.CreateIndexer(typeof(string), false);
            var dictionary = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
            Assert.Equal(dictionary["a"], indexer(dictionary, "a"));

            var valueReader = type.GetValueReaderWithNoCache("a");
            Assert.NotNull(valueReader);
            Assert.Equal(dictionary["a"], valueReader(dictionary));
        }

        [Fact(DisplayName = "get hashtable indexer")]
        public void GetHashtableIndexer()
        {
            var table = new Hashtable
            {
                { "1", 2 },
                { "2", 3 }
            };
            var type = typeof(Hashtable);
            var indexer = type.CreateIndexer(typeof(string), false);
            Assert.Equal(table["1"], indexer(table, "1"));

            var valueReader = type.GetValueReaderWithNoCache("1");
            Assert.NotNull(valueReader);
            Assert.Equal(table["1"], valueReader(table));
        }
    }
}
