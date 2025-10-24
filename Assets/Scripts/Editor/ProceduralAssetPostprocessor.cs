using System;
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

        // Use the material description from the .mtl file
        modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
        modelImporter.materialSearch = ModelImporterMaterialSearch.Local;
        modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
    }

    void OnPostprocessModel(GameObject gameObject)
    {
        if (!assetPath.Contains("Procedural")) return;
        Debug.Log($"PROCEDURAL_GEN: Postprocessing model: {gameObject.name}");

        // Add a BoxCollider if one doesn't exist
        if (gameObject.GetComponent<BoxCollider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            Debug.Log($"PROCEDURAL_GEN: Added BoxCollider to {gameObject.name}");
        }

        // --- Prefab Generation ---
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetFolder = (Path.GetDirectoryName(assetPath) ?? "Assets/Art/Procedural").Replace('\\', '/');
        string prefabName = assetName.EndsWith("_PFB", StringComparison.OrdinalIgnoreCase) ? assetName : assetName + "_PFB";
        string prefabPath = Path.Combine(assetFolder, prefabName + ".prefab").Replace('\\', '/');

        Debug.Log($"PROCEDURAL_GEN: Saving prefab to {prefabPath}");

        GameObject prefabRoot = Object.Instantiate(gameObject);
        prefabRoot.name = prefabName;

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath, out bool success);
        Object.DestroyImmediate(prefabRoot);

        if (success)
        {
            Debug.Log($"PROCEDURAL_GEN: Prefab saved successfully to {prefabPath}.");
        }
        else
        {
            Debug.LogError($"PROCEDURAL_GEN: Failed to save prefab for {assetName}");
        }
    }
}
