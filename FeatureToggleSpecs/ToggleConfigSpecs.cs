using System;
using FeatureToggle;
using NUnit.Framework;

namespace FeatureToggleSpecs
{
    [TestFixture]
    public class ToggleConfigSpecs
    {
        [TestCase("on")]
        [TestCase("On")]
        [TestCase("ON")]
        public void When_parsing_ON(string config)
        {
            var result = new ToggleConfig(config);

            Assert.That(result.ConfigType, Is.EqualTo(ToggleConfigType.On));
        }

        [TestCase("off")]
        [TestCase("Off")]
        [TestCase("OFF")]
        public void When_parsing_OFF(string config)
        {
            var result = new ToggleConfig(config);

            Assert.That(result.ConfigType, Is.EqualTo(ToggleConfigType.Off));
        }

        [TestCase("")]
        [TestCase("true")]
        [TestCase("false")]
        [TestCase("durh")]
        [TestCase("71892")]
        [TestCase("ALK@#*(")]
        public void When_an_invalid_config_is_provided(string config)
        {
            var exception = Assert.Throws<ToggleConfigException>(() => new ToggleConfig(config));

            Assert.That(exception.Message.StartsWith("Invalid toggle configuration: " + config));
            Assert.That(exception.ParamName, Is.EqualTo("config"));
            Assert.That(exception.InnerException, Is.Null);
        }

        [TestCase("percent=0", 0)]
        [TestCase("percent=1", 1)]
        [TestCase("percent=.25", .25)]
        public void When_parsing_a_percent(string config, double expectedPercent)
        {
            var result = new ToggleConfig(config);

            Assert.That(result.ConfigType, Is.EqualTo(ToggleConfigType.Percent));
            Assert.That(result.Percent, Is.EqualTo(expectedPercent));
            Assert.That(result.PercentGroup, Is.EqualTo(0));
        }

        [TestCase("percent=0,group=1", 0, 1)]
        [TestCase("percent=1,group=2", 1, 2)]
        [TestCase("percent=.25,group=0", .25, 0)]
        public void When_parsing_a_percent_with_a_group(string config, double expectedPercent, int expectedGroup)
        {
            var result = new ToggleConfig(config);

            Assert.That(result.ConfigType, Is.EqualTo(ToggleConfigType.Percent));
            Assert.That(result.Percent, Is.EqualTo(expectedPercent));
            Assert.That(result.PercentGroup, Is.EqualTo(expectedGroup));
        }

        [TestCase("percent=")]
        [TestCase("percent=wat")]
        [TestCase("percent=.3,")]
        [TestCase("percent=.3,group=")]
        [TestCase("percent=.3,group=x")]
        public void When_parsing_an_invalid_percent(string config)
        {
            var exception = Assert.Throws<ToggleConfigException>(() => new ToggleConfig(config));

            Assert.That(exception.Message.StartsWith("Invalid toggle configuration: " + config));
            Assert.That(exception.ParamName, Is.EqualTo("config"));
            Assert.That(exception.InnerException, Is.Not.Null);
        }

        [TestCase("start=2015-07-16T01:40:00Z", "2015-07-16T01:40:00Z")]
        public void When_parsing_a_start_timestamp(string config, string expectedStartString)
        {
            var result = new ToggleConfig(config);

            Assert.That(result.ConfigType, Is.EqualTo(ToggleConfigType.Start));
            Assert.That(result.StartUtc, Is.EqualTo(DateTime.Parse(expectedStartString).ToUniversalTime()));
        }

        [TestCase("start=")]
        [TestCase("start=2015-07-16T01")]
        [TestCase("start=wat")]
        public void When_parsing_an_invalid_start_timestamp(string config)
        {
            var exception = Assert.Throws<ToggleConfigException>(() => new ToggleConfig(config));

            Assert.That(exception.Message.StartsWith("Invalid toggle configuration: " + config));
            Assert.That(exception.ParamName, Is.EqualTo("config"));
            Assert.That(exception.InnerException, Is.TypeOf<FormatException>());
        }

        [TestCase("end=2015-07-16T01:40:00Z", "2015-07-16T01:40:00Z")]
        public void When_parsing_a_end_timestamp(string config, string expectedEndString)
        {
            var result = new ToggleConfig(config);

            Assert.That(result.ConfigType, Is.EqualTo(ToggleConfigType.End));
            Assert.That(result.EndUtc, Is.EqualTo(DateTime.Parse(expectedEndString).ToUniversalTime()));
        }

        [TestCase("end=")]
        [TestCase("end=2015-07-16T01")]
        [TestCase("end=wat")]
        public void When_parsing_an_invalid_end_timestamp(string config)
        {
            var exception = Assert.Throws<ToggleConfigException>(() => new ToggleConfig(config));

            Assert.That(exception.Message.StartsWith("Invalid toggle configuration: " + config));
            Assert.That(exception.ParamName, Is.EqualTo("config"));
            Assert.That(exception.InnerException, Is.TypeOf<FormatException>());
        }

        [TestCase("between=2015-07-16T01:40:00Z,2015-08-16T02:30:00Z", "2015-07-16T01:40:00Z", "2015-08-16T02:30:00Z")]
        public void When_parsing_a_between_config(string config, string expectedStartString, string expectedEndString)
        {
            var result = new ToggleConfig(config);

            Assert.That(result.ConfigType, Is.EqualTo(ToggleConfigType.Between));
            Assert.That(result.StartUtc, Is.EqualTo(DateTime.Parse(expectedStartString).ToUniversalTime()));
            Assert.That(result.EndUtc, Is.EqualTo(DateTime.Parse(expectedEndString).ToUniversalTime()));
        }

        [TestCase("between=")]
        [TestCase("between=2015-07-16T01:40:00Z")]
        [TestCase("between=2015-07-16T01:40:00Z,")]
        [TestCase("between=2015-07-16T01:40:00Z,201")]
        [TestCase("between=x,2015-07-16T01:40:00Z")]
        [TestCase("between=2015-07-16T01")]
        [TestCase("between=wat")]
        public void When_parsing_an_invalid_between_config(string config)
        {
            var exception = Assert.Throws<ToggleConfigException>(() => new ToggleConfig(config));

            Assert.That(exception.Message.StartsWith("Invalid toggle configuration: " + config));
            Assert.That(exception.ParamName, Is.EqualTo("config"));
            Assert.That(exception.InnerException, Is.Not.Null);
        }
    }
}