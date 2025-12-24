using inetz.auth.dbcontext.models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace inetz.authserver.helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<UserProfile> _hasher = new();

        public static string HashPassword ( UserProfile user, string password )
        {
            return _hasher.HashPassword(user, password);
        }
        public static string HashDevice ( UserProfile user, string device )
        {
            return _hasher.HashPassword(user, device);
        }

        public static bool VerifyPassword ( UserProfile user, string hashedPassword, string password )
        {
            var result = _hasher.VerifyHashedPassword(user, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

        public static bool VerifyDevice ( UserProfile user, string hashedDevice, string deviceId )
        {
            var result = _hasher.VerifyHashedPassword(user, hashedDevice, deviceId);
            return result == PasswordVerificationResult.Success;
        }
        public static string GenerateBin ()
        {
            // Range: 00000 – 99999
            int value = RandomNumberGenerator.GetInt32(0, 100000);
            return value.ToString("D5"); // zero-padded
        }
        public static string HashBin ( string bin )
        {
            using var sha = SHA256.Create();
            byte [] bytes = Encoding.UTF8.GetBytes(bin);
            byte [] hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
