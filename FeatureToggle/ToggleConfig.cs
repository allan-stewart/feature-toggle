using System;

namespace FeatureToggle
{
    public class ToggleConfig
    {
        public ToggleConfigType ConfigType { get; set; }
        public double Percent { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        public ToggleConfig(string config)
        {
            if (config.Equals("on", StringComparison.InvariantCultureIgnoreCase))
            {
                ConfigType = ToggleConfigType.On;
                return;
            }

            if (config.Equals("off", StringComparison.InvariantCultureIgnoreCase))
            {
                ConfigType = ToggleConfigType.Off;
                return;
            }

            if (config.StartsWith("percent=", StringComparison.InvariantCultureIgnoreCase))
            {
                ParsePercent(config);
                return;
            }

            if (config.StartsWith("start=", StringComparison.InvariantCultureIgnoreCase))
            {
                ParseStart(config);
                return;
            }

            if (config.StartsWith("end=", StringComparison.InvariantCultureIgnoreCase))
            {
                ParseEnd(config);
                return;
            }

            if (config.StartsWith("between=", StringComparison.InvariantCultureIgnoreCase))
            {
                ParseBetween(config);
                return;
            }

            throw new ToggleConfigException(config, null);
        }

        void ParsePercent(string config)
        {
            ConfigType = ToggleConfigType.Percent;
            try
            {
                Percent = Convert.ToDouble(config.Substring("percent=".Length));
            }
            catch (Exception exception)
            {
                throw new ToggleConfigException(config, exception);
            }
        }

        void ParseStart(string config)
        {
            ConfigType = ToggleConfigType.Start;
            try
            {
                StartUtc = DateTime.Parse(config.Substring("start=".Length)).ToUniversalTime();
            }
            catch (Exception exception)
            {
                throw new ToggleConfigException(config, exception);
            }
        }

        void ParseEnd(string config)
        {
            ConfigType = ToggleConfigType.End;
            try
            {
                EndUtc = DateTime.Parse(config.Substring("end=".Length)).ToUniversalTime();
            }
            catch (Exception exception)
            {
                throw new ToggleConfigException(config, exception);
            }
        }

        void ParseBetween(string config)
        {
            ConfigType = ToggleConfigType.Between;
            try
            {
                var parts = config.Substring("between=".Length).Split(',');
                StartUtc = DateTime.Parse(parts[0]).ToUniversalTime();
                EndUtc = DateTime.Parse(parts[1]).ToUniversalTime();
            }
            catch (Exception exception)
            {
                throw new ToggleConfigException(config, exception);
            }
        }
    }
}