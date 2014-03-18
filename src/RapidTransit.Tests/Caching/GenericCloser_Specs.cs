namespace RapidTransit.Tests.Caching
{
    using System;
    using Core.Caching;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_generic_type_cache
    {
        [Test]
        public void Should_properly_construct_and_close_a_generic_type()
        {
            Assert.AreEqual(typeof(int), _cache[typeof(int)].SpecializedType);
        }

        [Test]
        public void Should_properly_construct_and_close_another_generic_type()
        {
            _cache.Add(typeof(string), new GenericClass<string>());

            Assert.AreEqual(typeof(string), _cache[typeof(string)].SpecializedType);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void Should_throw_an_exception_on_duplicate_add()
        {
            IGeneric x = _cache[typeof(bool)];

            _cache.Add(typeof(bool), new GenericClass<bool>());
        }

        GenericTypeCache<IGeneric> _cache;

        [TestFixtureSetUp]
        public void Setup()
        {
            _cache = new GenericTypeCache<IGeneric>(typeof(GenericClass<>));
        }


        interface IGeneric
        {
            Type SpecializedType { get; }
        }


        interface IGeneric<T> :
            IGeneric
        {
        }


        class GenericClass<T> :
            IGeneric<T>
        {
            public Type SpecializedType
            {
                get { return typeof(T); }
            }
        }
    }
}