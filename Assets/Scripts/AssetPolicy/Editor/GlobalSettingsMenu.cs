using UnityEditor;

using UnityEngine;

namespace ProjectAction.AssetPolicy.Editor
{
    internal static class GlobalSettingsMenu
    {
        private const string ASSET_PATH = "Assets/Resources/GlobalSettings.asset";

        [MenuItem("Project/Asset Policy/Use Full (Primary Preferred)")]
        private static void _UseFull()
        {
            var settings = _GetOrCreateSettings();
            settings.hideFlags = HideFlags.None;
            _SetMode(settings, AssetAccessMode.Full);
        }

        [MenuItem("Project/Asset Policy/Use Restricted (Fallback Only)")]
        private static void _UseRestricted()
        {
            var settings = _GetOrCreateSettings();
            settings.hideFlags = HideFlags.None;
            _SetMode(settings, AssetAccessMode.Restricted);
        }

        private static GlobalSettings _GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<GlobalSettings>(ASSET_PATH);
            if (settings != null)
            {
                return settings;
            }

            settings = ScriptableObject.CreateInstance<GlobalSettings>();
            AssetDatabase.CreateAsset(settings, ASSET_PATH);
            AssetDatabase.SaveAssets();
            return settings;
        }

        private static void _SetMode(GlobalSettings settings, AssetAccessMode mode)
        {
            var serializedObject = new SerializedObject(settings);
            serializedObject.FindProperty("_assetAccessMode").enumValueIndex = (int)mode;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}
