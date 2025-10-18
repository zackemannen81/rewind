using UnityEngine;
using UnityEngine.InputSystem;

namespace Chapter1
{
    [RequireComponent(typeof(Collider))]
    public class Chapter1RadioInteract : MonoBehaviour
    {
        [SerializeField]
        private Chapter1RadioController radioController;

        [SerializeField]
        private AudioClip garbleClip;

        [SerializeField]
        private AudioClip clueClip;

        private Collider _collider;
        private bool _playerInside;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            _playerInside = true;
            radioController.SetAudioClips(garbleClip, clueClip);
            Debug.Log("Radio humming. Press E to tune channel.");
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
            radioController.ManualTriggerClue();
            Debug.Log("Radio signal tuned. Remember the numbers.");
        }
        }

        public void Initialize(Chapter1RadioController controller, AudioClip garble, AudioClip clue)
        {
            radioController = controller;
            garbleClip = garble;
            clueClip = clue;
        }
    }
}
