namespace RapidTransit.Core.Caching
{
    public interface ICacheItemRemoved<out TValue>
    {
        TValue Value { get; }
    }
}