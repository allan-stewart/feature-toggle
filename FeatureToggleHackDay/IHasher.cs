namespace FeatureToggle
{
    public interface IHasher
    {
        byte[] Hash(string input);
    }
}