
using UnityEngine;

public class SoundscapeBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (SoundscapeManager.Instance == null)
        {
            GameObject soundscapeManagerPrefab = new GameObject("SoundscapeManager");
            soundscapeManagerPrefab.AddComponent<SoundscapeManager>();
        }
    }
}
