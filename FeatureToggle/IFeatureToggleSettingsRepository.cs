using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureToggle
{
    public interface IFeatureToggleSettingsRepository
    {
        FeatureToggleSettings Find(string featureId);
        void Save(FeatureToggleSettings settings);
    }
}
