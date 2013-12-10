namespace RapidTransit.Core.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Primitives;


    public class InMemoryObjectCache<TKey, TValue> :
        IObjectCache<TKey, TValue>
    {
        readonly Observable<ICacheItemAdded<TValue>> _added;
        readonly object _mutateLock = new object();
        readonly Observable<ICacheItemRemoved<TValue>> _removed;
        readonly Observable<ICacheItemUpdated<TValue>> _updated;
        readonly ConcurrentDictionary<TKey, TValue> _values;

        public InMemoryObjectCache()
        {
            _added = new Observable<ICacheItemAdded<TValue>>();
            _removed = new Observable<ICacheItemRemoved<TValue>>();
            _updated = new Observable<ICacheItemUpdated<TValue>>();

            _values = new ConcurrentDictionary<TKey, TValue>();
        }

        public ICollection<TValue> Values
        {
            get { return _values.Values; }
        }

        public IDisposable Subscribe(IObserver<ICacheItemAdded<TValue>> observer)
        {
            return _added.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<ICacheItemRemoved<TValue>> observer)
        {
            return _removed.Subscribe(observer);
        }

        public bool Contains(TKey key)
        {
            return _values.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _values.Values.Any(x => x.Equals(value));
        }

        public bool Exists(Func<TValue, bool> predicate)
        {
            return _values.Values.Any(x => predicate(x));
        }

        public bool Find(Func<TValue, bool> predicate, out TValue value)
        {
            foreach (TValue candidate in _values.Values)
            {
                if (predicate(candidate))
                {
                    value = candidate;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return _values.TryGetValue(key, out value);
        }

        public IReadOnlyObjectIndex<TIndex, TValue> GetIndex<TIndex>(Func<TValue, TIndex> keyProvider)
        {
            var indexCache = new InMemoryObjectCache<TIndex, TValue>();

            lock (_mutateLock)
            {
                foreach (TValue value in _values.Values)
                    indexCache.TryAdd(keyProvider(value), value);

                return new ReadOnlyObjectIndex<TKey, TValue, TIndex>(this, indexCache, keyProvider);
            }
        }


        public bool TryAdd(TKey key, TValue value)
        {
            bool added;
            lock (_mutateLock)
            {
                added = _values.TryAdd(key, value);
            }

            if (added)
                _added.OnNext(new CacheItemAddedImpl(value));

            return added;
        }

        public bool TryRemove(TKey key)
        {
            TValue value;
            bool removed;
            lock (_mutateLock)
            {
                removed = _values.TryRemove(key, out value);
            }

            if (removed)
                _removed.OnNext(new CacheItemRemovedImpl(value));

            return removed;
        }

        public bool TryUpdate(TKey key, TValue value, TValue previousValue)
        {
            bool updated;
            lock (_mutateLock)
            {
                updated = _values.TryUpdate(key, value, previousValue);
            }

            if (updated)
                _updated.OnNext(new CacheItemUpdatedImpl(value));

            return updated;
        }

        public IDisposable Subscribe(IObserver<ICacheItemUpdated<TValue>> observer)
        {
            return _updated.Subscribe(observer);
        }


        class CacheItemAddedImpl :
            ICacheItemAdded<TValue>
        {
            public CacheItemAddedImpl(TValue value)
            {
                Value = value;
            }

            public TValue Value { get; private set; }
        }


        class CacheItemRemovedImpl :
            ICacheItemRemoved<TValue>
        {
            public CacheItemRemovedImpl(TValue value)
            {
                Value = value;
            }

            public TValue Value { get; private set; }
        }


        public class CacheItemUpdatedImpl :
            ICacheItemUpdated<TValue>
        {
            public CacheItemUpdatedImpl(TValue value)
            {
                Value = value;
            }

            public TValue Value { get; private set; }
        }
    }
}