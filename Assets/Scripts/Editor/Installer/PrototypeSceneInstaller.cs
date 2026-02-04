using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ProjectAction.Editor.Installers
{
    public static class PrototypeSceneInstaller
    {
        private readonly struct PlatformDefinition
        {
            public PlatformDefinition(string name, Vector3 position, Vector3 scale)
            {
                Name = name;
                Position = position;
                Scale = scale;
            }

            public string Name { get; }
            public Vector3 Position { get; }
            public Vector3 Scale { get; }
        }

        [MenuItem("Tools/Prototype/Install Parkour Vertical Slice v0")]
        public static void Install()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.isLoaded)
            {
                Debug.LogWarning("No active scene loaded.");
                return;
            }

            var root = Object.FindFirstObjectByType<ProjectAction.Core.RootScene>();
            if (root == null)
            {
                var rootObject = new GameObject("RootScene");
                Undo.RegisterCreatedObjectUndo(rootObject, "Create RootScene");
                root = rootObject.AddComponent<ProjectAction.Core.RootScene>();
            }

            var player = Object.FindFirstObjectByType<ProjectAction.Player.PlayerController>();
            if (player == null)
            {
                var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                playerObject.name = "Player";
                Undo.RegisterCreatedObjectUndo(playerObject, "Create Player");
                var capsuleCollider = playerObject.GetComponent<CapsuleCollider>();
                if (capsuleCollider != null)
                {
                    Object.DestroyImmediate(capsuleCollider);
                }
                var controller = playerObject.AddComponent<CharacterController>();
                player = playerObject.AddComponent<ProjectAction.Player.PlayerController>();

                var serializedPlayer = new SerializedObject(player);
                serializedPlayer.FindProperty("_controller").objectReferenceValue = controller;
                serializedPlayer.ApplyModifiedPropertiesWithoutUndo();
            }

            var courseRoot = EnsureCourseRoot();
            BuildCourse(courseRoot.transform);

            var cameraComponent = UnityEngine.Camera.main;
            if (cameraComponent == null)
            {
                var cameraObject = new GameObject("Main Camera");
                Undo.RegisterCreatedObjectUndo(cameraObject, "Create Camera");
                cameraComponent = cameraObject.AddComponent<UnityEngine.Camera>();
                cameraObject.tag = "MainCamera";
            }

            var brain = cameraComponent.GetComponent<CinemachineBrain>();
            if (brain == null)
            {
                brain = cameraComponent.gameObject.AddComponent<CinemachineBrain>();
            }

            var legacyThirdPerson = cameraComponent.GetComponent<ProjectAction.Camera.ThirdPersonCamera>();
            if (legacyThirdPerson != null)
            {
                Object.DestroyImmediate(legacyThirdPerson);
            }

            var virtualCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            if (virtualCamera == null)
            {
                var cameraRigObject = new GameObject("CinemachineCamera");
                Undo.RegisterCreatedObjectUndo(cameraRigObject, "Create Cinemachine Camera");
                virtualCamera = cameraRigObject.AddComponent<CinemachineCamera>();
            }

            var orbitalFollow = virtualCamera.GetComponent<CinemachineOrbitalFollow>();
            if (orbitalFollow == null)
            {
                orbitalFollow = virtualCamera.gameObject.AddComponent<CinemachineOrbitalFollow>();
            }

            var rotationComposer = virtualCamera.GetComponent<CinemachineRotationComposer>();
            if (rotationComposer == null)
            {
                rotationComposer = virtualCamera.gameObject.AddComponent<CinemachineRotationComposer>();
            }

            var orbitalCamera = virtualCamera.GetComponent<ProjectAction.Camera.CinemachineOrbitalCamera>();
            if (orbitalCamera == null)
            {
                orbitalCamera = virtualCamera.gameObject.AddComponent<ProjectAction.Camera.CinemachineOrbitalCamera>();
            }

            var serializedVirtualCamera = new SerializedObject(virtualCamera);
            var targetProperty = serializedVirtualCamera.FindProperty("Target");
            targetProperty.FindPropertyRelative("TrackingTarget").objectReferenceValue = player.transform;
            targetProperty.FindPropertyRelative("LookAtTarget").objectReferenceValue = player.transform;
            targetProperty.FindPropertyRelative("CustomLookAtTarget").boolValue = true;
            serializedVirtualCamera.ApplyModifiedPropertiesWithoutUndo();

            var serializedOrbitalCamera = new SerializedObject(orbitalCamera);
            serializedOrbitalCamera.FindProperty("_virtualCamera").objectReferenceValue = virtualCamera;
            serializedOrbitalCamera.FindProperty("_orbitalFollow").objectReferenceValue = orbitalFollow;
            serializedOrbitalCamera.FindProperty("_rotationComposer").objectReferenceValue = rotationComposer;
            serializedOrbitalCamera.FindProperty("_target").objectReferenceValue = player.transform;
            serializedOrbitalCamera.ApplyModifiedPropertiesWithoutUndo();

            var spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint == null)
            {
                spawnPoint = new GameObject("SpawnPoint");
                Undo.RegisterCreatedObjectUndo(spawnPoint, "Create SpawnPoint");
            }
            SetTransform(spawnPoint.transform, new Vector3(0f, 1.5f, -4f), Quaternion.identity, Vector3.one, "Update SpawnPoint");

            var virtualInputObject = GameObject.Find("VirtualInputBridge");
            if (virtualInputObject == null)
            {
                virtualInputObject = new GameObject("VirtualInputBridge");
                Undo.RegisterCreatedObjectUndo(virtualInputObject, "Create VirtualInputBridge");
                virtualInputObject.AddComponent<ProjectAction.Input.VirtualInputBridge>();
            }

            var checkpoint = GameObject.Find("Checkpoint");
            if (checkpoint == null)
            {
                checkpoint = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                checkpoint.name = "Checkpoint";
                Undo.RegisterCreatedObjectUndo(checkpoint, "Create Checkpoint");
            }
            SetTransform(checkpoint.transform, new Vector3(0f, 3.5f, 27f), Quaternion.identity, Vector3.one, "Update Checkpoint");
            ConfigureTriggerMarker(
                checkpoint,
                "Checkpoint",
                new Vector3(3.5f, 2.5f, 3.5f),
                new Color(1f, 0.95f, 0.2f, 0.35f),
                typeof(ProjectAction.Checkpoint.CheckpointTrigger));

            var goal = GameObject.Find("Goal");
            if (goal == null)
            {
                goal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                goal.name = "Goal";
                Undo.RegisterCreatedObjectUndo(goal, "Create Goal");
            }
            SetTransform(goal.transform, new Vector3(24f, 2.5f, 34f), Quaternion.identity, Vector3.one, "Update Goal");
            ConfigureTriggerMarker(
                goal,
                "Goal",
                new Vector3(4.5f, 2.5f, 4.5f),
                new Color(1f, 0.95f, 0.2f, 0.35f),
                typeof(ProjectAction.Checkpoint.GoalTrigger));

            var floor = GameObject.Find("PrototypeFloor");
            if (floor == null)
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "PrototypeFloor";
                Undo.RegisterCreatedObjectUndo(floor, "Create PrototypeFloor");
            }
            SetTransform(floor.transform, new Vector3(12f, -12f, 18f), Quaternion.identity, Vector3.one * 6f, "Update PrototypeFloor");

            if (player != null)
            {
                SetTransform(player.transform, new Vector3(0f, 1.5f, -4f), Quaternion.identity, Vector3.one, "Update Player");
            }

            var serializedRoot = new SerializedObject(root);
            serializedRoot.FindProperty("_player").objectReferenceValue = player;
            serializedRoot.FindProperty("_camera").objectReferenceValue = orbitalCamera;
            serializedRoot.FindProperty("_virtualInput").objectReferenceValue =
                virtualInputObject.GetComponent<ProjectAction.Input.VirtualInputBridge>();
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                "Assets/InputSystem_Actions.inputactions");
            serializedRoot.FindProperty("_inputActions").objectReferenceValue = inputActions;
            serializedRoot.FindProperty("_spawnPoint").objectReferenceValue = spawnPoint.transform;
            serializedRoot.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Installed Phase 1 parkour vertical slice objects into the active scene.");
        }

        private static GameObject EnsureCourseRoot()
        {
            var courseRoot = GameObject.Find("PrototypeCourse");
            if (courseRoot == null)
            {
                courseRoot = new GameObject("PrototypeCourse");
                Undo.RegisterCreatedObjectUndo(courseRoot, "Create PrototypeCourse");
            }

            return courseRoot;
        }

        private static void BuildCourse(Transform courseRoot)
        {
            var platforms = new[]
            {
                new PlatformDefinition("StartPlatform", new Vector3(0f, 0.5f, 0f), new Vector3(8f, 1f, 12f)),
                new PlatformDefinition("Step01", new Vector3(0f, 1.5f, 10f), new Vector3(4f, 1f, 4f)),
                new PlatformDefinition("Step02", new Vector3(3f, 2.5f, 16f), new Vector3(4f, 1f, 4f)),
                new PlatformDefinition("Step03", new Vector3(-3f, 3.5f, 22f), new Vector3(4f, 1f, 4f)),
                new PlatformDefinition("Runway", new Vector3(0f, 2.5f, 30f), new Vector3(6f, 1f, 10f)),
                new PlatformDefinition("TurnPlatform", new Vector3(8f, 2.5f, 34f), new Vector3(6f, 1f, 6f)),
                new PlatformDefinition("Sprint01", new Vector3(16f, 1.5f, 34f), new Vector3(6f, 1f, 10f)),
                new PlatformDefinition("GoalPlatform", new Vector3(24f, 1.5f, 34f), new Vector3(8f, 1f, 8f)),
            };

            foreach (var platform in platforms)
            {
                var platformObject = EnsurePlatform(courseRoot, platform.Name);
                SetTransform(platformObject.transform, platform.Position, Quaternion.identity, platform.Scale, $"Update {platform.Name}");
            }
        }

        private static void ConfigureTriggerMarker(
            GameObject marker,
            string markerName,
            Vector3 scale,
            Color inactiveColor,
            System.Type triggerType)
        {
            if (marker == null)
            {
                return;
            }

            var legacyTrigger = marker.transform.Find($"{markerName}Trigger");
            if (legacyTrigger != null)
            {
                Undo.DestroyObjectImmediate(legacyTrigger.gameObject);
            }

            marker.transform.localScale = scale;

            var collider = marker.GetComponent<Collider>();
            if (collider != null)
            {
                Undo.RecordObject(collider, $"Update {markerName} Collider");
                collider.isTrigger = true;
            }

            var trigger = marker.GetComponent(triggerType) ?? marker.AddComponent(triggerType);
            if (trigger != null)
            {
                Undo.RegisterCreatedObjectUndo(trigger, $"Add {triggerType.Name}");
            }

            var visual = marker.GetComponent<ProjectAction.Checkpoint.TriggerVisual>();
            if (visual == null)
            {
                visual = marker.AddComponent<ProjectAction.Checkpoint.TriggerVisual>();
                Undo.RegisterCreatedObjectUndo(visual, $"Add {markerName} Visual");
            }
            visual.SetInactive();

            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                ApplyTransparentMaterial(renderer, inactiveColor, $"Update {markerName} Material");
            }
        }

        private static GameObject EnsurePlatform(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = name;
            platform.transform.SetParent(parent, false);
            Undo.RegisterCreatedObjectUndo(platform, $"Create {name}");
            return platform;
        }

        private static void ApplyTransparentMaterial(Renderer renderer, Color baseColor, string undoName)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Standard");
            if (shader == null)
            {
                return;
            }

            Undo.RecordObject(renderer, undoName);
            var material = renderer.sharedMaterial;
            if (material == null || material.shader != shader)
            {
                material = new Material(shader);
                renderer.sharedMaterial = material;
            }

            ConfigureTransparentMaterial(material);
            material.SetColor("_BaseColor", baseColor);
        }

        private static void ConfigureTransparentMaterial(Material material)
        {
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_AlphaClip", 0f);
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        private static void SetTransform(
            Transform target,
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            string undoName)
        {
            Undo.RecordObject(target, undoName);
            target.position = position;
            target.rotation = rotation;
            target.localScale = scale;
        }
    }
}
