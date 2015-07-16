using System.Security.Cryptography;
using System.Text;

namespace FeatureToggle
{
    public class Hasher : IHasher
    {
        public byte[] Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
            }
        }
    }
}