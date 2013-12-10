namespace RapidTransit.Core.Caching
{
    public delegate TKey KeySelector<out TKey, in TValue>(TValue value);
}