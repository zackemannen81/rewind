using System;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1TransitTurnstile : MonoBehaviour
    {
        public event Action<bool> OnWindowStateChanged;

        [SerializeField]
        private float cycleDuration = 30f;
        [SerializeField]
        private float windowDuration = 3f;

        private bool _isWindowOpen;
        private float _loopStartTime;

        public bool IsWindowOpen => _isWindowOpen;

        public void OnLoopStart()
        {
            _loopStartTime = Time.time;
            EvaluateWindowState(forceNotify: true);
        }

        private void Update()
        {
            EvaluateWindowState();
        }

        private void EvaluateWindowState(bool forceNotify = false)
        {
            var elapsed = Time.time - _loopStartTime;
            var cyclePosition = Mathf.Repeat(elapsed, cycleDuration);
            var newState = cyclePosition <= windowDuration;

            if (forceNotify || newState != _isWindowOpen)
            {
                _isWindowOpen = newState;
                OnWindowStateChanged?.Invoke(_isWindowOpen);
            }
        }
    }
}
