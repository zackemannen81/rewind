using System;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1Generator : MonoBehaviour
    {
        public event Action OnGeneratorActivated;

        [SerializeField]
        private Chapter1FuseBox fuseBox;
        [SerializeField]
        private AudioSource generatorAudio;

        public bool IsGeneratorOnline { get; private set; }
        public bool HasCompletedGoldenPath { get; private set; }

        private void Awake()
        {
            if (generatorAudio == null)
            {
                generatorAudio = GetComponentInChildren<AudioSource>();
            }
        }

        public void OnLoopStart()
        {
            IsGeneratorOnline = false;
            if (generatorAudio != null)
            {
                generatorAudio.Stop();
            }
        }

        public bool TryActivate()
        {
            if (IsGeneratorOnline)
            {
                return false;
            }

            if (fuseBox != null && !fuseBox.IsPowerToCourtyard)
            {
                return false;
            }

            IsGeneratorOnline = true;
            HasCompletedGoldenPath = true;
            if (generatorAudio != null)
            {
                generatorAudio.Play();
            }
            OnGeneratorActivated?.Invoke();
            return true;
        }

        public void SetFuseBox(Chapter1FuseBox target)
        {
            fuseBox = target;
        }

        public void SetAudioSource(AudioSource audioSource)
        {
            generatorAudio = audioSource;
        }
    }
}
