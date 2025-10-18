using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;

namespace Chapter1
{
    [RequireComponent(typeof(EchoRecorder))]
    public class Chapter1EchoVisualizer : MonoBehaviour
    {
        [SerializeField]
        private float ghostScale = 0.4f;
        [SerializeField]
        private float sampleSpacingSeconds = 0.8f;
        [SerializeField]
        private int maxGhosts = 20;
        [SerializeField]
        private Color ghostTint = new(0.82f, 0.26f, 0.9f, 0.45f);

        private EchoRecorder _recorder;
        private readonly List<GameObject> _ghosts = new();
        private Material _ghostMaterial;

        private void Awake()
        {
            _recorder = GetComponent<EchoRecorder>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<LoopStartEvent>(OnLoopStart);
            EventBus.Subscribe<LoopEndEvent>(OnLoopEnd);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LoopStartEvent>(OnLoopStart);
            EventBus.Unsubscribe<LoopEndEvent>(OnLoopEnd);
            ClearGhosts();
        }

        private void OnLoopStart(LoopStartEvent _)
        {
            ClearGhosts();
        }

        private void OnLoopEnd(LoopEndEvent _)
        {
            BuildGhosts();
        }

        private void BuildGhosts()
        {
            ClearGhosts();

            if (_recorder.RecordedActions == null || _recorder.RecordedActions.Count == 0)
            {
                return;
            }

            float nextSample = _recorder.RecordedActions[0].Timestamp;
            int created = 0;

            for (int i = 0; i < _recorder.RecordedActions.Count && created < maxGhosts; i++)
            {
                var action = _recorder.RecordedActions[i];
                if (action.Timestamp < nextSample)
                {
                    continue;
                }

                CreateGhost(action.Position);
                created++;
                nextSample = action.Timestamp + sampleSpacingSeconds;
            }
        }

        private void CreateGhost(Vector3 worldPosition)
        {
            var ghost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ghost.name = $"EchoGhost_{_ghosts.Count:00}";
            ghost.transform.SetParent(transform, false);
            ghost.transform.position = worldPosition + Vector3.up * 0.3f;
            ghost.transform.localScale = Vector3.one * ghostScale;

            var renderer = ghost.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = GetGhostMaterial();

            var collider = ghost.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            var pulse = ghost.AddComponent<EchoPulse>();
            pulse.Configure(ghostTint);

            _ghosts.Add(ghost);
        }

        private Material GetGhostMaterial()
        {
            if (_ghostMaterial == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                _ghostMaterial = new Material(shader)
                {
                    color = ghostTint
                };
                _ghostMaterial.SetColor("_EmissionColor", ghostTint * 1.8f);
                _ghostMaterial.EnableKeyword("_EMISSION");
            }

            return _ghostMaterial;
        }

        private void ClearGhosts()
        {
            for (int i = 0; i < _ghosts.Count; i++)
            {
                if (_ghosts[i] != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        DestroyImmediate(_ghosts[i]);
                        continue;
                    }
#endif
                    Destroy(_ghosts[i]);
                }
            }

            _ghosts.Clear();
        }

        public void SetGhostTint(Color tint)
        {
            ghostTint = tint;
            if (_ghostMaterial != null)
            {
                _ghostMaterial.color = ghostTint;
                _ghostMaterial.SetColor("_EmissionColor", ghostTint * 1.8f);
            }
        }

        private class EchoPulse : MonoBehaviour
        {
            private const float PulseSpeed = 2.5f;
            private const float ScaleAmplitude = 0.15f;
            private Vector3 _baseScale;
            private Renderer _renderer;
            private Color _baseColor;

            public void Configure(Color tint)
            {
                _baseScale = transform.localScale;
                _renderer = GetComponent<Renderer>();
                if (_renderer != null && _renderer.material != null)
                {
                    _baseColor = tint;
                }
            }

            private void Update()
            {
                if (_renderer != null)
                {
                    float emission = 0.5f + Mathf.Sin(Time.time * PulseSpeed) * 0.5f;
                    _renderer.material.SetColor("_EmissionColor", _baseColor * (1.2f + emission));
                }

                transform.localScale = _baseScale * (1f + Mathf.Sin(Time.time * PulseSpeed) * ScaleAmplitude);
            }
        }
    }
}
