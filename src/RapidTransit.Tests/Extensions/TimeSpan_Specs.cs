namespace RapidTransit.Tests.Extensions
{
    using System;
    using Core.Extensions;
    using NUnit.Framework;


    [TestFixture]
    public class TimeSpan_Specs
    {
        [Test]
        public void Should_property_format_a_timespan()
        {
            var span = new TimeSpan(1, 2, 3, 4);

            string friendly = span.ToFriendlyString();

            Assert.AreEqual("1d2h3m4s", friendly);
        }
    }
}