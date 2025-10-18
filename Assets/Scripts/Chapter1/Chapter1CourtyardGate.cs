using Managers;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1CourtyardGate : MonoBehaviour
    {
        [SerializeField]
        private Animator gateAnimator;
        [SerializeField]
        private AudioSource gateAudio;

        private static readonly int OpenHash = Animator.StringToHash("Open");

        private Collider _gateCollider;

        public bool IsOpen { get; private set; }

        public void OnLoopStart()
        {
            if (AnchorManager.Instance != null && AnchorManager.Instance.IsAnchorActive(Chapter1Constants.AnchorCourtyardGate))
            {
                ForceOpen();
            }
            else
            {
                Close();
            }
        }

        public void Open()
        {
            IsOpen = true;
            if (gateAnimator != null)
            {
                gateAnimator.SetBool(OpenHash, true);
            }

            if (_gateCollider != null)
            {
                _gateCollider.enabled = false;
            }

            if (gateAudio != null)
            {
                gateAudio.Play();
            }
        }

        public void Close()
        {
            IsOpen = false;
            if (gateAnimator != null)
            {
                gateAnimator.SetBool(OpenHash, false);
            }

            if (_gateCollider != null)
            {
                _gateCollider.enabled = true;
            }

            if (gateAudio != null)
            {
                gateAudio.Stop();
            }
        }

        public void ForceOpen()
        {
            Open();
        }

        public void AssignCollider(Collider collider)
        {
            _gateCollider = collider;
        }

        public void SetAudioSource(AudioSource audioSource)
        {
            gateAudio = audioSource;
        }
    }
}
