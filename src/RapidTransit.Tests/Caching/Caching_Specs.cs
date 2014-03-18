namespace RapidTransit.Tests.Caching
{
    using System;
    using Core.Caching;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_in_memory_cache
    {
        [Test]
        public void Should_support_index_creation()
        {
            IObjectCache<Guid, Data> cache = new InMemoryObjectCache<Guid, Data>();
            var data = new Data();
            cache.TryAdd(data.Identifier, data);

            IReadOnlyObjectIndex<string, Data> index = cache.GetIndex(x => x.Value);

            Assert.IsTrue(index.Contains(data.Value));
        }

        [Test]
        public void Should_support_index_creation_with_adds()
        {
            IObjectCache<Guid, Data> cache = new InMemoryObjectCache<Guid, Data>();

            IReadOnlyObjectIndex<string, Data> index = cache.GetIndex(x => x.Value);

            var data = new Data();
            cache.TryAdd(data.Identifier, data);

            Assert.IsTrue(index.Contains(data.Value));
        }

        [Test]
        public void Should_support_index_retrieval_of_items()
        {
            IObjectCache<Guid, Data> cache = new InMemoryObjectCache<Guid, Data>();
            var data = new Data();
            cache.TryAdd(data.Identifier, data);

            Assert.IsTrue(cache.Contains(data.Identifier));
        }


        class Data
        {
            public Guid Identifier;
            public string Value;

            public Data()
            {
                Identifier = Guid.NewGuid();
                Value = Identifier.ToString();
            }
        }
    }
}