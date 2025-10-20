using UnityEngine;

namespace Audio
{
    public enum AudioCueChannel
    {
        Effects,
        UI,
        Voice
    }

    public class AudioCueTrigger : MonoBehaviour
    {
        [SerializeField]
        private AudioCueId cueId = AudioCueId.InteractionClick;
        [SerializeField]
        private AudioCueChannel channel = AudioCueChannel.Effects;
        [SerializeField, Range(0f, 2f)]
        private float volumeScale = 1f;

        public void Play()
        {
            if (SoundscapeManager.Instance == null)
            {
                return;
            }

            switch (channel)
            {
                case AudioCueChannel.Effects:
                    SoundscapeManager.Instance.PlayEffect(cueId, volumeScale);
                    break;
                case AudioCueChannel.UI:
                    SoundscapeManager.Instance.PlayUiCue(cueId, volumeScale);
                    break;
                case AudioCueChannel.Voice:
                    SoundscapeManager.Instance.PlayVoiceCue(cueId, volumeScale);
                    break;
            }
        }
    }
}
