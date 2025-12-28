using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace inetz.ifinance.app.Services
{
    public sealed class TokenResponse
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public int ExpiresIn { get; set; } // seconds
    }
    public static class SecureStoreService
    {
        public static async Task<bool> SetAsync ( string key, string value )
        {
        
            try
            {
                await SecureStorage.SetAsync(key, value);
                return true;
            }
            catch
            {
                Preferences.Set(key, value);
                return false;
            }
        }

        public static async Task<string?> GetAsync ( string key )
        {
            try
            {
                return await SecureStorage.GetAsync(key);
            }
            catch
            {
                return Preferences.Get(key, null);
            }
        }
    }

    public class DeviceService
    {
        private const string DeviceMetaKeyV1 = "app_device_meta_v1";
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string ExpiryKey = "token_expiry";
        private const string UserBinVerifiedKeyV1 = "bin_verified_v1";
        private const string UserBindKeyV1 = "bin_v1";

        public async Task SaveAsync ( TokenResponse token )
        {
            await SecureStorage.SetAsync(AccessTokenKey, token.AccessToken);
            await SecureStorage.SetAsync(RefreshTokenKey, token.RefreshToken);

            var expiry = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
            await SecureStorage.SetAsync(ExpiryKey, expiry.ToUnixTimeSeconds().ToString());
        }

        public async Task<string?> GetAccessTokenAsync ()
            => await SecureStorage.GetAsync(AccessTokenKey);

        public async Task<bool> HasValidTokenAsync ()
        {
            var expiryStr = await SecureStorage.GetAsync(ExpiryKey);
            if (!long.TryParse(expiryStr, out var expiry))
                return false;

            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() < expiry;
        }

        public async Task ClearAsync ()
        {
            await  Task.Run(() => ClearToken());
           
        }

        private void ClearToken ()
        {
            SecureStorage.Remove(AccessTokenKey);
            SecureStorage.Remove(RefreshTokenKey);
            SecureStorage.Remove(ExpiryKey);
        }
        // Public API
        public async Task<DeviceMetaV1> GetDeviceIdAsync ()
        {
            // Attempt to migrate if necessary
           // await TryMigrateV1ToV2Async();

            // Try V2 first
            var v1 = await ReadMetaV1Async();
            //if (v1 != null && !string.IsNullOrEmpty(v1.Id))
            return v1 ?? new DeviceMetaV1(); 

          
        }

        public async Task<string> CreateDeviceIdAsync ( string? devcieId )
        {
           
            if ( devcieId == null ) return string.Empty;

            // Fallback: create and store V1 meta
            var newMeta = CreateV1Meta(devcieId);
            await SaveMetaV1Async(newMeta);
            return newMeta.Id;
        }
       
        // helpers for read/save V2
        private async Task<DeviceMetaV1?> ReadMetaV1Async ()
        {
            try
            {
                var json = await SecureStoreService.GetAsync(DeviceMetaKeyV1);
              
                if (string.IsNullOrWhiteSpace(json)) return null;
                return JsonSerializer.Deserialize<DeviceMetaV1>(json);
            }
            catch
            {
               
                 return null;
            }
        }

        private async Task SaveMetaV1Async ( DeviceMetaV1 meta )
        {
            var json = JsonSerializer.Serialize(meta);

            var result=   await SecureStoreService.SetAsync(DeviceMetaKeyV1, json);
           
        }

        private DeviceMetaV1 CreateV1Meta (string id)
            => new DeviceMetaV1
            {
                Id = id,
                CreatedAtUtc = DateTime.UtcNow.ToString("o"),
                AppVersion = AppInfo.VersionString ?? string.Empty,
                Platform = DeviceInfo.Platform.ToString(),
                Signature = string.Empty
            };

        

        public class DeviceMetaV1
        {
            public string Id { get; set; } = string.Empty;
            public string CreatedAtUtc { get; set; } = string.Empty;
            public string AppVersion { get; set; } = string.Empty;
            public string Platform { get; set; } = string.Empty;
            public string Signature { get; set; } = string.Empty;
        }



    }
}
