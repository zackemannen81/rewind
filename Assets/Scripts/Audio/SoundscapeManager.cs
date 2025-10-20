using System.Collections;
using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Centralises procedural audio playback for ambience, SFX and diegetic cues.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class SoundscapeManager : MonoBehaviour
    {
        [Header("Mix Busses")]
        [SerializeField]
        private AudioSource ambientPrimary;
        [SerializeField]
        private AudioSource ambientSecondary;
        [SerializeField]
        private AudioSource effectsSource;
        [SerializeField]
        private AudioSource uiSource;
        [SerializeField]
        private AudioSource voiceSource;
        [SerializeField, Range(0f, 1f)]
        private float ambientPrimaryLevel = 0.7f;
        [SerializeField, Range(0f, 1f)]
        private float ambientSecondaryLevel = 0.45f;
        [SerializeField]
        private float ambientCrossFadeSeconds = 1.5f;

        private readonly Dictionary<AudioCueId, AudioCue> _library = new();
        private readonly HashSet<AudioCueId> _missingCueWarnings = new();

        private float _nextFootstepTime;
        private Coroutine _primaryFadeRoutine;
        private Coroutine _secondaryFadeRoutine;

        public static SoundscapeManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate SoundscapeManager detected; destroying the newest instance.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsureSource(ref ambientPrimary, "AmbientPrimary");
            EnsureSource(ref ambientSecondary, "AmbientSecondary");
            EnsureSource(ref effectsSource, "Effects");
            EnsureSource(ref uiSource, "UI");
            EnsureSource(ref voiceSource, "Voice");

            SeedLibrary();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<LoopStartEvent>(OnLoopStart);
            EventBus.Subscribe<LoopEndEvent>(OnLoopEnd);
            EventBus.Subscribe<MinutePassedEvent>(OnMinutePassed);
            EventBus.Subscribe<PlayerNoiseEvent>(OnPlayerNoise);
        }

        private void Start()
        {
            SeedAmbientLayer(AudioCueId.AmbientDistrictDrone, ambientPrimary, ambientPrimaryLevel);
            SeedAmbientLayer(AudioCueId.AmbientPulse, ambientSecondary, ambientSecondaryLevel);
        }

        public void PlayEffect(AudioCueId id, float volumeScale = 1f)
        {
            if (!TryGetCue(id, out var cue))
            {
                return;
            }

            cue.ApplyToSource(effectsSource, volumeScale);
            effectsSource.loop = false;
            effectsSource.Play();
        }

        public void PlayUiCue(AudioCueId id, float volumeScale = 1f)
        {
            if (!TryGetCue(id, out var cue))
            {
                return;
            }

            cue.ApplyToSource(uiSource, volumeScale);
            uiSource.loop = false;
            uiSource.Play();
        }

        public void PlayVoiceCue(AudioCueId id, float volumeScale = 1f)
        {
            if (!TryGetCue(id, out var cue))
            {
                return;
            }

            cue.ApplyToSource(voiceSource, volumeScale);
            voiceSource.loop = false;
            voiceSource.Play();
        }

        public void PlayAmbientLayer(AudioCueId id, AudioSource source, float volumeScale, bool loop)
        {
            if (!TryGetCue(id, out var cue))
            {
                return;
            }

            cue.ApplyToSource(source, volumeScale);
            source.loop = loop;
            if (!source.isPlaying)
            {
                source.Play();
            }
        }

        public void TriggerInteractionFeedback()
        {
            PlayEffect(AudioCueId.InteractionClick, 1f);
        }

        public void TriggerUiGlitch()
        {
            PlayUiCue(AudioCueId.UiGlitch, 1f);
        }

        public bool TryResolveCue(AudioCueId id, out AudioCue cue)
        {
            return TryGetCue(id, out cue);
        }

        private void OnLoopStart(LoopStartEvent evt)
        {
            PlayEffect(AudioCueId.LoopStartStutter, 1f);

            EnsureAmbientClip(AudioCueId.AmbientDistrictDrone, ambientPrimary, ambientPrimaryLevel);
            EnsureAmbientClip(AudioCueId.AmbientPulse, ambientSecondary, ambientSecondaryLevel);

            FadeAmbient(ambientPrimary, GetAmbientTargetVolume(AudioCueId.AmbientDistrictDrone, ambientPrimaryLevel));
            FadeAmbient(ambientSecondary, GetAmbientTargetVolume(AudioCueId.AmbientPulse, ambientSecondaryLevel));
        }

        private void OnLoopEnd(LoopEndEvent evt)
        {
            PlayEffect(AudioCueId.LoopEndCollapse, 1f);
            FadeAmbient(ambientPrimary, 0f);
            FadeAmbient(ambientSecondary, 0f);
        }

        private void OnMinutePassed(MinutePassedEvent evt)
        {
            var proximity = Mathf.Clamp01(1f - evt.MinutesRemaining / 7f);
            PlayUiCue(AudioCueId.LoopTick, Mathf.Lerp(0.2f, 0.7f, proximity));
        }

        private void OnPlayerNoise(PlayerNoiseEvent evt)
        {
            if (!evt.IsMoving)
            {
                return;
            }

            var normalizedNoise = Mathf.Clamp01(evt.NoiseLevel);
            var cadence = Mathf.Lerp(0.52f, 0.24f, normalizedNoise);
            if (Time.time < _nextFootstepTime)
            {
                return;
            }

            PlayEffect(AudioCueId.FootstepConcrete, Mathf.Lerp(0.4f, 1f, normalizedNoise));
            _nextFootstepTime = Time.time + cadence;
        }

        private void EnsureSource(ref AudioSource source, string childName)
        {
            if (source != null)
            {
                return;
            }

            var child = new GameObject(childName);
            child.transform.SetParent(transform);
            source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
        }

        private void SeedLibrary()
        {
            _library.Clear();
            foreach (var kvp in ProceduralCueLibrary.BuildLibrary())
            {
                if (kvp.Value.Clip == null)
                {
                    Debug.LogWarning($"Audio cue {kvp.Key} returned a null clip.");
                    continue;
                }

                _library[kvp.Key] = kvp.Value;
            }
        }

        private bool TryGetCue(AudioCueId id, out AudioCue cue)
        {
            if (_library.TryGetValue(id, out cue))
            {
                return true;
            }

            if (_missingCueWarnings.Add(id))
            {
                Debug.LogWarning($"Audio cue {id} is missing from the library.");
            }

            return false;
        }

        private void SeedAmbientLayer(AudioCueId id, AudioSource source, float volumeScale)
        {
            if (!TryGetCue(id, out var cue))
            {
                return;
            }

            cue.ApplyToSource(source, volumeScale);
            source.loop = true;
            source.volume = GetAmbientTargetVolume(id, volumeScale);
            source.Play();
        }

        private void EnsureAmbientClip(AudioCueId id, AudioSource source, float volumeScale)
        {
            if (!TryGetCue(id, out var cue))
            {
                return;
            }

            var shouldRestart = !source.isPlaying || source.clip != cue.Clip;
            cue.ApplyToSource(source, volumeScale);
            source.loop = true;

            if (shouldRestart)
            {
                source.volume = 0f;
                source.Play();
            }
        }

        private float GetAmbientTargetVolume(AudioCueId id, float volumeScale)
        {
            return TryGetCue(id, out var cue)
                ? Mathf.Clamp01(cue.DefaultVolume * volumeScale)
                : 0f;
        }

        private void FadeAmbient(AudioSource target, float targetVolume)
        {
            if (target == null)
            {
                return;
            }

            if (target == ambientPrimary)
            {
                if (_primaryFadeRoutine != null)
                {
                    StopCoroutine(_primaryFadeRoutine);
                }

                _primaryFadeRoutine = StartCoroutine(FadeRoutine(target, targetVolume));
            }
            else if (target == ambientSecondary)
            {
                if (_secondaryFadeRoutine != null)
                {
                    StopCoroutine(_secondaryFadeRoutine);
                }

                _secondaryFadeRoutine = StartCoroutine(FadeRoutine(target, targetVolume));
            }
            else
            {
                StartCoroutine(FadeRoutine(target, targetVolume));
            }
        }

        private IEnumerator FadeRoutine(AudioSource source, float targetVolume)
        {
            var startVolume = source.volume;
            var elapsed = 0f;
            var duration = Mathf.Max(0.05f, ambientCrossFadeSeconds);

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
