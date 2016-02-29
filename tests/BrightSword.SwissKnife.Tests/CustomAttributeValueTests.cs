using System;
using System.Linq;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class CustomAttributeValueTests
    {
        private enum Platform
        {
            [Name("latest")] [Tag("What?", "Whatever!")] [Tag("When?", "Whenever!")] Latest,

            [Name("v1")] V1,

            [Name("v1.1")] V11,

            [Name("v2")] V2,

            [Name("v4")] V4
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        private class PropertyAttribute : Attribute
        {
            public string AttributeValue { get; set; }
        }

        [Tag("Type", typeof (TypeWithAttributedProperty))]
        private class TypeWithAttributedProperty
        {
            [Property(AttributeValue = "Hello Joe!")]
            [Tag("Name1", "Some Value")]
            [Tag("Name2", "Some Other Value")]
            // ReSharper disable UnusedMember.Local
            public string SomeProperty { get; set; }

// ReSharper restore UnusedMember.Local
        }

        [Test]
        public void Test_GetCustomAttribute_Member()
        {
            var propertyInfo = typeof (TypeWithAttributedProperty).GetProperty("SomeProperty", true);
            Assert.IsNotNull(propertyInfo);

            var attribute = propertyInfo.GetCustomAttribute<PropertyAttribute>();
            Assert.IsInstanceOf<PropertyAttribute>(attribute);
            Assert.AreEqual("Hello Joe!", attribute.AttributeValue);
        }

        [Test]
        public void Test_GetCustomAttribute_Type()
        {
            var attributeIsValidOn = typeof (PropertyAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            Assert.IsInstanceOf<AttributeUsageAttribute>(attributeIsValidOn);
        }

        [Test]
        public void Test_GetCustomAttributeValue_Member()
        {
            var propertyInfo = typeof (TypeWithAttributedProperty).GetProperty("SomeProperty", true);
            Assert.IsNotNull(propertyInfo);

            var attributeValue = propertyInfo.GetCustomAttributeValue<PropertyAttribute, string>(_ => _.AttributeValue);
            Assert.AreEqual("Hello Joe!", attributeValue);
        }

        [Test]
        public void Test_GetCustomAttributeValue_Type()
        {
            var attributeIsValidOn =
                typeof (PropertyAttribute).GetCustomAttributeValue<AttributeUsageAttribute, AttributeTargets>(
                    _ => _.ValidOn);
            Assert.AreEqual(AttributeTargets.Property, attributeIsValidOn);
        }

        [Test]
        public void Test_GetCustomAttributeValues()
        {
            var propertyInfo = typeof (TypeWithAttributedProperty).GetProperty("SomeProperty", true);
            Assert.IsNotNull(propertyInfo);

            var attributeValues = propertyInfo.GetCustomAttributeValues<TagAttribute, string>(_ => _.Name);
            Assert.IsTrue(
                attributeValues.OrderBy(_ => _)
                               .SequenceEqual(
                                   new[]
                                   {
                                       "Name1",
                                       "Name2"
                                   }));
        }

        [Test]
        public void Test_NameAttribute()
        {
            Assert.AreEqual("latest", Platform.Latest.GetName());
            Assert.AreEqual("v1", Platform.V1.GetName());
            Assert.AreEqual("v1.1", Platform.V11.GetName());
            Assert.AreEqual("v2", Platform.V2.GetName());
            Assert.AreEqual("v4", Platform.V4.GetName());
        }

        [Test]
        public void Test_NamedValueAttribute()
        {
            Assert.AreEqual(
                typeof (TypeWithAttributedProperty),
                typeof (TypeWithAttributedProperty).GetNamedValue<Type>("Type"));
            Assert.AreEqual("Whatever!", Platform.Latest.GetNamedValue<string>("What?"));
            Assert.AreEqual("Whenever!", Platform.Latest.GetNamedValue<string>("When?"));
        }
    }
}