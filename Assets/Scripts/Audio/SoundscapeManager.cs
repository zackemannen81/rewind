using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundscapeManager : MonoBehaviour
{
    #region Singleton
    public static SoundscapeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }
    #endregion

    public LoopStartEvent loopStartEvent;
    public LoopEndEvent loopEndEvent;
    public MinutePassedEvent minutePassedEvent;
    public PlayerNoiseEvent playerNoiseEvent;

    private ProceduralCueLibrary cueLibrary;
    private AudioSource audioSource;
    private AudioSource droneAudioSource;
    private AudioSource ambientAudioSource;
    private AudioSource pulseAudioSource;

    private void Initialize()
    {
        cueLibrary = new ProceduralCueLibrary();
        audioSource = GetComponent<AudioSource>();
        droneAudioSource = gameObject.AddComponent<AudioSource>();
        ambientAudioSource = gameObject.AddComponent<AudioSource>();
        pulseAudioSource = gameObject.AddComponent<AudioSource>();

        ambientAudioSource.clip = cueLibrary.GetClip(AudioCueId.AmbientDistrictDrone);
        ambientAudioSource.loop = true;
        ambientAudioSource.Play();

        pulseAudioSource.clip = cueLibrary.GetClip(AudioCueId.AmbientPulse);
        pulseAudioSource.loop = true;
        pulseAudioSource.Play();
    }

    private void OnEnable()
    {
        loopStartEvent.RegisterListener(OnLoopStart);
        loopEndEvent.RegisterListener(OnLoopEnd);
        minutePassedEvent.RegisterListener(OnMinutePassed);
        playerNoiseEvent.RegisterListener(OnPlayerNoise);
    }

    private void OnDisable()
    {
        loopStartEvent.UnregisterListener(OnLoopStart);
        loopEndEvent.UnregisterListener(OnLoopEnd);
        minutePassedEvent.UnregisterListener(OnMinutePassed);
        playerNoiseEvent.UnregisterListener(OnPlayerNoise);
    }

    private void OnLoopStart()
    {
        audioSource.PlayOneShot(cueLibrary.GetClip(AudioCueId.LoopStartStutter));
    }

    private void OnLoopEnd()
    {
        audioSource.PlayOneShot(cueLibrary.GetClip(AudioCueId.LoopEndCollapse));
    }

    private void OnMinutePassed()
    {
        audioSource.PlayOneShot(cueLibrary.GetClip(AudioCueId.LoopTick));
    }

    private void OnPlayerNoise(GameEvent @event)
    {
        PlayerNoiseEvent noiseEvent = @event as PlayerNoiseEvent;
        if (noiseEvent != null)
        {
            audioSource.PlayOneShot(cueLibrary.GetClip(AudioCueId.FootstepConcrete), noiseEvent.noiseLevel);
        }
    }

    public void TriggerInteractionFeedback()
    {
        audioSource.PlayOneShot(cueLibrary.GetClip(AudioCueId.InteractionClick));
    }

    public void TriggerUiGlitch()
    {
        audioSource.PlayOneShot(cueLibrary.GetClip(AudioCueId.UiGlitch));
    }

    public void PlayVoiceCue(AudioCueId cueId)
    {
        audioSource.PlayOneShot(cueLibrary.GetClip(cueId));
    }

    public void SetAlertState(bool isAlerted)
    {
        if (isAlerted)
        {
            droneAudioSource.clip = cueLibrary.GetClip(AudioCueId.DroneAlert);
        }
        else
        {
            droneAudioSource.clip = cueLibrary.GetClip(AudioCueId.DroneHover);
        }
        droneAudioSource.Play();
    }
}

public class ProceduralCueLibrary
{
    private Dictionary<AudioCueId, AudioClip> cues = new Dictionary<AudioCueId, AudioClip>();

    public ProceduralCueLibrary()
    {
        cues.Add(AudioCueId.LoopStartStutter, GenerateSineWave(440, 0.5f, 0.5f));
        cues.Add(AudioCueId.LoopEndCollapse, GenerateSineWave(880, 0.5f, 0.5f));
        cues.Add(AudioCueId.LoopTick, GenerateSineWave(1760, 0.1f, 0.2f));
        cues.Add(AudioCueId.InteractionClick, GenerateSineWave(3520, 0.1f, 0.5f));
        cues.Add(AudioCueId.FootstepConcrete, GenerateSineWave(100, 0.1f, 0.8f));
        cues.Add(AudioCueId.UiGlitch, GenerateSineWave(2000, 0.2f, 0.5f));
        cues.Add(AudioCueId.DroneHover, GenerateSineWave(220, 1f, 0.3f, true));
        cues.Add(AudioCueId.DroneAlert, GenerateSineWave(440, 1f, 0.6f, true));
        cues.Add(AudioCueId.RadioPacket, GenerateSineWave(1000, 1f, 0.5f));
        cues.Add(AudioCueId.AmbientDistrictDrone, GenerateSineWave(60, 10f, 0.1f, true));
        cues.Add(AudioCueId.AmbientPulse, GenerateSineWave(40, 2f, 0.2f, true));
    }

    public AudioClip GetClip(AudioCueId cueId)
    {
        return cues.ContainsKey(cueId) ? cues[cueId] : null;
    }

    private AudioClip GenerateSineWave(float frequency, float duration, float volume, bool loop = false)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(duration * sampleRate);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            samples[i] = volume * Mathf.Sin(2 * Mathf.PI * frequency * t);
        }

        AudioClip clip = AudioClip.Create("SineWave", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        clip.loop = loop;
        return clip;
    }
}

public enum AudioCueId
{
    AmbientDistrictDrone,
    AmbientPulse,
    LoopStartStutter,
    LoopEndCollapse,
    LoopTick,
    FootstepConcrete,
    InteractionClick,
    UiGlitch,
    DroneHover,
    DroneAlert,
    RadioPacket
}