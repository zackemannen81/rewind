using UnityEngine;

namespace Audio
{
    public readonly struct AudioCue
    {
        public AudioCue(AudioClip clip, float defaultVolume = 1f, float pitch = 1f)
        {
            Clip = clip;
            DefaultVolume = Mathf.Clamp01(defaultVolume);
            Pitch = pitch;
        }

        public AudioClip Clip { get; }
        public float DefaultVolume { get; }
        public float Pitch { get; }

        public void ApplyToSource(AudioSource source, float volumeScale = 1f)
        {
            if (source == null || Clip == null)
            {
                return;
            }

            source.pitch = Pitch;
            source.volume = Mathf.Clamp01(DefaultVolume * volumeScale);
            source.clip = Clip;
        }
    }
}
