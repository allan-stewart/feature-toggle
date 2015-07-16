using System;
using FeatureToggle;
using NUnit.Framework;

namespace FeatureToggleSpecs
{
    [TestFixture]
    public class HasherSpecs
    {
        Hasher ClassUnderTest;

        [SetUp]
        public void SetUp()
        {
            ClassUnderTest = new Hasher();
        }

        [Test]
        public void When_hashing_a_string()
        {
            var result = ClassUnderTest.Hash("this-is-a-string");

            Assert.That(result.Length, Is.EqualTo(32));
            Assert.That(Convert.ToBase64String(result), Is.EqualTo("WZfVC42xIJ8ZNIdUvqjGt+GqTGE9/Nm4qF6c97EMTE4="));
        }

        [Test]
        public void When_hashing_an_empty_string()
        {
            var result = ClassUnderTest.Hash("");

            Assert.That(result.Length, Is.EqualTo(32));
            Assert.That(Convert.ToBase64String(result), Is.EqualTo("47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU="));
        }

        [Test]
        public void When_hashing_a_null_value()
        {
            var result = ClassUnderTest.Hash(null);

            Assert.That(result.Length, Is.EqualTo(32));
            Assert.That(Convert.ToBase64String(result), Is.EqualTo("47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU="));
        }
    }
}