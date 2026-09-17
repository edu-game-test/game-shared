using System.Security.Cryptography;
using System.Text;

namespace MetaFramework.Common
{
    /// <summary>bucket = SHA256(userId + ":" + key) mod 100, digest read as a big-endian unsigned integer (segmentation-system.md).</summary>
    public static class StableBucket
    {
        public static int Compute(string userId, string key)
        {
            using var sha = SHA256.Create();
            var digest = sha.ComputeHash(Encoding.UTF8.GetBytes(userId + ":" + key));
            // Big-endian unsigned integer mod 100, computed incrementally to avoid BigInteger.
            int mod = 0;
            foreach (var b in digest)
                mod = (mod * 256 + b) % 100;
            return mod;
        }
    }
}
