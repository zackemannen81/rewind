using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Generates the procedural audio palette used across the Chapter 1 vertical slice.
    /// </summary>
    public static class ProceduralCueLibrary
    {
        private const int SampleRate = 44100;

        public static IReadOnlyDictionary<AudioCueId, AudioCue> BuildLibrary()
        {
            return new Dictionary<AudioCueId, AudioCue>
            {
                { AudioCueId.AmbientDistrictDrone, CreateAmbientDrone() },
                { AudioCueId.AmbientPulse, CreateAmbientPulse() },
                { AudioCueId.FootstepConcrete, CreateFootstep() },
                { AudioCueId.InteractionClick, CreateInteractionClick() },
                { AudioCueId.UiGlitch, CreateUiGlitch() },
                { AudioCueId.LoopStartStutter, CreateLoopStart() },
                { AudioCueId.LoopEndCollapse, CreateLoopEnd() },
                { AudioCueId.LoopTick, CreateLoopTick() },
                { AudioCueId.DroneHover, CreateDroneHover() },
                { AudioCueId.DroneAlert, CreateDroneAlert() },
                { AudioCueId.RadioPacket, CreateRadioPacket() }
            };
        }

        private static AudioCue CreateAmbientDrone()
        {
            var clip = ProceduralAudioGenerator.CreateLoop(
                "AMB_District_Drone",
                8f,
                t => LayeredSine(t, (0.12f, 0.2f), (0.4f, 0.05f)) + FilteredNoise(t, 0.02f, 0.3f));
            return new AudioCue(clip, 0.55f);
        }

        private static AudioCue CreateAmbientPulse()
        {
            var clip = ProceduralAudioGenerator.CreateLoop(
                "AMB_Pulse",
                6f,
                t => 0.15f * Mathf.Sin(Mathf.PI * 2f * 0.35f * t) * Envelope(t % 6f, 0.4f, 0.8f));
            return new AudioCue(clip, 0.45f);
        }

        private static AudioCue CreateFootstep()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "SFX_Footstep_Concrete",
                0.28f,
                t => Envelope(t, 0.02f, 0.22f) * (FilteredNoise(t * 40f, 0.35f, 0.65f) + 0.2f * Mathf.Sin(2f * Mathf.PI * 120f * t)));
            return new AudioCue(clip, 0.8f, UnityEngine.Random.Range(0.9f, 1.1f));
        }

        private static AudioCue CreateInteractionClick()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "SFX_Interact_Click",
                0.18f,
                t => Envelope(t, 0.01f, 0.05f) * Mathf.Sin(2f * Mathf.PI * 680f * t) + Envelope(t, 0.05f, 0.07f) * Mathf.Sin(2f * Mathf.PI * 1200f * t));
            return new AudioCue(clip, 0.35f);
        }

        private static AudioCue CreateUiGlitch()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "SFX_UI_Glitch",
                0.4f,
                t => Envelope(t, 0.01f, 0.15f) * (BitCrush(Mathf.Sin(2f * Mathf.PI * (320f + 40f * Mathf.Sin(2f * Mathf.PI * 8f * t)) * t), 6) + 0.35f * FilteredNoise(t * 60f, 0.25f, 0.6f)));
            return new AudioCue(clip, 0.5f);
        }

        private static AudioCue CreateLoopStart()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "SFX_Loop_Start",
                1.8f,
                t => Envelope(t, 0.05f, 0.9f) * (Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(860f, 120f, t)) + 0.4f * FilteredNoise(t * 10f, 0.1f, 0.45f)));
            return new AudioCue(clip, 0.65f);
        }

        private static AudioCue CreateLoopEnd()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "SFX_Loop_End",
                2.2f,
                t => Envelope(t, 0.02f, 1.6f) * (Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(180f, 960f, t * t)) + 0.6f * FilteredNoise(t * 15f, 0.18f, 0.55f)));
            return new AudioCue(clip, 0.7f);
        }

        private static AudioCue CreateLoopTick()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "SFX_Loop_Tick",
                0.12f,
                t => Envelope(t, 0.005f, 0.03f) * Mathf.Sin(2f * Mathf.PI * 1600f * t));
            return new AudioCue(clip, 0.35f);
        }

        private static AudioCue CreateDroneHover()
        {
            var clip = ProceduralAudioGenerator.CreateLoop(
                "SFX_Drone_Hover",
                3.5f,
                t => 0.3f * Mathf.Sin(2f * Mathf.PI * 220f * t) + 0.2f * Mathf.Sin(2f * Mathf.PI * 440f * t + 0.5f * Mathf.Sin(2f * Mathf.PI * 2f * t)) + 0.25f * FilteredNoise(t * 22f, 0.1f, 0.35f));
            return new AudioCue(clip, 0.6f);
        }

        private static AudioCue CreateDroneAlert()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "SFX_Drone_Alert",
                1.4f,
                t => Envelope(t, 0.05f, 0.7f) * Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(480f, 960f, Mathf.Pow(t, 0.65f))));
            return new AudioCue(clip, 0.75f);
        }

        private static AudioCue CreateRadioPacket()
        {
            var clip = ProceduralAudioGenerator.CreateClip(
                "VO_Radio_Packet",
                3.2f,
                t => Envelope(t, 0.08f, 1.1f) * (
                    0.5f * FilteredNoise(t * 32f, 0.2f, 0.6f) +
                    0.35f * Mathf.Sin(2f * Mathf.PI * (220f + 20f * Mathf.Sin(2f * Mathf.PI * 2.3f * t)) * t) * (Square(2f * Mathf.PI * 4.2f * t) * 0.6f + 0.4f)
                ));
            return new AudioCue(clip, 0.55f, 0.95f);
        }

        private static float Envelope(float t, float attack, float release)
        {
            t = Mathf.Max(0f, t);
            if (t < attack)
            {
                return t / Mathf.Max(0.0001f, attack);
            }

            if (t > release)
            {
                var tail = Mathf.Max(0f, 1f - (t - release) / Mathf.Max(0.0001f, release));
                return Mathf.Max(0f, tail);
            }

            return 1f;
        }

        private static float FilteredNoise(float t, float cutoff, float intensity)
        {
            var noise = Mathf.PerlinNoise(t, 0.42f) * 2f - 1f;
            var filtered = Mathf.Lerp(noise, Mathf.Sin(2f * Mathf.PI * cutoff * t), 0.35f);
            return filtered * intensity;
        }

        private static float LayeredSine(float t, params (float freq, float amp)[] layers)
        {
            var value = 0f;
            foreach (var (freq, amp) in layers)
            {
                value += amp * Mathf.Sin(2f * Mathf.PI * freq * t);
            }

            return value;
        }

        private static float BitCrush(float sample, int bits)
        {
            var levels = Mathf.Pow(2f, bits);
            return Mathf.Floor(sample * levels) / levels;
        }

        private static float Square(float value)
        {
            return Mathf.Sign(Mathf.Sin(value));
        }
    }
}
