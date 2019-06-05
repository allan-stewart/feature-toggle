namespace FeatureToggle
{
    public class FeatureToggleEvaluator : IFeatureToggleEvaluator
    {
        readonly IHasher hasher;
        readonly IFeatureToggleSettingsRepository repository;

        public FeatureToggleEvaluator(IHasher hasher, IFeatureToggleSettingsRepository repository)
        {
            this.hasher = hasher;
            this.repository = repository;
        }

        public bool IsFeatureOnForUser(string featureId, string userIdentifier)
        {
            var settings = repository.Find(featureId);
            return (settings == null) ? false : IsFeatureOnForUser(settings, userIdentifier);
        }

        public bool IsFeatureOnForUser(FeatureToggleSettings settings, string userIdentifier)
        {
            if (!settings.Enabled)
            {
                return false;
            }

            if (settings.UsersToExclude.Contains(userIdentifier))
            {
                return false;
            }

            if (settings.UsersToInclude.Contains(userIdentifier))
            {
                return true;
            }

            return GetPercentageForUser(settings, userIdentifier) < settings.RolloutPercentage;
        }

        public double GetPercentageForUser(FeatureToggleSettings settings, string userIdentifer)
        {
            var bytes = hasher.Hash(settings.FeatureId + userIdentifer);
            return bytes[0] / 256d;
        }
    }
}
