using Managers;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1CourtyardGate : MonoBehaviour
    {
        [SerializeField]
        private Animator gateAnimator;

        private static readonly int OpenHash = Animator.StringToHash("Open");

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
        }

        public void Close()
        {
            IsOpen = false;
            if (gateAnimator != null)
            {
                gateAnimator.SetBool(OpenHash, false);
            }
        }

        public void ForceOpen()
        {
            Open();
        }
    }
}
