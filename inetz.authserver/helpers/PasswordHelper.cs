using inetz.auth.dbcontext.models;
using Microsoft.AspNetCore.Identity;

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
    }
}
