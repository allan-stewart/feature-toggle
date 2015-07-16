using System;

namespace FeatureToggle
{
    public class ToggleConfigException : ArgumentException
    {
        public ToggleConfigException(string message, Exception innerException) : base("Invalid toggle configuration: " + message, "config", innerException)
        {
        }
    }
}