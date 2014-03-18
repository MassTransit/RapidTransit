namespace RapidTransit.Tests.Reflection
{
    using System;
    using Core.Reflection;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_constructor_has_zero_dependencies
    {
        [Test]
        public void Should_create_with_ease()
        {
            var subject = ComponentFactory.Get<MyClass>();

            Assert.IsNotNull(subject);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            ComponentFactory.Add<MyClass>();
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            ComponentFactory.Remove<MyClass>();
        }


        class MyClass
        {
        }
    }


    [TestFixture]
    public class When_a_constructor_has_dependencies
    {
        [Test]
        public void Should_create_with_ease()
        {
            var subject = ComponentFactory.Get<MyClass>();

            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.Dependency);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            ComponentFactory.Add(typeof(MyClass), typeof(MyClass), new[] {typeof(MyDependency)});
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            ComponentFactory.Remove<MyClass>();
            ComponentFactory.Remove<MyDependency>();
        }


        class MyClass
        {
            public MyClass(MyDependency dependency)
            {
                Dependency = dependency;
            }

            public MyDependency Dependency { get; set; }
        }


        class MyDependency
        {
        }
    }


    [TestFixture]
    public class When_a_constructor_has_dependencies_that_are_not_added
    {
        [Test]
        public void Should_create_with_ease()
        {
            var subject = ComponentFactory.Get<MyClass>();

            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.Dependency);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            ComponentFactory.Remove<MyClass>();
        }


        class MyClass
        {
            public MyClass(MyDependency dependency)
            {
                Dependency = dependency;
            }

            public MyDependency Dependency { get; set; }
        }


        class MyDependency
        {
        }
    }


    [TestFixture]
    public class When_a_constructor_has_nested_dependencies_that_are_not_added
    {
        [Test]
        public void Should_create_with_ease()
        {
            var subject = ComponentFactory.Get<MyClass>();

            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.Dependency);
            Assert.IsNotNull(subject.Dependency.Dependency);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            ComponentFactory.Remove<MyClass>();
        }


        class MyClass
        {
            public MyClass(MyDependency dependency)
            {
                Dependency = dependency;
            }

            public MyDependency Dependency { get; private set; }
        }


        class MyDependency
        {
            public MyDependency(MyChildDependency dependency)
            {
                Dependency = dependency;
            }

            public MyChildDependency Dependency { get; private set; }
        }


        class MyChildDependency
        {
        }
    }


    [TestFixture]
    public class When_adding_a_type_that_is_not_assignable
    {
        [Test]
        public void Should_throw_an_exception()
        {
            Assert.Throws<ArgumentException>(() => ComponentFactory.Add(typeof(IB), typeof(A)));
        }


        class A
        {
        }


        interface IB
        {
        }
    }
}