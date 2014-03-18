namespace RapidTransit.Tests.Reflection
{
    using Core.Reflection;
    using NUnit.Framework;


    [TestFixture]
    public class When_multiple_constructors_are_satisfied
    {
        [Test]
        public void Should_choose_the_greedy_one()
        {
            var subject = ComponentFactory.Get<MyClass>();

            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.Dependency);
            Assert.IsNotNull(subject.OtherDependency);
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

            public MyClass(MyDependency dependency, MyOtherDependency otherDependency)
            {
                Dependency = dependency;
                OtherDependency = otherDependency;
            }

            public MyDependency Dependency { get; private set; }
            public MyOtherDependency OtherDependency { get; private set; }
        }


        class MyDependency
        {
            public MyDependency(MyChildDependency dependency)
            {
                Dependency = dependency;
            }

            public MyChildDependency Dependency { get; private set; }
        }


        class MyOtherDependency
        {
        }


        class MyChildDependency
        {
        }
    }


    [TestFixture]
    public class When_multiple_constructors_are_satisfied_by_interfaces
    {
        [Test]
        public void Should_pull_the_implementation_for_the_type()
        {
            ComponentFactory.Add<IMyDependency>(typeof(MyDependency));

            var subject = ComponentFactory.Get<MyClass>();

            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.Dependency);
            Assert.IsInstanceOf<MyDependency>(subject.Dependency);
            Assert.IsNotNull(subject.OtherDependency);
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

            public MyClass(IMyDependency dependency, MyOtherDependency otherDependency)
            {
                Dependency = dependency;
                OtherDependency = otherDependency;
            }

            public IMyDependency Dependency { get; private set; }
            public MyOtherDependency OtherDependency { get; private set; }
        }


        interface IMyDependency
        {
            MyChildDependency Dependency { get; }
        }


        class MyDependency :
            IMyDependency
        {
            public MyDependency(MyChildDependency dependency)
            {
                Dependency = dependency;
            }

            public MyChildDependency Dependency { get; private set; }
        }


        class MyOtherDependency
        {
        }


        class MyChildDependency
        {
        }
    }
}