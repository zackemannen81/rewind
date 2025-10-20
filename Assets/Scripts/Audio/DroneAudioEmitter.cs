using System.Collections;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Handles spatialised drone audio using the shared soundscape library.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DroneAudioEmitter : MonoBehaviour
    {
        [SerializeField]
        private AudioSource hoverSource;
        [SerializeField]
        private AudioSource alertSource;
        [SerializeField]
        private float hoverVolume = 0.55f;
        [SerializeField]
        private float alertVolume = 0.85f;
        [SerializeField]
        private float crossFadeSeconds = 0.6f;

        private Coroutine _hoverFadeRoutine;
        private Coroutine _alertFadeRoutine;

        private void Awake()
        {
            EnsureSource(ref hoverSource, "HoverSource", loop: true);
            EnsureSource(ref alertSource, "AlertSource", loop: true);
        }

        private void OnEnable()
        {
            StartHoverLoop();
        }

        private void OnDisable()
        {
            hoverSource.Stop();
            alertSource.Stop();
        }

        public void SetAlertState(bool isAlert)
        {
            if (SoundscapeManager.Instance == null)
            {
                return;
            }

            if (isAlert)
            {
                if (!alertSource.isPlaying && SoundscapeManager.Instance.TryResolveCue(AudioCueId.DroneAlert, out var alertCue))
                {
                    alertCue.ApplyToSource(alertSource, alertVolume);
                    alertSource.loop = true;
                    alertSource.spatialBlend = 1f;
                    alertSource.Play();
                }

                Fade(ref hoverSource, ref _hoverFadeRoutine, Mathf.Max(0.25f, hoverVolume * 0.45f));
                Fade(ref alertSource, ref _alertFadeRoutine, alertVolume);
            }
            else
            {
                Fade(ref hoverSource, ref _hoverFadeRoutine, hoverVolume);
                Fade(ref alertSource, ref _alertFadeRoutine, 0f);
            }
        }

        private void StartHoverLoop()
        {
            if (SoundscapeManager.Instance == null)
            {
                return;
            }

            if (SoundscapeManager.Instance.TryResolveCue(AudioCueId.DroneHover, out var cue))
            {
                cue.ApplyToSource(hoverSource, hoverVolume);
                hoverSource.loop = true;
                hoverSource.spatialBlend = 1f;
                hoverSource.Play();
            }
        }

        private void EnsureSource(ref AudioSource source, string childName, bool loop)
        {
            if (source == null)
            {
                var child = new GameObject(childName);
                child.transform.SetParent(transform);
                child.transform.localPosition = Vector3.zero;
                source = child.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1.5f;
            source.maxDistance = 18f;
        }

        private void Fade(ref AudioSource source, ref Coroutine routine, float targetVolume)
        {
            if (source == null)
            {
                return;
            }

            if (routine != null)
            {
                StopCoroutine(routine);
            }

            routine = StartCoroutine(FadeRoutine(source, targetVolume));
        }

        private IEnumerator FadeRoutine(AudioSource source, float targetVolume)
        {
            var startVolume = source.volume;
            var elapsed = 0f;
            var duration = Mathf.Max(0.05f, crossFadeSeconds);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                source.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            source.volume = targetVolume;
            if (Mathf.Approximately(targetVolume, 0f))
            {
                source.Stop();
            }
        }
    }
}
