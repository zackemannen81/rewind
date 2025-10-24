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
            // Add a BoxCollider
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();

            // Calculate the bounds of the mesh
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            collider.center = bounds.center - gameObject.transform.position;
            collider.size = bounds.size;

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
