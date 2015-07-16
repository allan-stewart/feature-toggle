namespace FeatureToggle
{
    public interface IToggle
    {
        bool IsFeatureOn(string config, string identifier);
    }
}