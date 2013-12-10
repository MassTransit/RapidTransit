namespace RapidTransit.Core.Caching
{
    public interface ICacheItemUpdated<out TValue>
    {
        TValue Value { get; }
    }
}