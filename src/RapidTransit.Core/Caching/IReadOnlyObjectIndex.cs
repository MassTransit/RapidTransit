namespace RapidTransit.Core.Caching
{
    using System;


    /// <summary>
    /// An index to a cache for quick access to items
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IReadOnlyObjectIndex<in TKey, TValue> :
        IReadOnlyObjectCache<TKey, TValue>,
        IDisposable
    {
    }
}