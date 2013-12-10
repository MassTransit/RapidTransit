namespace RapidTransit.Core.Caching
{
    public interface ICacheItemAdded<out TValue>
    {
        TValue Value { get; }
    }
}