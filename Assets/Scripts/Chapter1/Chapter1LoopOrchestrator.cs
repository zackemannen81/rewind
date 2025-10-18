using Core;
using Events;
using Managers;
using UnityEngine;

namespace Chapter1
{
    /// <summary>
    /// Coordinates Chapter 1 state transitions (radio clarity, anchor availability, knowledge unlocks).
    /// </summary>
    public class Chapter1LoopOrchestrator : MonoBehaviour
    {
        [Header("References")]
        private Chapter1RadioController _radioController;
        private Chapter1FuseBox _fuseBox;
        private Chapter1Generator _generator;
        private Chapter1CourtyardGate _gate;
        private Chapter1TransitTurnstile _transitTurnstile;

        private int _loopIteration;

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

        private void OnLoopStart(LoopStartEvent _)
        {
            _loopIteration++;

            if (_radioController != null)
            {
                _radioController.OnLoopStart(_loopIteration);
            }

            if (_fuseBox != null)
            {
                _fuseBox.OnLoopStart();
            }

            if (_generator != null)
            {
                _generator.OnLoopStart();
            }

            if (_gate != null)
            {
                _gate.OnLoopStart();
            }

            if (_transitTurnstile != null)
            {
                _transitTurnstile.OnLoopStart();
            }
        }

        private void OnLoopEnd(LoopEndEvent evt)
        {
            if (evt.Reason == "player_death" || evt.Reason == "time_expired")
            {
                EvaluateAnchorProgress();
            }
        }

        private void EvaluateAnchorProgress()
        {
            if (_generator != null && _generator.HasCompletedGoldenPath && _gate != null)
            {
                AnchorManager.Instance?.ActivateAnchor(Chapter1Constants.AnchorCourtyardGate);
            }
        }

        public void Configure(
            Chapter1RadioController radioController,
            Chapter1FuseBox fuseBox,
            Chapter1Generator generator,
            Chapter1CourtyardGate courtyardGate,
            Chapter1TransitTurnstile transitTurnstile)
        {
            _radioController = radioController;
            _fuseBox = fuseBox;
            _generator = generator;
            _gate = courtyardGate;
            _transitTurnstile = transitTurnstile;
        }
    }
}
