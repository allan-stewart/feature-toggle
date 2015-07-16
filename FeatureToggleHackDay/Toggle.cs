using System;

namespace FeatureToggle
{
    public class Toggle
    {
        readonly IHasher hasher;

        public Toggle(IHasher hasher)
        {
            this.hasher = hasher;
        }

        public bool IsFeatureOn(string config, string identifier)
        {
            var toggleConfig = new ToggleConfig(config);

            switch (toggleConfig.ConfigType)
            {
                case ToggleConfigType.Off:
                    return false;
                case ToggleConfigType.On:
                    return true;
                case ToggleConfigType.Percent:
                    return IsIdentifierWithinPercentage(toggleConfig.Percent, identifier);
                case ToggleConfigType.Start:
                    return HasTimeArrived(toggleConfig.StartUtc);
                case ToggleConfigType.End:
                    return !HasTimeArrived(toggleConfig.EndUtc);
                case ToggleConfigType.Between:
                    return HasTimeArrived(toggleConfig.StartUtc) && !HasTimeArrived(toggleConfig.EndUtc);
                default:
                    throw new NotImplementedException();
            }
        }

        bool HasTimeArrived(DateTime timestamp)
        {
            return timestamp <= DateTime.UtcNow;
        }

        bool IsIdentifierWithinPercentage(double percent, string identifier)
        {
            var bytes = hasher.Hash(identifier);
            var result = bytes[0] / 255d;
            return result >= percent;
        }
    }
}
