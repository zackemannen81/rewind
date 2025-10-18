using UnityEngine;
using UnityEngine.InputSystem;

namespace Chapter1
{
    [RequireComponent(typeof(Collider))]
    public class Chapter1FuseInteract : MonoBehaviour
    {
        [SerializeField]
        private Chapter1FuseBox fuseBox;

        [SerializeField]
        private bool routeToCourtyard = true;

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
                Debug.Log("Fuse box ready. Press E to toggle power.");
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
                if (fuseBox.TryRoutePower(routeToCourtyard))
                {
                    fuseBox.ConsumeFuse();
                    Debug.Log("Fuse routed. Courtyard has power.");
                }
                else
                {
                    Debug.Log("Fuse routing failed (already used or missing).");
                }
            }
        }

        public void SetFuseBox(Chapter1FuseBox target)
        {
            fuseBox = target;
        }
    }
}
