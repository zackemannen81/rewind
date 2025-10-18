using UnityEngine;
using UnityEngine.InputSystem;

namespace Chapter1
{
    [RequireComponent(typeof(Collider))]
    public class Chapter1TurnstileInteract : MonoBehaviour
    {
        [SerializeField]
        private Chapter1TransitTurnstile turnstile;

        [SerializeField]
        private Chapter1CourtyardGate gate;

        [SerializeField]
        private AudioClip lockedClip;

        [SerializeField]
        private AudioClip openClip;

        private AudioSource _audioSource;
        private Collider _collider;
        private bool _playerInside;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            _playerInside = true;
            Debug.Log("Turnstile humming. Wait for the window (press E when lights flash).");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            _playerInside = false;
        }

        private void Update()
        {
            if (!_playerInside || Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (turnstile.IsWindowOpen)
                {
                    Debug.Log("Transit window breached. Loop objective complete!");
                    PlayClip(openClip);
                    // Future: trigger chapter transition here.
                }
                else
                {
                    Debug.Log("Turnstile locked. Wait for the window.");
                    PlayClip(lockedClip);
                }
            }
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public void Initialize(Chapter1TransitTurnstile turnstileRef, Chapter1CourtyardGate gateRef, AudioClip locked, AudioClip opened)
        {
            turnstile = turnstileRef;
            gate = gateRef;
            lockedClip = locked;
            openClip = opened;
        }
    }
}
