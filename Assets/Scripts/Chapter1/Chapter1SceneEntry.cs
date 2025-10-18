using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chapter1
{
    public static class Chapter1SceneEntry
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.name != Chapter1Constants.SceneNameBlockout)
            {
                return;
            }

            EnsureBootstrapExists();
        }

        private static void EnsureBootstrapExists()
        {
            var existing = Object.FindObjectOfType<Chapter1Bootstrap>();
            if (existing != null)
            {
                return;
            }

            var root = new GameObject("Chapter1Root");
            SceneManager.MoveGameObjectToScene(root, SceneManager.GetActiveScene());
            root.AddComponent<Chapter1Bootstrap>();
        }
    }
}
