using System;
using UnityEngine;

namespace ProjectAction.AssetPolicy
{
    public static class AssetLoader
    {
        private const string PrimaryPrefix = "Primary/";
        private const string FallbackPrefix = "Fallback/";

        public static T Load<T>(string path) where T : Object
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _LogMissing(typeof(T), path, GlobalSettings.CurrentMode, Array.Empty<string>());
                return null;
            }

            var normalizedPath = path.TrimStart('/');
            return GlobalSettings.CurrentMode == AssetAccessMode.Restricted
                ? _LoadFallbackOnly<T>(normalizedPath)
                : _LoadPrimaryThenFallback<T>(normalizedPath);
        }

        private static T _LoadPrimaryThenFallback<T>(string path) where T : Object
        {
            var primaryPath = $"{PrimaryPrefix}{path}";
            var primary = Resources.Load<T>(primaryPath);
            if (primary != null)
            {
                return primary;
            }

            var fallbackPath = $"{FallbackPrefix}{path}";
            var fallback = Resources.Load<T>(fallbackPath);
            if (fallback != null)
            {
                return fallback;
            }

            _LogMissing(typeof(T), path, AssetAccessMode.Full, new[] { primaryPath, fallbackPath });
            return null;
        }

        private static T _LoadFallbackOnly<T>(string path) where T : Object
        {
            var fallbackPath = $"{FallbackPrefix}{path}";
            var fallback = Resources.Load<T>(fallbackPath);
            if (fallback != null)
            {
                return fallback;
            }

            _LogMissing(typeof(T), path, AssetAccessMode.Restricted, new[] { fallbackPath });
            return null;
        }

        private static void _LogMissing(Type assetType, string path, AssetAccessMode mode, string[] attempts)
        {
            var attemptText = attempts.Length > 0 ? string.Join(", ", attempts) : "none";
            Debug.LogWarning(
                $"AssetLoader: mode={mode}, path='{path}', type={assetType.Name}, attempts=[{attemptText}]");
        }
    }
}
