using UnityEngine;

namespace Chapter1
{
    [RequireComponent(typeof(AudioSource))]
    public class Chapter1AmbientController : MonoBehaviour
    {
        [SerializeField]
        private AudioClip ambientClip;

        [SerializeField]
        private float fadeInSeconds = 3f;

        private AudioSource _audioSource;
        private float _targetVolume;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
            _targetVolume = _audioSource.volume <= 0f ? 0.65f : _audioSource.volume;
            _audioSource.volume = 0f;
        }

        private void Start()
        {
            if (ambientClip != null)
            {
                _audioSource.clip = ambientClip;
                _audioSource.Play();
            }
        }

        private void Update()
        {
            if (_audioSource.clip == null)
            {
                return;
            }

            if (_audioSource.volume < _targetVolume)
            {
                _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, _targetVolume, Time.deltaTime * (_targetVolume / Mathf.Max(0.001f, fadeInSeconds)));
            }
        }

        public void SetClip(AudioClip clip)
        {
            ambientClip = clip;

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource != null)
                {
                    _audioSource.loop = true;
                    _audioSource.playOnAwake = false;
                }
                else
                {
                    return;
                }
            }

            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            if (clip != null)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        }
    }
}
