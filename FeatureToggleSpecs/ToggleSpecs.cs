using System;
using System.Linq;
using FeatureToggle;
using Moq;
using NUnit.Framework;

namespace FeatureToggleSpecs
{
    [TestFixture]
    public class ToggleSpecs
    {
        Toggle ClassUnderTest;
        Mock<IHasher> mockHasher;
        string identifier;
        
        [SetUp]
        public void Setup()
        {
            identifier = "test-user";
            mockHasher = new Mock<IHasher>();

            ClassUnderTest = new Toggle(mockHasher.Object);
        }

        [TestCase(.2, true)]
        [TestCase(.5, true)]
        [TestCase(.9, false)]
        public void When_checking_the_toggle_for_a_percent(double randomConfig, bool expected)
        {

            mockHasher.Setup(x => x.Hash(identifier)).Returns(Enumerable.Range(128, 10).Select(x => (byte) x).ToArray);

            var result = ClassUnderTest.IsFeatureOn("percent=" + randomConfig, identifier);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void When_checking_a_toggle_and_the_config_is_on()
        {
            var result = ClassUnderTest.IsFeatureOn("on", identifier);
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void When_checking_a_toggle_and_the_config_is_off()
        {
            var result = ClassUnderTest.IsFeatureOn("off", identifier);
            
            Assert.That(result, Is.False);
        }

        [TestCase(1, false)]
        [TestCase(-1, true)]
        public void When_checking_a_toggle_for_a_start_time(int minutesFromNow, bool expected)
        {
            var result = ClassUnderTest.IsFeatureOn("start=" + BuildTimestampFromMinutes(minutesFromNow), identifier);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(1, true)]
        [TestCase(-1, false)]
        public void When_checking_a_toggle_for_an_end_time(int minutesFromNow, bool expected)
        {
            var result = ClassUnderTest.IsFeatureOn("end=" + BuildTimestampFromMinutes(minutesFromNow), identifier);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(1, 2, false)]
        [TestCase(-1, 1, true)]
        [TestCase(-2, -1, false)]
        public void When_checking_a_toggle_for_a_time_range(int minutesFromStart, int minutesFromEnd, bool expected)
        {
            var result = ClassUnderTest.IsFeatureOn(string.Format("between={0},{1}", BuildTimestampFromMinutes(minutesFromStart), BuildTimestampFromMinutes(minutesFromEnd)), identifier);

            Assert.That(result, Is.EqualTo(expected));
        }

        private string BuildTimestampFromMinutes(int minutes)
        {
            return DateTime.UtcNow.AddMinutes(minutes).ToString("O");
        }
    }
}
