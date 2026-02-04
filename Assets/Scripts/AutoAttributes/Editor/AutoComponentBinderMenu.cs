using System.Collections.Generic;
using System.Linq;
using UnityEditor;

using UnityEngine;

namespace ProjectAction.AutoAttributes.Editor
{
    public static class AutoComponentBinderMenu
    {
        [MenuItem("Tools/Auto Attributes/Bind/Active Scene")]
        public static void BindActiveScene()
        {
            var targets = Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .Where(IsBindable)
                .ToArray();

            BindTargets(targets, false, "Bind Auto Attributes (Scene)");
        }

        [MenuItem("Tools/Auto Attributes/Bind/Active Scene (Overwrite Existing)")]
        public static void BindActiveSceneOverwrite()
        {
            var targets = Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .Where(IsBindable)
                .ToArray();

            BindTargets(targets, true, "Bind Auto Attributes (Scene Overwrite)");
        }

        [MenuItem("Tools/Auto Attributes/Bind/Current Prefab Stage")]
        public static void BindCurrentPrefabStage()
        {
            var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage == null || stage.prefabContentsRoot == null)
            {
                Debug.LogWarning("No prefab stage is currently open.");
                return;
            }

            var targets = stage.prefabContentsRoot
                .GetComponentsInChildren<MonoBehaviour>(true)
                .Where(IsBindable)
                .ToArray();

            BindTargets(targets, false, "Bind Auto Attributes (Prefab)");
        }

        [MenuItem("Tools/Auto Attributes/Bind/Current Prefab Stage (Overwrite Existing)")]
        public static void BindCurrentPrefabStageOverwrite()
        {
            var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage == null || stage.prefabContentsRoot == null)
            {
                Debug.LogWarning("No prefab stage is currently open.");
                return;
            }

            var targets = stage.prefabContentsRoot
                .GetComponentsInChildren<MonoBehaviour>(true)
                .Where(IsBindable)
                .ToArray();

            BindTargets(targets, true, "Bind Auto Attributes (Prefab Overwrite)");
        }

        private static bool IsBindable(MonoBehaviour behaviour)
        {
            return behaviour != null && AutoComponentBinder.HasAutoFields(behaviour.GetType());
        }

        private static void BindTargets(
            IReadOnlyCollection<MonoBehaviour> targets,
            bool overwriteExisting,
            string undoName)
        {
            if (targets == null || targets.Count == 0)
            {
                Debug.Log("No components with auto attributes were found.");
                return;
            }

            var updatedFields = 0;
            var updatedComponents = 0;
            targets.ForEach(target =>
            {
                Undo.RecordObject(target, undoName);
                var fieldCount = AutoComponentBinder.Bind(target, overwriteExisting);
                if (fieldCount <= 0)
                {
                    return;
                }

                updatedFields += fieldCount;
                updatedComponents++;
                EditorUtility.SetDirty(target);
            });

            if (updatedComponents > 0)
            {
                AssetDatabase.SaveAssets();
            }

            Debug.Log(
                $"Auto Attributes bind complete. Components updated: {updatedComponents}, fields updated: {updatedFields}.");
        }
    }
}
