using UnityEditor;
using UnityEngine;

public class ProceduralAssetPostprocessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        if (assetPath.Contains("Procedural"))
        {
            ModelImporter modelImporter = (ModelImporter)assetImporter;
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
        }
    }

    void OnPostprocessModel(GameObject gameObject)
    {
        if (assetPath.Contains("Procedural"))
        {
            string[] pathParts = assetPath.Split('/');
            string assetName = pathParts[pathParts.Length - 1].Split('.')[0];
            string assetFolder = string.Join("/", pathParts, 0, pathParts.Length - 1);

            // Create a prefab
            string prefabPath = $"{assetFolder}/{assetName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);

            // Further steps would involve finding the correct material from the palette and assigning it.
        }
    }
}
