using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TupacAmaru.Yacep.BuiltIn;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep
{
    public sealed class ParseOption
    {
        private sealed class ConcurrentList<T> : ICollection<T>
        {
            private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
            private readonly List<T> storage = new List<T>();

            public IEnumerator<T> GetEnumerator()
            {
                rwLock.EnterReadLock();
                try
                {
                    var enumerator = storage.Select(x => x).ToList();
                    return enumerator.GetEnumerator();
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(T item)
            {
                rwLock.EnterWriteLock();
                try
                {
                    storage.Add(item);
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }

            public void Clear()
            {
                rwLock.EnterWriteLock();
                try
                {
                    storage.Clear();
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }

            public bool Contains(T item)
            {
                rwLock.EnterReadLock();
                try
                {
                    return storage.Contains(item);
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                rwLock.EnterReadLock();
                try
                {
                    storage.CopyTo(array, arrayIndex);
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }

            public bool Remove(T item)
            {
                rwLock.EnterWriteLock();
                try
                {
                    return storage.Remove(item);
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }

            public int Count => storage.Count;
            public bool IsReadOnly => false;
        }
        public bool NotAllowedArrayExpression { get; set; }
        public bool NotAllowedConditionExpression { get; set; }
        public bool NotAllowedMemberExpression { get; set; }
        public bool NotAllowedIndexerExpression { get; set; }
        public bool NotAllowedInExpression { get; set; }
        public bool NotAllowedConvertUnsignedInteger { get; set; }
        public ICollection<UnaryOperator> UnaryOperators { get; } = new ConcurrentList<UnaryOperator>();
        public ICollection<BinaryOperator> BinaryOperators { get; } = new ConcurrentList<BinaryOperator>();
        public ICollection<LiteralValue> LiteralValues { get; } = new ConcurrentList<LiteralValue>();
        public ICollection<NakedFunction> NakedFunctions { get; } = new ConcurrentList<NakedFunction>();

        public ReadOnlyParseOption AsReadOnly()
        {
            return new ReadOnlyParseOption(NotAllowedArrayExpression, NotAllowedConditionExpression,
                NotAllowedMemberExpression, NotAllowedIndexerExpression, NotAllowedInExpression, NotAllowedConvertUnsignedInteger,
                UnaryOperators, BinaryOperators, LiteralValues, NakedFunctions);
        }

        public static ParseOption CreateOption(bool addDefaultBinaryOperators = true, bool addDefaultUnaryOperators = true,
            bool addDefaultLiteralValues = true, bool addStatisticsFunctions = true, bool addDateAndTimeFunctions = true)
        {
            var option = new ParseOption();

            if (addDefaultLiteralValues)
            {
                option.LiteralValues.Add(Literals.True);
                option.LiteralValues.Add(Literals.False);
                option.LiteralValues.Add(Literals.Null);
            }

            if (addDefaultUnaryOperators)
            {
                option.UnaryOperators.Add(BuiltIn.UnaryOperators.Positive);
                option.UnaryOperators.Add(BuiltIn.UnaryOperators.Negative);
                option.UnaryOperators.Add(BuiltIn.UnaryOperators.Not);
            }

            if (addDefaultBinaryOperators)
            {
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Or);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.And);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Equal);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.NotEqual);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Less);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Greater);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.LessEqual);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.GreaterEqual);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Add);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Minus);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Multiply);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Divide);
                option.BinaryOperators.Add(BuiltIn.BinaryOperators.Modulo);
            }

            if (addStatisticsFunctions)
            {
                option.NakedFunctions.Add(StatisticsFunctions.Len);
                option.NakedFunctions.Add(StatisticsFunctions.Max);
                option.NakedFunctions.Add(StatisticsFunctions.Min);
                option.NakedFunctions.Add(StatisticsFunctions.Sum);
                option.NakedFunctions.Add(StatisticsFunctions.Avg);
            }

            if (addDateAndTimeFunctions)
            {
                option.NakedFunctions.Add(DateAndTimeFunctions.Now);
                option.NakedFunctions.Add(DateAndTimeFunctions.Date);
                option.NakedFunctions.Add(DateAndTimeFunctions.Time);
                option.NakedFunctions.Add(DateAndTimeFunctions.Year);
                option.NakedFunctions.Add(DateAndTimeFunctions.Month);
                option.NakedFunctions.Add(DateAndTimeFunctions.Day);
                option.NakedFunctions.Add(DateAndTimeFunctions.Hour);
                option.NakedFunctions.Add(DateAndTimeFunctions.Minute);
                option.NakedFunctions.Add(DateAndTimeFunctions.Second);
            }
            return option;
        }
    }
}