using UnityEditor;
using UnityEngine;
using System.IO;

public class ProceduralAssetPostprocessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        if (!assetPath.Contains("Procedural")) return;
        Debug.Log($"PROCEDURAL_GEN: Preprocessing model: {assetPath}");
        ModelImporter modelImporter = (ModelImporter)assetImporter;
        modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
    }

    void OnPostprocessModel(GameObject gameObject)
    {
        if (!assetPath.Contains("Procedural")) return;
        Debug.Log($"PROCEDURAL_GEN: Postprocessing model: {assetPath}");

        // Add a BoxCollider
        if (gameObject.GetComponent<BoxCollider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            Debug.Log($"PROCEDURAL_GEN: Added BoxCollider to {gameObject.name}");
        }

        // --- Prefab Generation ---
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetFolder = Path.GetDirectoryName(assetPath);
        string prefabPath = Path.Combine(assetFolder, $"{assetName}.prefab");

        // Check if prefab already exists
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            Debug.Log($"PROCEDURAL_GEN: Prefab already exists at {prefabPath}. Overwriting.");
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, prefabPath, InteractionMode.AutomatedAction);
        }
        else
        {
            Debug.Log($"PROCEDURAL_GEN: Creating new prefab at {prefabPath}");
            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
        }
    }
}
