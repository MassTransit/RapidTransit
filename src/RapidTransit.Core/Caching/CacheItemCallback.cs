namespace RapidTransit.Core.Caching
{
    public delegate void CacheItemCallback<in TKey, in TValue>(TKey key, TValue value);
}