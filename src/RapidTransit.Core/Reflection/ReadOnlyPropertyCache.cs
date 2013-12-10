namespace RapidTransit.Core.Reflection
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;
    using RapidTransit.Core.Caching;


    public class ReadOnlyPropertyCache<T> :
        AbstractCacheDecorator<string, ReadOnlyProperty<T>>
    {
        public ReadOnlyPropertyCache()
            : base(CreatePropertyCache())
        {
        }

        static ICache<string, ReadOnlyProperty<T>> CreatePropertyCache()
        {
            return new DictionaryCache<string, ReadOnlyProperty<T>>(typeof(T).GetAllProperties()
                .Where(x => x.CanRead)
                .Select(x => new ReadOnlyProperty<T>(x))
                .ToDictionary(x => x.Property.Name));
        }

        public object Get(Expression<Func<T, object>> propertyExpression, T instance)
        {
            return this[propertyExpression.GetMemberName()].Get(instance);
        }

        public void Each(T instance, Action<ReadOnlyProperty<T>, object> action)
        {
            Each(property => action(property, property.Get(instance)));
        }
    }
}