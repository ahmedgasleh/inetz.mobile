using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace inetz.ifinance.app.Services
{
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
        //private const string DeviceMetaKeyV2 = "app_device_meta_v2";

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



        // Migration: safe & idempotent
        //private async Task TryMigrateV1ToV2Async ()
        //{
        //    try
        //    {
        //        // If V2 already exists, nothing to do
        //        var existingV2 = await SecureStorage.GetAsync(DeviceMetaKeyV2);
        //        if (!string.IsNullOrWhiteSpace(existingV2))
        //            return;

        //        // Read V1
        //        var v1Json = await SecureStorage.GetAsync(DeviceMetaKeyV1);
        //        if (string.IsNullOrWhiteSpace(v1Json))
        //            return; // no V1 stored → nothing to migrate

        //        // Deserialize V1
        //        DeviceMetaV1? metaV1 = null;
        //        try
        //        {
        //            metaV1 = JsonSerializer.Deserialize<DeviceMetaV1>(v1Json);
        //        }
        //        catch (Exception ex)
        //        {
        //            //Debug.WriteLine($"DeviceService: failed to parse V1 meta: {ex.Message}");
        //        }

        //        if (metaV1 == null || string.IsNullOrEmpty(metaV1.Id))
        //        {
        //            // try Preferences as fallback
        //            var pref = Preferences.Get(DeviceMetaKeyV1, null);
        //            if (!string.IsNullOrEmpty(pref))
        //            {
        //                try { metaV1 = JsonSerializer.Deserialize<DeviceMetaV1>(pref); }
        //                catch { /* can't parse → give up on migration */ }
        //            }
        //        }

        //        if (metaV1 == null || string.IsNullOrEmpty(metaV1.Id))
        //            return; // nothing valid to migrate

        //        // Build V2 from V1 values + extras
        //        var metaV2 = new DeviceMetaV2
        //        {
        //            Id = metaV1.Id,
        //            CreatedAtUtc = string.IsNullOrEmpty(metaV1.CreatedAtUtc) ? DateTime.UtcNow.ToString("o") : metaV1.CreatedAtUtc,
        //            AppVersion = AppInfo.VersionString ?? string.Empty,
        //            Platform = DeviceInfo.Platform.ToString(),
        //            Signature = string.Empty // compute HMAC here if you want
        //        };

        //        // Save V2
        //        await SaveMetaV2Async(metaV2);

        //        // Optionally remove V1 (or keep for diagnostics)
        //        try
        //        {
        //            SecureStorage.Remove(DeviceMetaKeyV1);
        //        }
        //        catch
        //        {
        //            // ignore, not fatal
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Debug.WriteLine($"DeviceService: migration failed: {ex.Message}");
        //        // Do not throw - migration failures should not block startup
        //    }
        //}

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
