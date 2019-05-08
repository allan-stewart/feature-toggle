using FeatureToggle;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureToggleSpecs
{
    public class FeatureToggleEvaluatorSpecs
    {
        private FeatureToggleEvaluator classUnderTest;
        private Mock<IHasher> mockHasher;
        private Mock<IFeatureToggleSettingsRepository> mockSettingsRepository;

        [SetUp]
        public void SetUp()
        {
            mockHasher = new Mock<IHasher>();
            mockSettingsRepository = new Mock<IFeatureToggleSettingsRepository>();
            classUnderTest = new FeatureToggleEvaluator(mockHasher.Object, mockSettingsRepository.Object);
        }

        [TestCase(new byte[] { 0 }, 0)]
        [TestCase(new byte[] { 1 }, 0.0039)]
        [TestCase(new byte[] { 50 }, 0.1953)]
        [TestCase(new byte[] { 113 }, 0.4414)]
        [TestCase(new byte[] { 128 }, 0.5)]
        [TestCase(new byte[] { 255 }, 0.9960)]
        public void When_determining_percentage_for_a_user(byte[] hash, double expected)
        {
            var settings = new FeatureToggleSettings
            {
                FeatureId = "test-feature"
            };

            mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(hash);

            var result = classUnderTest.GetPercentageForUser(settings, "test-user");

            Assert.That(result, Is.EqualTo(expected).Within(.0001));
            mockHasher.Verify(x => x.Hash(settings.FeatureId + "test-user"));
        }

        [Test]
        public void When_checking_a_feature_for_user_and_the_feature_is_disabled()
        {
            var settings = new FeatureToggleSettings
            {
                FeatureId = "test-feature",
                Enabled = false
            };

            var result = classUnderTest.IsFeatureOnForUser(settings, "test-user");

            Assert.That(result, Is.False);
        }

        [Test]
        public void When_checking_a_feature_for_user_and_the_feature_is_set_to_zero_percent_rollout()
        {
            var settings = new FeatureToggleSettings
            {
                FeatureId = "test-feature",
                Enabled = true,
                RolloutPercentage = 0
            };

            mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(new byte[] { 0 });

            var result = classUnderTest.IsFeatureOnForUser(settings, "test-user");

            Assert.That(result, Is.False);
        }

        [TestCase(new byte[] { 0 }, .10, true)]
        [TestCase(new byte[] { 127 }, .5, true)]
        [TestCase(new byte[] { 128 }, .5, false)]
        [TestCase(new byte[] { 129 }, .5, false)]
        [TestCase(new byte[] { 255 }, 1, true)]
        public void When_checking_a_feature_for_user_and_the_feature_is_set_to_a_non_zero_percent_rollout(byte[] hash, double percentage, bool expected)
        {
            var settings = new FeatureToggleSettings
            {
                FeatureId = "test-feature",
                Enabled = true,
                RolloutPercentage = percentage
            };

            mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(hash);

            var result = classUnderTest.IsFeatureOnForUser(settings, "test-user");

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void When_checking_a_feature_for_user_and_the_user_is_on_the_include_list()
        {
            var settings = new FeatureToggleSettings
            {
                FeatureId = "test-feature",
                Enabled = true,
                RolloutPercentage = 0,
                UsersToInclude = new List<string> { "test-user" }
            };

            mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(new byte[] { 255 });

            var result = classUnderTest.IsFeatureOnForUser(settings, "test-user");

            Assert.That(result, Is.True);
        }

        [Test]
        public void When_checking_a_feature_for_user_and_the_user_is_on_the_exclude_list()
        {
            var settings = new FeatureToggleSettings
            {
                FeatureId = "test-feature",
                Enabled = true,
                RolloutPercentage = 100,
                UsersToExclude = new List<string> { "test-user" }
            };

            mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(new byte[] { 0 });

            var result = classUnderTest.IsFeatureOnForUser(settings, "test-user");

            Assert.That(result, Is.False);
        }

        [Test]
        public void When_checking_a_feature_for_user_by_feature_id_and_the_feature_is_not_found()
        {
            mockSettingsRepository.Setup(x => x.Find(It.IsAny<string>())).Returns<FeatureToggleSettings>(null);

            var result = classUnderTest.IsFeatureOnForUser("test-feature-toggle", "test-user");

            Assert.That(result, Is.False);
        }

        [Test]
        public void When_checking_a_feature_for_user_by_feature_id_and_the_feature_is_found()
        {
            mockSettingsRepository.Setup(x => x.Find(It.IsAny<string>())).Returns(new FeatureToggleSettings {
                    FeatureId = "test-feature-toggle",
                    Enabled = true,
                    RolloutPercentage = 100
            });

            mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(new byte[] { 123 });

            var result = classUnderTest.IsFeatureOnForUser("test-feature-toggle", "test-user");

            Assert.That(result, Is.True);
        }
    }
}
