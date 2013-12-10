namespace RapidTransit.Core.Caching
{
    using System;
    using System.Collections.Generic;


    public interface IReadOnlyObjectCache<in TKey, TValue> :
        IObservable<ICacheItemAdded<TValue>>,
        IObservable<ICacheItemRemoved<TValue>>,
        IObservable<ICacheItemUpdated<TValue>>
    {
        ICollection<TValue> Values { get; }
        bool Contains(TKey key);
        bool ContainsValue(TValue value);

        bool Exists(Func<TValue, bool> predicate);

        bool Find(Func<TValue, bool> predicate, out TValue value);

        bool TryGet(TKey key, out TValue value);

        IReadOnlyObjectIndex<T, TValue> GetIndex<T>(Func<TValue, T> keyProvider);
    }
}