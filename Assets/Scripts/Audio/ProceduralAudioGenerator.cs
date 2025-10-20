using System;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Utility helpers for generating simple procedural audio clips at runtime.
    /// </summary>
    public static class ProceduralAudioGenerator
    {
        private const int DefaultSampleRate = 44100;

        public static AudioClip CreateClip(string name, float durationSeconds, Func<float, float> sampleProvider, int sampleRate = DefaultSampleRate)
        {
            if (sampleProvider == null)
            {
                throw new ArgumentNullException(nameof(sampleProvider));
            }

            durationSeconds = Mathf.Max(0.01f, durationSeconds);
            sampleRate = Mathf.Max(22050, sampleRate);

            var sampleCount = Mathf.CeilToInt(durationSeconds * sampleRate);
            var data = new float[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                var time = i / (float)sampleRate;
                data[i] = Mathf.Clamp(sampleProvider.Invoke(time), -1f, 1f);
            }

            var clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip CreateLoop(string name, float durationSeconds, Func<float, float> sampleProvider, int sampleRate = DefaultSampleRate)
        {
            var clip = CreateClip(name, durationSeconds, sampleProvider, sampleRate);
            clip.name = name;
            clip.SetData(GetLoopedSamples(sampleProvider, durationSeconds, sampleRate), 0);
            return clip;
        }

        private static float[] GetLoopedSamples(Func<float, float> sampleProvider, float durationSeconds, int sampleRate)
        {
            var sampleCount = Mathf.CeilToInt(durationSeconds * sampleRate);
            var data = new float[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                var time = i / (float)sampleRate;
                data[i] = Mathf.Clamp(sampleProvider.Invoke(time), -1f, 1f);
            }

            var lastSample = data[^1];
            var firstSample = data[0];
            var fadeSamples = Mathf.Min(sampleRate / 100, sampleCount / 4);

            for (var i = 0; i < fadeSamples; i++)
            {
                var t = i / (float)fadeSamples;
                data[sampleCount - fadeSamples + i] = Mathf.Lerp(lastSample, firstSample, t);
            }

            return data;
        }
    }
}
