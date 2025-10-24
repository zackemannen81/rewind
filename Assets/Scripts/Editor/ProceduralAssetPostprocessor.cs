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
            Debug.Log($"PROCEDural_GEN: Added BoxCollider to {gameObject.name}");
        }

        // --- Prefab Generation ---
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetFolder = Path.GetDirectoryName(assetPath);
        string prefabPath = Path.Combine(assetFolder, $"{assetName}.prefab");

        Debug.Log($"PROCEDURAL_GEN: Saving prefab to {prefabPath}");
        PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath, out bool success);
        if (success)
        {
            Debug.Log("PROCEDURAL_GEN: Prefab saved successfully.");
        }
        else
        {
            Debug.LogError($"PROCEDURAL_GEN: Failed to save prefab for {assetName}");
        }
    }
}
