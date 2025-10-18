using UnityEngine;
using UnityEngine.InputSystem;

namespace Chapter1
{
    [RequireComponent(typeof(Collider))]
    public class Chapter1GeneratorInteract : MonoBehaviour
    {
        [SerializeField]
        private Chapter1Generator generator;

        private Collider _collider;
        private bool _playerInside;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = true;
                Debug.Log("Generator ready. Press E to activate.");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = false;
            }
        }

        private void Update()
        {
            if (!_playerInside || Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (generator.TryActivate())
                {
                    Debug.Log("Generator online. Courtyard gate unlocked.");
                }
                else
                {
                    Debug.Log("Generator activation failed. Ensure power is routed.");
                }
            }
        }

        public void SetGenerator(Chapter1Generator target)
        {
            generator = target;
        }
    }
}
