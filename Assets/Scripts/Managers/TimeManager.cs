
using Core;
using Events;
using UnityEngine;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField]
        private float loopDurationSeconds = 420f; // 7 minutes

        private float _currentTime;
        private bool _isLoopActive;
        private int _lastMinutePublished;

        private void Start()
        {
            // Automatically start the first loop for testing purposes
            StartLoop();
        }

        private void Update()
        {
            if (!_isLoopActive) return;

            _currentTime -= Time.deltaTime;

            // Publish minute passed event
            var minutesRemaining = Mathf.FloorToInt(_currentTime / 60f);
            if (minutesRemaining < _lastMinutePublished)
            {
                _lastMinutePublished = minutesRemaining;
                EventBus.Publish(new MinutePassedEvent { MinutesRemaining = minutesRemaining });
            }

            if (_currentTime <= 0)
            {
                EndLoop("time_expired");
            }
        }

        public void StartLoop()
        {
            _currentTime = loopDurationSeconds;
            _isLoopActive = true;
            _lastMinutePublished = Mathf.FloorToInt(_currentTime / 60f);
            EventBus.Publish(new LoopStartEvent());
            Debug.Log("New loop started.");
        }

        public void EndLoop(string reason)
        {
            if (!_isLoopActive) return;

            _isLoopActive = false;
            EventBus.Publish(new LoopEndEvent { Reason = reason });
            Debug.Log($"Loop ended. Reason: {reason}");

            // In a real scenario, we would transition to a loading screen or similar
            // before starting the next loop.
            // For now, we'll just reset immediately.
            ResetLoop();
        }

        public void ResetLoop()
        {
            // This method would be responsible for resetting the world state.
            // For now, it just starts a new loop.
            StartLoop();
        }
    }
}
