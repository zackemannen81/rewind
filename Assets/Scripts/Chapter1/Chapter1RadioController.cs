using Managers;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1RadioController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource radioSource;
        [SerializeField]
        private AudioClip garbleClip;
        [SerializeField]
        private AudioClip clueClip;

        private int _clarityLevel;
        private bool _codeRecorded;

        public void OnLoopStart(int loopIteration)
        {
            _clarityLevel = Mathf.Clamp(loopIteration, 1, 4);
            PlayCurrentClip();

            if (_clarityLevel >= 3 && !_codeRecorded)
            {
                KnowledgeManager.Instance?.AddKnowledge(Chapter1Constants.KnowledgeRadioCode, "7312");
                _codeRecorded = true;
            }
        }

        public void ManualTriggerClue()
        {
            PlayCurrentClip(forceClue: true);
        }

        public void SetAudioSource(AudioSource source)
        {
            radioSource = source;
        }

        private void PlayCurrentClip(bool forceClue = false)
        {
            if (radioSource == null)
            {
                return;
            }

            var clip = _clarityLevel >= 3 || forceClue ? clueClip : garbleClip;
            if (clip == null)
            {
                return;
            }

            radioSource.clip = clip;
            if (!radioSource.isPlaying)
            {
                radioSource.Play();
            }
        }
    }
}
