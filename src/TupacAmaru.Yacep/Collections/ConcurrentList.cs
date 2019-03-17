using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TupacAmaru.Yacep.Collections
{
    internal sealed class ConcurrentList<T> : ICollection<T>
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
}
