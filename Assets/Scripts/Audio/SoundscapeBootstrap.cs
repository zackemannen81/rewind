using UnityEngine;

namespace Audio
{
    public static class SoundscapeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureSoundscape()
        {
            if (SoundscapeManager.Instance != null)
            {
                return;
            }

            var root = new GameObject("SoundscapeManager");
            root.AddComponent<SoundscapeManager>();
        }
    }
}
