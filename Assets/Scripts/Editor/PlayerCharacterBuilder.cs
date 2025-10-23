using System.IO;
using CameraSystem;
using Player;
using UnityEditor;
using UnityEngine;

namespace Art.Editor
{
    public static class PlayerCharacterBuilder
    {
        private const string PlayerFolder = "Assets/Art/Characters/Player";
        private const string PlayerPrefabPath = PlayerFolder + "/PlayerCharacter.prefab";
        private const float CharacterHeight = 1.8f;
        private const float CharacterRadius = 0.3f;

        [MenuItem("Art/Generate/Build Final Player Character")]
        public static void BuildPlayer()
        {
            var bodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Mat_PrimaryConcrete.mat");
            var accentMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Mat_AccentMagenta.mat");

            if (bodyMat == null || accentMat == null)
            {
                Debug.LogError("PlayerCharacterBuilder: Missing required materials. Aborting generation.");
                return;
            }

            EnsureFolder(PlayerFolder);

            var root = new GameObject("PlayerCharacter");
            try
            {
                var characterController = root.AddComponent<CharacterController>();
                characterController.height = CharacterHeight;
                characterController.radius = CharacterRadius;
                characterController.center = new Vector3(0f, CharacterHeight * 0.5f, 0f);
                characterController.stepOffset = 0.45f;
                characterController.slopeLimit = 45f;

                var playerInput = root.AddComponent<PlayerInput>();
                var playerController = root.AddComponent<PlayerController>();

                var visualRoot = new GameObject("VisualRoot").transform;
                visualRoot.SetParent(root.transform, false);

                var rootPivot = new GameObject("RootPivot").transform;
                rootPivot.SetParent(visualRoot, false);

                // Core body
                var hips = CreateSegment("Hips", rootPivot, new Vector3(0f, 0.9f, 0f), Vector3.zero, new Vector3(0.34f, 0.26f, 0.26f), bodyMat);
                var torso = CreateSegment("Torso", hips, new Vector3(0f, 0.44f, 0f), Vector3.zero, new Vector3(0.32f, 0.72f, 0.26f), bodyMat);
                var head = CreateSegment("Head", torso, new Vector3(0f, 0.48f, 0f), new Vector3(0f, 0.22f, 0f), new Vector3(0.28f, 0.32f, 0.28f), bodyMat);

                // Accent visor
                var visorMesh = CreateSegmentMesh("Visor", head, new Vector3(0f, 0.08f, 0.18f), new Vector3(0.24f, 0.12f, 0.04f), accentMat);
                var leftUpperArm = CreateSegment("LeftUpperArm", torso, new Vector3(-0.32f, 0.28f, 0f), new Vector3(0f, -0.24f, 0f), new Vector3(0.18f, 0.52f, 0.18f), bodyMat);
                var leftLowerArm = CreateSegment("LeftLowerArm", leftUpperArm, new Vector3(0f, -0.46f, 0f), new Vector3(0f, -0.22f, 0f), new Vector3(0.16f, 0.46f, 0.16f), bodyMat);
                var rightUpperArm = CreateSegment("RightUpperArm", torso, new Vector3(0.32f, 0.28f, 0f), new Vector3(0f, -0.24f, 0f), new Vector3(0.18f, 0.52f, 0.18f), bodyMat);
                var rightLowerArm = CreateSegment("RightLowerArm", rightUpperArm, new Vector3(0f, -0.46f, 0f), new Vector3(0f, -0.22f, 0f), new Vector3(0.16f, 0.46f, 0.16f), bodyMat);

                // Legs
                var leftUpperLeg = CreateSegment("LeftUpperLeg", hips, new Vector3(-0.18f, -0.32f, 0f), new Vector3(0f, -0.28f, 0f), new Vector3(0.2f, 0.58f, 0.2f), bodyMat);
                var leftLowerLeg = CreateSegment("LeftLowerLeg", leftUpperLeg, new Vector3(0f, -0.42f, 0f), new Vector3(0f, -0.26f, 0f), new Vector3(0.18f, 0.5f, 0.18f), bodyMat);
                var rightUpperLeg = CreateSegment("RightUpperLeg", hips, new Vector3(0.18f, -0.32f, 0f), new Vector3(0f, -0.28f, 0f), new Vector3(0.2f, 0.58f, 0.2f), bodyMat);
                var rightLowerLeg = CreateSegment("RightLowerLeg", rightUpperLeg, new Vector3(0f, -0.42f, 0f), new Vector3(0f, -0.26f, 0f), new Vector3(0.18f, 0.5f, 0.18f), bodyMat);

                var leftFoot = CreateSegment("LeftFoot", leftLowerLeg, new Vector3(0f, -0.18f, 0.02f), new Vector3(0f, -0.05f, 0.12f), new Vector3(0.24f, 0.1f, 0.34f), bodyMat);
                var rightFoot = CreateSegment("RightFoot", rightLowerLeg, new Vector3(0f, -0.18f, 0.02f), new Vector3(0f, -0.05f, 0.12f), new Vector3(0.24f, 0.1f, 0.34f), bodyMat);

                var driver = root.AddComponent<PlayerProceduralAnimator>();

                var cameraRig = new GameObject("PlayerCameraRig");
                cameraRig.transform.SetParent(root.transform, false);

                var cameraPivot = new GameObject("CameraPivot").transform;
                cameraPivot.SetParent(cameraRig.transform, false);
                cameraPivot.localPosition = Vector3.zero;

                var cameraGo = new GameObject("PlayerCamera");
                cameraGo.transform.SetParent(cameraPivot, false);
                var cameraComponent = cameraGo.AddComponent<Camera>();
                cameraComponent.tag = "MainCamera";
                cameraComponent.nearClipPlane = 0.05f;
                cameraComponent.fieldOfView = 65f;
                cameraGo.AddComponent<AudioListener>();

                var cameraDriver = cameraRig.AddComponent<ThirdPersonCamera>();
                var cameraSO = new SerializedObject(cameraDriver);
                cameraSO.FindProperty("followTarget").objectReferenceValue = root.transform;
                cameraSO.FindProperty("rotationTarget").objectReferenceValue = root.transform;
                cameraSO.FindProperty("pivot").objectReferenceValue = cameraPivot;
                cameraSO.FindProperty("cameraTransform").objectReferenceValue = cameraGo.transform;
                cameraSO.ApplyModifiedPropertiesWithoutUndo();
                AssignAnimatorBindings(driver, rootPivot, hips, torso, head, leftUpperArm, leftLowerArm, rightUpperArm, rightLowerArm, leftUpperLeg, leftLowerLeg, rightUpperLeg, rightLowerLeg, leftFoot, rightFoot, visorMesh);

                var controllerSO = new SerializedObject(playerController);
                controllerSO.FindProperty("leanPivot").objectReferenceValue = torso;
                controllerSO.FindProperty("animationDriver").objectReferenceValue = driver;
                controllerSO.ApplyModifiedPropertiesWithoutUndo();

                var animatorSO = new SerializedObject(driver);
                animatorSO.ApplyModifiedPropertiesWithoutUndo();

                AssetDatabase.DeleteAsset(PlayerPrefabPath);
                PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
                Debug.Log("PlayerCharacterBuilder: PlayerCharacter prefab generated at " + PlayerPrefabPath);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static Transform CreateSegment(string name, Transform parent, Vector3 bonePosition, Vector3 meshOffset, Vector3 meshScale, Material material)
        {
            var bone = new GameObject(name).transform;
            bone.SetParent(parent, false);
            bone.localPosition = bonePosition;
            bone.localRotation = Quaternion.identity;

            var mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.name = name + "_Mesh";
            mesh.transform.SetParent(bone, false);
            mesh.transform.localPosition = meshOffset;
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localScale = meshScale;
            RemoveCollider(mesh);
            var renderer = mesh.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;

            return bone;
        }

        private static Transform CreateSegmentMesh(string name, Transform parent, Vector3 meshOffset, Vector3 meshScale, Material material)
        {
            var mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.name = name;
            mesh.transform.SetParent(parent, false);
            mesh.transform.localPosition = meshOffset;
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localScale = meshScale;
            RemoveCollider(mesh);
            var renderer = mesh.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            return mesh.transform;
        }

        private static void AssignAnimatorBindings(PlayerProceduralAnimator driver, Transform rootPivot, Transform hips, Transform torso, Transform head,
            Transform leftUpperArm, Transform leftLowerArm, Transform rightUpperArm, Transform rightLowerArm,
            Transform leftUpperLeg, Transform leftLowerLeg, Transform rightUpperLeg, Transform rightLowerLeg,
            Transform leftFoot, Transform rightFoot, Transform accent)
        {
            var driverSO = new SerializedObject(driver);
            driverSO.FindProperty("rootPivot").objectReferenceValue = rootPivot;
            driverSO.FindProperty("hips").objectReferenceValue = hips;
            driverSO.FindProperty("torso").objectReferenceValue = torso;
            driverSO.FindProperty("head").objectReferenceValue = head;
            driverSO.FindProperty("leftUpperArm").objectReferenceValue = leftUpperArm;
            driverSO.FindProperty("leftLowerArm").objectReferenceValue = leftLowerArm;
            driverSO.FindProperty("rightUpperArm").objectReferenceValue = rightUpperArm;
            driverSO.FindProperty("rightLowerArm").objectReferenceValue = rightLowerArm;
            driverSO.FindProperty("leftUpperLeg").objectReferenceValue = leftUpperLeg;
            driverSO.FindProperty("leftLowerLeg").objectReferenceValue = leftLowerLeg;
            driverSO.FindProperty("rightUpperLeg").objectReferenceValue = rightUpperLeg;
            driverSO.FindProperty("rightLowerLeg").objectReferenceValue = rightLowerLeg;
            driverSO.FindProperty("leftFoot").objectReferenceValue = leftFoot;
            driverSO.FindProperty("rightFoot").objectReferenceValue = rightFoot;
            driverSO.FindProperty("accentNode").objectReferenceValue = accent;
            driverSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var segments = path.Split('/');
            var current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                var next = segments[i];
                var combined = Path.Combine(current, next).Replace('\\', '/');
                if (!AssetDatabase.IsValidFolder(combined))
                {
                    AssetDatabase.CreateFolder(current, next);
                }

                current = combined;
            }
        }
    }
}
