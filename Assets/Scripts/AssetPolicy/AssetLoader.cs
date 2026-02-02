using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProjectAction.AssetPolicy
{
    public static class AssetLoader
    {
        private const string PRIMARY_PREFIX = "Primary/";
        private const string FALLBACK_PREFIX = "Fallback/";

        public static T Load<T>(string path) where T : Object
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                LogMissing(typeof(T), path, GlobalSettings.CurrentMode, Array.Empty<string>());
                return null;
            }

            var normalizedPath = path.TrimStart('/');
            return GlobalSettings.CurrentMode == AssetAccessMode.Restricted
                ? LoadFallbackOnly<T>(normalizedPath)
                : LoadPrimaryThenFallback<T>(normalizedPath);
        }

        private static T LoadPrimaryThenFallback<T>(string path) where T : Object
        {
            var primaryPath = $"{PRIMARY_PREFIX}{path}";
            var primary = Resources.Load<T>(primaryPath);
            if (primary != null)
            {
                return primary;
            }

            var fallbackPath = $"{FALLBACK_PREFIX}{path}";
            var fallback = Resources.Load<T>(fallbackPath);
            if (fallback != null)
            {
                return fallback;
            }

            LogMissing(typeof(T), path, AssetAccessMode.Full, new[] { primaryPath, fallbackPath });
            return null;
        }

        private static T LoadFallbackOnly<T>(string path) where T : Object
        {
            var fallbackPath = $"{FALLBACK_PREFIX}{path}";
            var fallback = Resources.Load<T>(fallbackPath);
            if (fallback != null)
            {
                return fallback;
            }

            LogMissing(typeof(T), path, AssetAccessMode.Restricted, new[] { fallbackPath });
            return null;
        }

        private static void LogMissing(Type assetType, string path, AssetAccessMode mode, string[] attempts)
        {
            var attemptText = attempts.Length > 0 ? string.Join(", ", attempts) : "none";
            Debug.LogWarning(
                $"AssetLoader: mode={mode}, path='{path}', type={assetType.Name}, attempts=[{attemptText}]");
        }
    }
}
