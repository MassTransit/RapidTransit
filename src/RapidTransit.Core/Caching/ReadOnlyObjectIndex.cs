namespace RapidTransit.Core.Caching
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    ///     Provides an index to items in a cache, using the key provider of the index
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TIndex"></typeparam>
    public class ReadOnlyObjectIndex<TKey, TValue, TIndex> :
        IReadOnlyObjectIndex<TIndex, TValue>,
        IObserver<ICacheItemAdded<TValue>>,
        IObserver<ICacheItemRemoved<TValue>>,
        IObserver<ICacheItemUpdated<TValue>>
    {
        readonly IDisposable _added;
        readonly IReadOnlyObjectCache<TKey, TValue> _cache;
        readonly IObjectCache<TIndex, TValue> _index;
        readonly Func<TValue, TIndex> _keyProvider;
        readonly IDisposable _removed;
        readonly IDisposable _updated;


        public ReadOnlyObjectIndex(IReadOnlyObjectCache<TKey, TValue> cache, IObjectCache<TIndex, TValue> index,
            Func<TValue, TIndex> keyProvider)
        {
            _index = index;
            _keyProvider = keyProvider;

            _cache = cache;
            _added = _cache.Subscribe((IObserver<ICacheItemAdded<TValue>>)this);
            _removed = _cache.Subscribe((IObserver<ICacheItemRemoved<TValue>>)this);
            _updated = _cache.Subscribe((IObserver<ICacheItemUpdated<TValue>>)this);
        }

        void IObserver<ICacheItemAdded<TValue>>.OnNext(ICacheItemAdded<TValue> value)
        {
            TIndex key = _keyProvider(value.Value);

            _index.TryAdd(key, value.Value);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        void IObserver<ICacheItemRemoved<TValue>>.OnNext(ICacheItemRemoved<TValue> value)
        {
            TIndex key = _keyProvider(value.Value);

            _index.TryRemove(key);
        }

        void IObserver<ICacheItemUpdated<TValue>>.OnNext(ICacheItemUpdated<TValue> value)
        {
            TIndex key = _keyProvider(value.Value);

            TValue existing;
            if (_index.TryGet(key, out existing))
                _index.TryUpdate(key, value.Value, existing);
        }

        public ICollection<TValue> Values
        {
            get { return _index.Values; }
        }

        public bool Contains(TIndex key)
        {
            return _index.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _index.ContainsValue(value);
        }

        public bool Exists(Func<TValue, bool> predicate)
        {
            return _index.Exists(predicate);
        }

        public bool Find(Func<TValue, bool> predicate, out TValue value)
        {
            return _index.Find(predicate, out value);
        }

        public bool TryGet(TIndex key, out TValue value)
        {
            return _index.TryGet(key, out value);
        }

        public IReadOnlyObjectIndex<T, TValue> GetIndex<T>(Func<TValue, T> keyProvider)
        {
            return _cache.GetIndex(keyProvider);
        }

        public IDisposable Subscribe(IObserver<ICacheItemAdded<TValue>> observer)
        {
            return _index.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<ICacheItemRemoved<TValue>> observer)
        {
            return _index.Subscribe(observer);
        }

        public void Dispose()
        {
            _added.Dispose();
            _removed.Dispose();
            _updated.Dispose();
        }

        public IDisposable Subscribe(IObserver<ICacheItemUpdated<TValue>> observer)
        {
            return _index.Subscribe(observer);
        }
    }
}