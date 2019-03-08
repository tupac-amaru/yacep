using System.Collections;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.Test
{
    public class ParseOptionUnitTest
    {
        [Fact(DisplayName = "parse option collection operate")]
        public void ParseOptionCollectionOperate()
        {
            var values = new ParseOption().LiteralValues;
            Assert.False(values.IsReadOnly);
            Parallel.For(0, 100, i =>
            {
                var value = new LiteralValue($"p{i}", i);
                values.Add(value);
                Assert.True(values.Contains(value));
                if (i % 2 == 0)
                {
                    values.Remove(value);
                    Assert.False(values.Contains(value));
                }
            });
            Assert.Equal(50, values.Count);

            Assert.True(values.Skip(10).Take(10).Count(x => (int)(x.Value) % 2 == 0) == 0);

            var copies = new LiteralValue[70];
            values.CopyTo(copies, 10);
            Assert.True(copies.Take(10).Count(x => x == null) == 10);
            Assert.True(copies.Skip(10).Take(50).Count(x => x != null) == 50);
            Assert.True(copies.Skip(60).Take(10).Count(x => x == null) == 10);

            IEnumerable enumerator = values;
            foreach (var item in enumerator)
                Assert.True((int)((item as LiteralValue)?.Value ?? 0) % 2 != 0);
            Parallel.For(0, 100, i => values.Clear());
            Assert.Equal(0, values.Count);
        }
    }
}
