namespace RapidTransit.Core.Caching
{
    public delegate TValue MissingValueProvider<in TKey, out TValue>(TKey key);
}