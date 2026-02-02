using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectAction.Editor.Installers
{
    public static class PrototypeSceneInstaller
    {
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
                playerObject.transform.position = new Vector3(0f, 1f, 0f);
                Undo.RegisterCreatedObjectUndo(playerObject, "Create Player");
                var controller = playerObject.AddComponent<CharacterController>();
                player = playerObject.AddComponent<ProjectAction.Player.PlayerController>();

                var serializedPlayer = new SerializedObject(player);
                serializedPlayer.FindProperty("_controller").objectReferenceValue = controller;
                serializedPlayer.ApplyModifiedPropertiesWithoutUndo();
            }

            var cameraComponent = UnityEngine.Camera.main;
            if (cameraComponent == null)
            {
                var cameraObject = new GameObject("Main Camera");
                Undo.RegisterCreatedObjectUndo(cameraObject, "Create Camera");
                cameraComponent = cameraObject.AddComponent<UnityEngine.Camera>();
                cameraObject.tag = "MainCamera";
            }

            var thirdPerson = cameraComponent.GetComponent<ProjectAction.Camera.ThirdPersonCamera>();
            if (thirdPerson == null)
            {
                thirdPerson = cameraComponent.gameObject.AddComponent<ProjectAction.Camera.ThirdPersonCamera>();
            }

            var serializedCamera = new SerializedObject(thirdPerson);
            serializedCamera.FindProperty("_target").objectReferenceValue = player.transform;
            serializedCamera.ApplyModifiedPropertiesWithoutUndo();

            var spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint == null)
            {
                spawnPoint = new GameObject("SpawnPoint");
                spawnPoint.transform.position = player.transform.position;
                Undo.RegisterCreatedObjectUndo(spawnPoint, "Create SpawnPoint");
            }

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
                checkpoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
                checkpoint.name = "Checkpoint";
                checkpoint.transform.position = new Vector3(0f, 0.5f, 8f);
                Undo.RegisterCreatedObjectUndo(checkpoint, "Create Checkpoint");
                var collider = checkpoint.GetComponent<BoxCollider>();
                collider.isTrigger = true;
                checkpoint.AddComponent<ProjectAction.Checkpoint.CheckpointTrigger>();
            }

            var goal = GameObject.Find("Goal");
            if (goal == null)
            {
                goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
                goal.name = "Goal";
                goal.transform.position = new Vector3(0f, 0.5f, 18f);
                Undo.RegisterCreatedObjectUndo(goal, "Create Goal");
                var collider = goal.GetComponent<BoxCollider>();
                collider.isTrigger = true;
                goal.AddComponent<ProjectAction.Checkpoint.GoalTrigger>();
            }

            var floor = GameObject.Find("PrototypeFloor");
            if (floor == null)
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "PrototypeFloor";
                floor.transform.position = Vector3.zero;
                Undo.RegisterCreatedObjectUndo(floor, "Create PrototypeFloor");
            }

            var serializedRoot = new SerializedObject(root);
            serializedRoot.FindProperty("_player").objectReferenceValue = player;
            serializedRoot.FindProperty("_camera").objectReferenceValue = thirdPerson;
            serializedRoot.FindProperty("_virtualInput").objectReferenceValue =
                virtualInputObject.GetComponent<ProjectAction.Input.VirtualInputBridge>();
            serializedRoot.FindProperty("_spawnPoint").objectReferenceValue = spawnPoint.transform;
            serializedRoot.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Installed Phase 1 parkour vertical slice objects into the active scene.");
        }
    }
}
