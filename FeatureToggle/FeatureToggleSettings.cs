using System.Collections.Generic;

namespace FeatureToggle
{
    public class FeatureToggleSettings
    {
        public string FeatureId { get; set; }
        public bool Enabled { get; set; }
        public double RolloutPercentage { get; set; }
        public IList<string> UsersToInclude { get; set; } = new List<string>();
        public IList<string> UsersToExclude { get; set; } = new List<string>();
    }
}
