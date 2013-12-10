namespace RapidTransit.Core.Reflection
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;
    using RapidTransit.Core.Caching;


    public class ReadWritePropertyCache<T> :
        AbstractCacheDecorator<string, ReadWriteProperty<T>>
    {
        public ReadWritePropertyCache()
            : this(false)
        {
        }

        public ReadWritePropertyCache(bool includeNonPublic)
            : base(CreatePropertyCache(includeNonPublic))
        {
        }

        static ICache<string, ReadWriteProperty<T>> CreatePropertyCache(bool includeNonPublic)
        {
            return new DictionaryCache<string, ReadWriteProperty<T>>(typeof(T).GetAllProperties()
                .Where(x => x.CanRead && (includeNonPublic || x.CanWrite))
                .Where(x => x.SetMethod != null)
                .Select(x => new ReadWriteProperty<T>(x, includeNonPublic))
                .ToDictionary(x => x.Property.Name));
        }

        public void Set(Expression<Func<T, object>> propertyExpression, T instance, object value)
        {
            this[propertyExpression.GetMemberName()].Set(instance, value);
        }

        public object Get(Expression<Func<T, object>> propertyExpression, T instance)
        {
            return this[propertyExpression.GetMemberName()].Get(instance);
        }

        public void Each(T instance, Action<ReadWriteProperty<T>, object> action)
        {
            Each(property => action(property, property.Get(instance)));
        }
    }
}