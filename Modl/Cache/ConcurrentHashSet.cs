using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Cache
{
    public class ConcurrentHashSet<T> : ICollection<T>
    {
        private HashSet<T> collection;
        private object hashLock = new object();

        public ConcurrentHashSet()
        {
            this.collection = new HashSet<T>();
        }

        public ConcurrentHashSet(IEnumerable<T> collection)
        {
            this.collection = new HashSet<T>(collection);
        }

        public bool Add(T item)
        {
            lock (hashLock)
                return collection.Add(item);
        }

        public bool AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                lock (hashLock)
                    this.collection.Add(item);

            return true;
        }

        public bool Remove(T item)
        {
            lock (hashLock)
                return collection.Remove(item);
        }

        public bool TryRemove(T item)
        {
            lock (hashLock)
            {
                if (collection.Contains(item))
                    return collection.Remove(item);

                return false;
            }
        }

        public void Clear()
        {
            lock (hashLock)
                collection.Clear();
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Contains(T item)
        {
            lock (hashLock)
                return collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (hashLock)
                collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                lock (hashLock)
                    return collection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (hashLock)
                return collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
