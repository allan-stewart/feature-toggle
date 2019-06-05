using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureToggle
{
    interface IFeatureToggleEvaluator
    {
        bool IsFeatureOnForUser(string featureId, string userIdentifier);
    }
}
