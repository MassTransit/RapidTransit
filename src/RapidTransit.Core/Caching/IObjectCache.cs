namespace RapidTransit.Core.Caching
{
    public interface IObjectCache<in TKey, TValue> :
        IReadOnlyObjectCache<TKey, TValue>
    {
        bool TryAdd(TKey key, TValue value);

        bool TryRemove(TKey key);

        bool TryUpdate(TKey key, TValue value, TValue previousValue);
    }
}