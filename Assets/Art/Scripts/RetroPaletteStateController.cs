using System;
using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;

namespace Art
{
    /// <summary>
    /// Syncs project materials with the configured RetroPalette and toggles shader keywords as loop states change.
    /// </summary>
    public class RetroPaletteStateController : MonoBehaviour
    {
        [Serializable]
        private struct MaterialBinding
        {
            public Material material;
            public PaletteSwatch baseSwatch;
            public PaletteSwatch accentSwatch;
            [Range(0f, 5f)] public float accentIntensity;
            [Range(0f, 1f)] public float glitchStrength;
            [Range(0f, 1f)] public float smoothness;
            [Range(0f, 1f)] public float metallic;
        }

        [SerializeField]
        private RetroPalette palette;

        [SerializeField]
        private List<MaterialBinding> materialBindings = new();

        private LoopState _currentLoopState = LoopState.Alert;
        private bool _isInitialised;

        private void OnEnable()
        {
            ApplyPaletteToMaterials();
            SetLoopState(LoopState.Normal);

            if (_isInitialised) return;

            EventBus.Subscribe<LoopStartEvent>(_ => SetLoopState(LoopState.Normal));
            EventBus.Subscribe<MinutePassedEvent>(OnMinutePassed);
            EventBus.Subscribe<LoopEndEvent>(_ => SetLoopState(LoopState.LoopEnd));
            _isInitialised = true;
        }

        private void OnMinutePassed(MinutePassedEvent minuteEvent)
        {
            // Enter alert mode for the final minute.
            if (minuteEvent.MinutesRemaining <= 1 && _currentLoopState == LoopState.Normal)
            {
                SetLoopState(LoopState.Alert);
            }
        }

        private void ApplyPaletteToMaterials()
        {
            if (palette == null)
            {
                Debug.LogWarning("RetroPaletteStateController has no palette assigned.", this);
                return;
            }

            foreach (var binding in materialBindings)
            {
                if (binding.material == null) continue;

                binding.material.SetColor("_BaseColor", palette.GetColor(binding.baseSwatch));
                binding.material.SetColor("_AccentColor", palette.GetColor(binding.accentSwatch));
                binding.material.SetFloat("_AccentIntensity", binding.accentIntensity);
                binding.material.SetFloat("_GlitchStrength", binding.glitchStrength);
                binding.material.SetFloat("_Smoothness", binding.smoothness);
                binding.material.SetFloat("_Metallic", binding.metallic);
            }
        }

        private void SetLoopState(LoopState state)
        {
            if (_currentLoopState == state) return;

            _currentLoopState = state;

            switch (state)
            {
                case LoopState.Normal:
                    Shader.DisableKeyword("RETRO_ALERT");
                    Shader.DisableKeyword("RETRO_LOOPEND");
                    break;
                case LoopState.Alert:
                    Shader.EnableKeyword("RETRO_ALERT");
                    Shader.DisableKeyword("RETRO_LOOPEND");
                    break;
                case LoopState.LoopEnd:
                    Shader.DisableKeyword("RETRO_ALERT");
                    Shader.EnableKeyword("RETRO_LOOPEND");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private enum LoopState
        {
            Normal,
            Alert,
            LoopEnd,
        }
    }
}
