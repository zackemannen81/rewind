using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Art.Editor
{
    public static class RetroArtBaselineGenerator
    {
        private const string RootArtPath = "Assets/Art";
        private const string CharactersPath = "Assets/Art/Characters";
        private const string PropsPath = "Assets/Art/Props";
        private const string LightingPath = "Assets/Art/Lighting";
        private const string MaterialsPath = "Assets/Art/Materials";
        private const string PaletteAssetPath = "Assets/Art/Palettes/RetroPalette_Default.asset";

        [MenuItem("Art/Generate/Build Core Art Baseline")]
        public static void BuildCoreArtBaseline()
        {
            EnsureFolders();

            var palette = AssetDatabase.LoadAssetAtPath<RetroPalette>(PaletteAssetPath);
            if (palette == null)
            {
                Debug.LogError($"Missing palette asset at {PaletteAssetPath}. Aborting generation.");
                return;
            }

            var matPrimaryBase = LoadRequired<Material>($"{MaterialsPath}/Mat_PrimaryBase.mat");
            var matPrimaryConcrete = LoadRequired<Material>($"{MaterialsPath}/Mat_PrimaryConcrete.mat");
            var matAccentMagenta = LoadRequired<Material>($"{MaterialsPath}/Mat_AccentMagenta.mat");
            var matAccentCyan = LoadRequired<Material>($"{MaterialsPath}/Mat_AccentCyan.mat");
            var matTertiaryOxide = LoadRequired<Material>($"{MaterialsPath}/Mat_TertiaryOxide.mat");
            var matTertiaryWarmGrey = LoadRequired<Material>($"{MaterialsPath}/Mat_TertiaryWarmGrey.mat");

            if (matPrimaryBase == null || matPrimaryConcrete == null || matAccentMagenta == null ||
                matAccentCyan == null || matTertiaryOxide == null || matTertiaryWarmGrey == null)
            {
                Debug.LogError("Missing one or more required materials. Aborting generation.");
                return;
            }

            GeneratePlayer(matPrimaryConcrete, matAccentMagenta);
            GenerateGuard(matPrimaryConcrete, matAccentCyan);
            GenerateDrone(matPrimaryBase, matAccentCyan);
            GenerateGenerator(matPrimaryConcrete, matAccentMagenta);
            GenerateTerminal(matPrimaryConcrete, matAccentCyan, matTertiaryWarmGrey);
            GenerateWristwatch(matPrimaryConcrete, matAccentMagenta);
            GenerateEchoAnchor(matPrimaryConcrete, matAccentMagenta);
            GeneratePostProcessingAssets();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Retro art baseline generated successfully.");
        }

        private static void EnsureFolders()
        {
            CreateFolderIfMissing("Assets", "Art");
            CreateFolderIfMissing(RootArtPath, "Characters");
            CreateFolderIfMissing(CharactersPath, "Player");
            CreateFolderIfMissing(CharactersPath, "Guard");
            CreateFolderIfMissing(CharactersPath, "Drone");
            CreateFolderIfMissing(RootArtPath, "Props");
            CreateFolderIfMissing(PropsPath, "Generator");
            CreateFolderIfMissing(PropsPath, "Terminal");
            CreateFolderIfMissing(PropsPath, "Wristwatch");
            CreateFolderIfMissing(PropsPath, "EchoAnchor");
            CreateFolderIfMissing(RootArtPath, "Lighting");
            CreateFolderIfMissing(LightingPath, "Profiles");
            CreateFolderIfMissing(LightingPath, "Volumes");
        }

        private static void GeneratePlayer(Material bodyMat, Material accentMat)
        {
            var go = new GameObject("Player_LowPoly");
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = new[] { bodyMat, accentMat };

            var mesh = CreateHumanoidMesh("Player_Body", 1.8f, 0.25f, 0.18f, 0.35f);
            mesh.subMeshCount = 2;
            mesh.SetTriangles(GeneratePrismTriangles(), 0);
            mesh.SetTriangles(GenerateAccentTriangles(), 1);
            meshFilter.sharedMesh = mesh;

            SavePrefab(CharactersPath + "/Player", go.name, go);
            Object.DestroyImmediate(go);
        }

        private static void GenerateGuard(Material bodyMat, Material accentMat)
        {
            var go = new GameObject("Guard_LowPoly");
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = new[] { bodyMat, accentMat };

            var mesh = CreateHumanoidMesh("Guard_Body", 1.9f, 0.3f, 0.2f, 0.4f, 0.1f);
            mesh.subMeshCount = 2;
            mesh.SetTriangles(GeneratePrismTriangles(), 0);
            mesh.SetTriangles(GenerateAccentTriangles(), 1);
            meshFilter.sharedMesh = mesh;

            SavePrefab(CharactersPath + "/Guard", go.name, go);
            Object.DestroyImmediate(go);
        }

        private static void GenerateDrone(Material bodyMat, Material accentMat)
        {
            var go = new GameObject("Drone_LowPoly");
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = new[] { bodyMat, accentMat };

            var mesh = CreateDroneMesh();
            meshFilter.sharedMesh = mesh;

            SavePrefab(CharactersPath + "/Drone", go.name, go);
            Object.DestroyImmediate(go);
        }

        private static void GenerateGenerator(Material shellMat, Material accentMat)
        {
            var go = new GameObject("Generator_Hero");
            var shell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            RemoveCollider(shell);
            shell.name = "Shell";
            shell.transform.SetParent(go.transform);
            shell.transform.localScale = new Vector3(1.6f, 1.0f, 1.0f);
            var shellRenderer = shell.GetComponent<MeshRenderer>();
            shellRenderer.sharedMaterial = shellMat;

            var accent = GameObject.CreatePrimitive(PrimitiveType.Cube);
            accent.name = "AccentPanel";
            accent.transform.SetParent(go.transform);
            RemoveCollider(accent);
            accent.transform.localScale = new Vector3(1.2f, 0.2f, 0.05f);
            accent.transform.localPosition = new Vector3(0f, 0.2f, 0.55f);
            accent.GetComponent<MeshRenderer>().sharedMaterial = accentMat;

            SavePrefab(PropsPath + "/Generator", go.name, go);
            Object.DestroyImmediate(go);
        }

        private static void GenerateTerminal(Material shellMat, Material screenMat, Material detailMat)
        {
            var go = new GameObject("Terminal_Diegetic");
            var baseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseCube.name = "Housing";
            baseCube.transform.SetParent(go.transform);
            RemoveCollider(baseCube);
            baseCube.transform.localScale = new Vector3(0.6f, 1.2f, 0.3f);
            baseCube.GetComponent<MeshRenderer>().sharedMaterial = shellMat;

            var screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
            screen.name = "Screen";
            screen.transform.SetParent(go.transform);
            RemoveCollider(screen);
            screen.transform.localScale = new Vector3(0.4f, 0.3f, 1f);
            screen.transform.localPosition = new Vector3(0f, 0.35f, 0.16f);
            screen.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            screen.GetComponent<MeshRenderer>().sharedMaterial = screenMat;

            var keyboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            keyboard.name = "Console";
            keyboard.transform.SetParent(go.transform);
            RemoveCollider(keyboard);
            keyboard.transform.localScale = new Vector3(0.5f, 0.08f, 0.2f);
            keyboard.transform.localPosition = new Vector3(0f, -0.4f, 0.1f);
            keyboard.GetComponent<MeshRenderer>().sharedMaterial = detailMat;

            SavePrefab(PropsPath + "/Terminal", go.name, go);
            Object.DestroyImmediate(go);
        }

        private static void GenerateWristwatch(Material shellMat, Material accentMat)
        {
            var go = new GameObject("Wristwatch_DiegeticUI");
            var face = GameObject.CreatePrimitive(PrimitiveType.Cube);
            face.name = "Face";
            face.transform.SetParent(go.transform);
            RemoveCollider(face);
            face.transform.localScale = new Vector3(0.3f, 0.05f, 0.4f);
            face.GetComponent<MeshRenderer>().sharedMaterial = accentMat;

            var strap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strap.name = "Strap";
            strap.transform.SetParent(go.transform);
            RemoveCollider(strap);
            strap.transform.localScale = new Vector3(0.1f, 0.02f, 1.0f);
            strap.transform.localPosition = new Vector3(0f, -0.015f, 0f);
            strap.GetComponent<MeshRenderer>().sharedMaterial = shellMat;

            SavePrefab(PropsPath + "/Wristwatch", go.name, go);
            Object.DestroyImmediate(go);
        }

        private static void GenerateEchoAnchor(Material shellMat, Material accentMat)
        {
            var go = new GameObject("EchoAnchor_Prop");
            var core = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RemoveCollider(core);
            core.name = "Core";
            core.transform.SetParent(go.transform);
            core.transform.localScale = new Vector3(0.4f, 0.2f, 0.4f);
            core.GetComponent<MeshRenderer>().sharedMaterial = shellMat;

            var halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RemoveCollider(halo);
            halo.name = "Halo";
            halo.transform.SetParent(go.transform);
            halo.transform.localScale = new Vector3(0.6f, 0.02f, 0.6f);
            halo.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            halo.GetComponent<MeshRenderer>().sharedMaterial = accentMat;

            SavePrefab(PropsPath + "/EchoAnchor", go.name, go);
            Object.DestroyImmediate(go);
        }

        private static void SavePrefab(string folderPath, string prefabName, GameObject root)
        {
            var prefabFolder = EnsureAssetPath(folderPath);
            var prefabPath = Path.Combine(prefabFolder, prefabName + ".prefab").Replace('\\', '/');
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }

        private static void GeneratePostProcessingAssets()
        {
            const string globalProfilePath = LightingPath + "/Profiles/Chapter1_GlobalProfile.asset";
            const string loopEndProfilePath = LightingPath + "/Profiles/Chapter1_LoopEndProfile.asset";

            RecreateProfile(globalProfilePath, BuildGlobalProfile);
            RecreateProfile(loopEndProfilePath, BuildLoopEndProfile);

            var globalVolumeGo = new GameObject("Chapter1_GlobalVolume");
            var globalVolume = globalVolumeGo.AddComponent<PostProcessVolume>();
            globalVolume.isGlobal = true;
            globalVolume.priority = 0f;
            globalVolume.weight = 1f;
            globalVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(globalProfilePath);

            SavePrefab(LightingPath + "/Volumes", globalVolumeGo.name, globalVolumeGo);
            Object.DestroyImmediate(globalVolumeGo);

            var loopEndVolumeGo = new GameObject("Chapter1_LoopEndVolume");
            var loopEndVolume = loopEndVolumeGo.AddComponent<PostProcessVolume>();
            loopEndVolume.isGlobal = true;
            loopEndVolume.priority = 10f;
            loopEndVolume.weight = 0f;
            loopEndVolume.sharedProfile = AssetDatabase.LoadAssetAtPath<PostProcessProfile>(loopEndProfilePath);

            SavePrefab(LightingPath + "/Volumes", loopEndVolumeGo.name, loopEndVolumeGo);
            Object.DestroyImmediate(loopEndVolumeGo);
        }

        private static void RecreateProfile(string assetPath, System.Action<PostProcessProfile> builder)
        {
            if (AssetDatabase.LoadAssetAtPath<PostProcessProfile>(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            var profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            AssetDatabase.CreateAsset(profile, assetPath);
            builder?.Invoke(profile);
            EditorUtility.SetDirty(profile);
        }

        private static void BuildGlobalProfile(PostProcessProfile profile)
        {
            var bloom = profile.AddSettings<Bloom>();
            bloom.intensity.Override(12f);
            bloom.threshold.Override(0.9f);
            bloom.softKnee.Override(0.5f);
            bloom.color.Override(Color.white);

            var chromatic = profile.AddSettings<ChromaticAberration>();
            chromatic.intensity.Override(0.2f);
            chromatic.fastMode.Override(true);

            var vignette = profile.AddSettings<Vignette>();
            vignette.intensity.Override(0.35f);
            vignette.smoothness.Override(0.7f);

            var grain = profile.AddSettings<Grain>();
            grain.intensity.Override(0.15f);
            grain.colored.Override(false);
            grain.size.Override(0.7f);

            var colorGrading = profile.AddSettings<ColorGrading>();
            colorGrading.postExposure.Override(0.2f);
            colorGrading.temperature.Override(-12f);
            colorGrading.tint.Override(4f);
            colorGrading.saturation.Override(-5f);
            colorGrading.hueShift.Override(-3f);
        }

        private static void BuildLoopEndProfile(PostProcessProfile profile)
        {
            var colorGrading = profile.AddSettings<ColorGrading>();
            colorGrading.saturation.Override(-40f);
            colorGrading.hueShift.Override(-10f);
            colorGrading.postExposure.Override(-0.3f);

            var chromatic = profile.AddSettings<ChromaticAberration>();
            chromatic.intensity.Override(0.6f);

            var vignette = profile.AddSettings<Vignette>();
            vignette.intensity.Override(0.45f);

            var grain = profile.AddSettings<Grain>();
            grain.intensity.Override(0.25f);
            grain.colored.Override(false);
            grain.size.Override(0.9f);
        }

        private static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static string EnsureAssetPath(string targetPath)
        {
            if (!AssetDatabase.IsValidFolder(targetPath))
            {
                var segments = targetPath.Split('/');
                var current = segments[0];
                for (int i = 1; i < segments.Length; i++)
                {
                    var next = segments[i];
                    var combined = current + "/" + next;
                    if (!AssetDatabase.IsValidFolder(combined))
                    {
                        AssetDatabase.CreateFolder(current, next);
                    }

                    current = combined;
                }
            }

            return targetPath;
        }

        private static void CreateFolderIfMissing(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder(parent + "/" + child))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static T LoadRequired<T>(string assetPath) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset == null)
            {
                Debug.LogError($"Missing required asset at {assetPath}");
            }

            return asset;
        }

        private static Mesh CreateHumanoidMesh(string meshName, float height, float torsoWidth, float torsoDepth, float shoulderWidth, float accentInset = 0.08f)
        {
            var halfDepth = torsoDepth * 0.5f;
            var halfShoulder = shoulderWidth * 0.5f;
            var hipWidth = torsoWidth * 0.6f;
            var halfHip = hipWidth * 0.5f;
            var waistHeight = height * 0.45f;
            var shoulderHeight = height * 0.9f;

            var vertices = new List<Vector3>
            {
                // hips
                new(-halfHip, 0f, -halfDepth),
                new(halfHip, 0f, -halfDepth),
                new(halfHip, 0f, halfDepth),
                new(-halfHip, 0f, halfDepth),

                // waist
                new(-torsoWidth * 0.5f, waistHeight, -halfDepth),
                new(torsoWidth * 0.5f, waistHeight, -halfDepth),
                new(torsoWidth * 0.5f, waistHeight, halfDepth),
                new(-torsoWidth * 0.5f, waistHeight, halfDepth),

                // shoulders
                new(-halfShoulder, shoulderHeight, -halfDepth),
                new(halfShoulder, shoulderHeight, -halfDepth),
                new(halfShoulder, shoulderHeight, halfDepth),
                new(-halfShoulder, shoulderHeight, halfDepth),

                // head top
                new(-torsoWidth * 0.25f, height, -halfDepth * 0.7f),
                new(torsoWidth * 0.25f, height, -halfDepth * 0.7f),
                new(torsoWidth * 0.25f, height, halfDepth * 0.7f),
                new(-torsoWidth * 0.25f, height, halfDepth * 0.7f)
            };

            var mesh = new Mesh
            {
                name = meshName
            };

            mesh.SetVertices(vertices);
            mesh.SetTriangles(GeneratePrismTriangles(), 0);

            var accentVertices = new List<Vector3>
            {
                new(-accentInset, waistHeight * 0.5f, -halfDepth * 0.2f),
                new(accentInset, waistHeight * 0.5f, -halfDepth * 0.2f),
                new(accentInset, waistHeight * 0.5f, halfDepth * 0.2f),
                new(-accentInset, waistHeight * 0.5f, halfDepth * 0.2f),
                new(-accentInset, waistHeight * 0.5f + 0.25f, -halfDepth * 0.2f),
                new(accentInset, waistHeight * 0.5f + 0.25f, -halfDepth * 0.2f),
                new(accentInset, waistHeight * 0.5f + 0.25f, halfDepth * 0.2f),
                new(-accentInset, waistHeight * 0.5f + 0.25f, halfDepth * 0.2f)
            };

            var combinedVertices = new List<Vector3>();
            combinedVertices.AddRange(vertices);
            combinedVertices.AddRange(accentVertices);
            mesh.SetVertices(combinedVertices);

            var triangles = GeneratePrismTriangles();
            mesh.subMeshCount = 2;
            mesh.SetTriangles(triangles, 0);

            var accentTris = GenerateOffsetPrismTriangles(vertices.Count, 8);
            mesh.SetTriangles(accentTris, 1);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static Mesh CreateDroneMesh()
        {
            var mesh = new Mesh
            {
                name = "Drone_Body"
            };

            const int segments = 8;
            const float radius = 0.45f;
            const float height = 0.2f;

            var vertices = new Vector3[segments * 2 + 2];
            for (int i = 0; i < segments; i++)
            {
                var angle = Mathf.PI * 2f * i / segments;
                var x = Mathf.Cos(angle) * radius;
                var z = Mathf.Sin(angle) * radius;
                vertices[i] = new Vector3(x, height * 0.5f, z);
                vertices[i + segments] = new Vector3(x, -height * 0.5f, z);
            }

            vertices[segments * 2] = new Vector3(0f, height * 0.5f, 0f);
            vertices[segments * 2 + 1] = new Vector3(0f, -height * 0.5f, 0f);
            mesh.vertices = vertices;

            var triangles = new List<int>();
            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;

                // side quad
                triangles.Add(i);
                triangles.Add(i + segments);
                triangles.Add(next);

                triangles.Add(next);
                triangles.Add(i + segments);
                triangles.Add(next + segments);

                // top fan
                triangles.Add(segments * 2);
                triangles.Add(i);
                triangles.Add(next);

                // bottom fan
                triangles.Add(segments * 2 + 1);
                triangles.Add(next + segments);
                triangles.Add(i + segments);
            }

            mesh.subMeshCount = 1;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static int[] GeneratePrismTriangles()
        {
            return new[]
            {
                0, 4, 5,
                0, 5, 1,
                1, 5, 6,
                1, 6, 2,
                2, 6, 7,
                2, 7, 3,
                3, 7, 4,
                3, 4, 0,
                4, 8, 9,
                4, 9, 5,
                5, 9, 10,
                5, 10, 6,
                6, 10, 11,
                6, 11, 7,
                7, 11, 8,
                7, 8, 4,
                8, 12, 13,
                8, 13, 9,
                9, 13, 14,
                9, 14, 10,
                10, 14, 15,
                10, 15, 11,
                11, 15, 12,
                11, 12, 8,
                12, 13, 14,
                12, 14, 15,
                0, 1, 2,
                0, 2, 3
            };
        }

        private static int[] GenerateAccentTriangles()
        {
            return new[]
            {
                0, 1, 2,
                0, 2, 3,
                4, 7, 6,
                4, 6, 5,
                0, 4, 5,
                0, 5, 1,
                1, 5, 6,
                1, 6, 2,
                2, 6, 7,
                2, 7, 3,
                3, 7, 4,
                3, 4, 0
            };
        }

        private static int[] GenerateOffsetPrismTriangles(int startIndex, int vertexCount)
        {
            var baseTris = GenerateAccentTriangles();
            var offsetTris = new int[baseTris.Length];
            for (int i = 0; i < baseTris.Length; i++)
            {
                offsetTris[i] = startIndex + baseTris[i];
            }

            return offsetTris;
        }
    }
}
