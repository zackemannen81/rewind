
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public struct PlayerAction
    {
        public float Timestamp;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public class EchoRecorder : MonoBehaviour
    {
        public List<PlayerAction> RecordedActions { get; private set; } = new();

        [SerializeField]
        private float recordInterval = 0.1f;

        private float _timer;
        private bool _isRecording;

        private void OnEnable()
        {
            EventBus.Subscribe<LoopStartEvent>(OnLoopStart);
            EventBus.Subscribe<LoopEndEvent>(OnLoopEnd);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LoopStartEvent>(OnLoopStart);
            EventBus.Unsubscribe<LoopEndEvent>(OnLoopEnd);
        }

        private void Update()
        {
            if (!_isRecording) return;

            _timer += Time.deltaTime;
            if (_timer >= recordInterval)
            {
                _timer = 0f;
                RecordAction();
            }
        }

        private void OnLoopStart(LoopStartEvent e)
        {
            StartRecording();
        }

        private void OnLoopEnd(LoopEndEvent e)
        {
            StopRecording();
            // Here you would typically save the recording
        }

        public void StartRecording()
        {
            RecordedActions.Clear();
            _isRecording = true;
        }

        public void StopRecording()
        {
            _isRecording = false;
        }

        private void RecordAction()
        {
            RecordedActions.Add(new PlayerAction
            {
                Timestamp = Time.time,
                Position = transform.position,
                Rotation = transform.rotation
            });
        }
    }

    public class EchoPlayback : MonoBehaviour
    {
        private List<PlayerAction> _actionsToPlay;
        private int _currentActionIndex;
        private bool _isPlaying;

        public void StartPlayback(List<PlayerAction> actions)
        {
            _actionsToPlay = actions;
            _currentActionIndex = 0;
            _isPlaying = true;
            transform.position = _actionsToPlay[0].Position;
            transform.rotation = _actionsToPlay[0].Rotation;
        }

        private void Update()
        {
            if (!_isPlaying || _actionsToPlay == null || _actionsToPlay.Count == 0) return;

            if (_currentActionIndex >= _actionsToPlay.Count - 1) 
            {
                _isPlaying = false;
                return;
            }

            var nextAction = _actionsToPlay[_currentActionIndex + 1];
            var timeSinceStart = Time.time - _actionsToPlay[0].Timestamp;

            if (timeSinceStart >= nextAction.Timestamp - _actionsToPlay[0].Timestamp)
            {
                _currentActionIndex++;
            }

            var currentAction = _actionsToPlay[_currentActionIndex];
            var nextActionForLerp = _actionsToPlay[_currentActionIndex + 1];
            
            float actionDuration = nextActionForLerp.Timestamp - currentAction.Timestamp;
            float timeIntoAction = timeSinceStart - (currentAction.Timestamp - _actionsToPlay[0].Timestamp);
            float lerpFactor = timeIntoAction / actionDuration;

            transform.position = Vector3.Lerp(currentAction.Position, nextActionForLerp.Position, lerpFactor);
            transform.rotation = Quaternion.Slerp(currentAction.Rotation, nextActionForLerp.Rotation, lerpFactor);
        }
    }
}
