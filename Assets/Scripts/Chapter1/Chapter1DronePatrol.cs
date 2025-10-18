using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1DronePatrol : MonoBehaviour
    {
        [SerializeField]
        private Transform[] waypoints;
        [SerializeField]
        private float patrolSpeed = 3.2f;
        [SerializeField]
        private float detectionRadius = 14f;
        [SerializeField]
        private float hoverAmplitude = 0.35f;
        [SerializeField]
        private float hoverFrequency = 1.6f;
        [SerializeField]
        private float investigateDuration = 3.5f;

        private int _index;
        private bool _investigating;
        private Vector3 _investigateTarget;
        private float _investigateTimer;
        private Vector3 _baseOffset;
        private List<Renderer> _renderers;
        private Color _emissiveColor = new(0.78f, 0.24f, 0.9f);

        private void Awake()
        {
            _baseOffset = transform.position;
            _renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
            UpdateEmission(false);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerNoiseEvent>(OnNoise);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerNoiseEvent>(OnNoise);
        }

        private void Update()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }

            Vector3 target;
            if (_investigating)
            {
                target = _investigateTarget;
                _investigateTimer -= Time.deltaTime;
                if (_investigateTimer <= 0f)
                {
                    _investigating = false;
                    UpdateEmission(false);
                }
            }
            else
            {
                target = waypoints[_index].position;
            }

            MoveTowards(target, _investigating ? patrolSpeed * 1.4f : patrolSpeed);
            ApplyHover();

            if (!_investigating && Vector3.Distance(transform.position, target) <= 0.4f)
            {
                _index = (_index + 1) % waypoints.Length;
            }
        }

        private void MoveTowards(Vector3 target, float speed)
        {
            var position = transform.position;
            var direction = target - position;
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            var delta = direction.normalized * speed * Time.deltaTime;
            if (delta.sqrMagnitude > direction.sqrMagnitude)
            {
                delta = direction;
            }

            transform.position += delta;
            if (delta.sqrMagnitude > 0.0001f)
            {
                var planar = delta;
                planar.y = 0f;
                if (planar.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(planar.normalized), Time.deltaTime * 4f);
                }
            }
        }

        private void ApplyHover()
        {
            var position = transform.position;
            position.y = _baseOffset.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
            transform.position = new Vector3(position.x, position.y, position.z);
        }

        private void OnNoise(PlayerNoiseEvent evt)
        {
            if (evt.NoiseLevel < 0.25f)
            {
                return;
            }

            if (Vector3.Distance(transform.position, evt.Position) > detectionRadius)
            {
                return;
            }

            _investigating = true;
            _investigateTarget = evt.Position + Vector3.up * 3f;
            _investigateTimer = investigateDuration;
            UpdateEmission(true);
        }

        private void UpdateEmission(bool alert)
        {
            if (_renderers == null)
            {
                return;
            }

            foreach (var renderer in _renderers)
            {
                if (renderer == null || renderer.sharedMaterial == null)
                {
                    continue;
                }

                var emission = alert ? _emissiveColor * 2.2f : _emissiveColor * 0.4f;
                renderer.sharedMaterial.color = alert ? _emissiveColor : renderer.sharedMaterial.color;
                renderer.sharedMaterial.SetColor("_EmissionColor", emission);
                renderer.sharedMaterial.EnableKeyword("_EMISSION");
            }
        }

        public void SetWaypoints(IReadOnlyList<Transform> points)
        {
            waypoints = new Transform[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                waypoints[i] = points[i];
            }
        }

        public void SetEmissiveColor(Color color)
        {
            _emissiveColor = color;
            UpdateEmission(_investigating);
        }
    }
}
