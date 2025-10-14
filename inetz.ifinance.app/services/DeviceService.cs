using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace inetz.ifinance.app.services
{
    public class DeviceService
    {
        private const string DeviceMetaKeyV1 = "app_device_meta_v1";
        private const string DeviceMetaKeyV2 = "app_device_meta_v2";

        // Public API
        public async Task<string> GetOrCreateDeviceIdAsync ()
        {
            // Attempt to migrate if necessary
            await TryMigrateV1ToV2Async();

            // Try V2 first
            var v2 = await ReadMetaV2Async();
            if (v2 != null && !string.IsNullOrEmpty(v2.Id))
                return v2.Id;

            // Fallback: create and store V2 meta
            var newMeta = CreateV2Meta();
            await SaveMetaV2Async(newMeta);
            return newMeta.Id;
        }

        // Migration: safe & idempotent
        private async Task TryMigrateV1ToV2Async ()
        {
            try
            {
                // If V2 already exists, nothing to do
                var existingV2 = await SecureStorage.GetAsync(DeviceMetaKeyV2);
                if (!string.IsNullOrWhiteSpace(existingV2))
                    return;

                // Read V1
                var v1Json = await SecureStorage.GetAsync(DeviceMetaKeyV1);
                if (string.IsNullOrWhiteSpace(v1Json))
                    return; // no V1 stored → nothing to migrate

                // Deserialize V1
                DeviceMetaV1? metaV1 = null;
                try
                {
                    metaV1 = JsonSerializer.Deserialize<DeviceMetaV1>(v1Json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"DeviceService: failed to parse V1 meta: {ex.Message}");
                }

                if (metaV1 == null || string.IsNullOrEmpty(metaV1.Id))
                {
                    // try Preferences as fallback
                    var pref = Preferences.Get(DeviceMetaKeyV1, null);
                    if (!string.IsNullOrEmpty(pref))
                    {
                        try { metaV1 = JsonSerializer.Deserialize<DeviceMetaV1>(pref); }
                        catch { /* can't parse → give up on migration */ }
                    }
                }

                if (metaV1 == null || string.IsNullOrEmpty(metaV1.Id))
                    return; // nothing valid to migrate

                // Build V2 from V1 values + extras
                var metaV2 = new DeviceMetaV2
                {
                    Id = metaV1.Id,
                    CreatedAtUtc = string.IsNullOrEmpty(metaV1.CreatedAtUtc) ? DateTime.UtcNow.ToString("o") : metaV1.CreatedAtUtc,
                    AppVersion = AppInfo.VersionString ?? string.Empty,
                    Platform = DeviceInfo.Platform.ToString(),
                    Signature = string.Empty // compute HMAC here if you want
                };

                // Save V2
                await SaveMetaV2Async(metaV2);

                // Optionally remove V1 (or keep for diagnostics)
                try
                {
                    SecureStorage.Remove(DeviceMetaKeyV1);
                }
                catch
                {
                    // ignore, not fatal
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeviceService: migration failed: {ex.Message}");
                // Do not throw - migration failures should not block startup
            }
        }

        // helpers for read/save V2
        private async Task<DeviceMetaV2?> ReadMetaV2Async ()
        {
            try
            {
                var json = await SecureStorage.GetAsync(DeviceMetaKeyV2);
                if (string.IsNullOrWhiteSpace(json)) return null;
                return JsonSerializer.Deserialize<DeviceMetaV2>(json);
            }
            catch
            {
                // fallback to preferences
                try
                {
                    var pref = Preferences.Get(DeviceMetaKeyV2, null);
                    if (string.IsNullOrEmpty(pref)) return null;
                    return JsonSerializer.Deserialize<DeviceMetaV2>(pref);
                }
                catch { return null; }
            }
        }

        private async Task SaveMetaV2Async ( DeviceMetaV2 meta )
        {
            var json = JsonSerializer.Serialize(meta);
            try
            {
                await SecureStorage.SetAsync(DeviceMetaKeyV2, json);
            }
            catch
            {
                Preferences.Set(DeviceMetaKeyV2, json);
            }
        }

        private DeviceMetaV2 CreateV2Meta ()
            => new DeviceMetaV2
            {
                Id = CreateSecureId(),
                CreatedAtUtc = DateTime.UtcNow.ToString("o"),
                AppVersion = AppInfo.VersionString ?? string.Empty,
                Platform = DeviceInfo.Platform.ToString(),
                Signature = string.Empty
            };

        private static string CreateSecureId ()
        {
            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        // v1 and v2 classes (private)
        private class DeviceMetaV1
        {
            public string Id { get; set; } = string.Empty;
            public string CreatedAtUtc { get; set; } = string.Empty;
        }

        private class DeviceMetaV2
        {
            public string Id { get; set; } = string.Empty;
            public string CreatedAtUtc { get; set; } = string.Empty;
            public string AppVersion { get; set; } = string.Empty;
            public string Platform { get; set; } = string.Empty;
            public string Signature { get; set; } = string.Empty;
        }



    }
}
