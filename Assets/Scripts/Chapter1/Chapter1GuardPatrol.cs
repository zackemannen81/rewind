using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1GuardPatrol : MonoBehaviour
    {
        [SerializeField]
        private Transform[] waypoints;
        [SerializeField]
        private float patrolSpeed = 1.6f;
        [SerializeField]
        private float investigateSpeed = 2.8f;
        [SerializeField]
        private float detectionRadius = 12f;
        [SerializeField]
        private float investigationDuration = 4f;

        private int _currentIndex;
        private bool _investigating;
        private Vector3 _investigateTarget;
        private float _investigateTimer;
        private CapsuleCollider _collider;
        private Renderer[] _renderers;
        private Color _originalColor;
        private Color _alertColor = new(0.85f, 0.25f, 0.45f, 1f);

        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<CapsuleCollider>();
                _collider.height = 1.8f;
                _collider.radius = 0.4f;
                _collider.center = new Vector3(0f, 0.9f, 0f);
                _collider.isTrigger = true;
            }

            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers.Length > 0)
            {
                _originalColor = _renderers[0].sharedMaterial.color;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerNoiseEvent>(OnNoiseHeard);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerNoiseEvent>(OnNoiseHeard);
        }

        private void Update()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }

            Vector3 target;
            float speed;
            if (_investigating)
            {
                target = _investigateTarget;
                speed = investigateSpeed;
                _investigateTimer -= Time.deltaTime;
                if (_investigateTimer <= 0f)
                {
                    _investigating = false;
                    SetAlertState(false);
                }
            }
            else
            {
                target = waypoints[_currentIndex].position;
                speed = patrolSpeed;
            }

            MoveTowards(target, speed);

            if (!_investigating && Vector3.Distance(transform.position, target) <= 0.3f)
            {
                _currentIndex = (_currentIndex + 1) % waypoints.Length;
            }
        }

        private void MoveTowards(Vector3 target, float speed)
        {
            var current = transform.position;
            target.y = current.y;
            Vector3 direction = target - current;
            float distance = direction.magnitude;
            if (distance > Mathf.Epsilon)
            {
                Vector3 delta = direction.normalized * speed * Time.deltaTime;
                if (delta.magnitude > distance)
                {
                    delta = direction;
                }

                transform.position += delta;
                if (delta.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(delta.normalized), Time.deltaTime * 6f);
                }
            }
        }

        private void OnNoiseHeard(PlayerNoiseEvent evt)
        {
            var distance = Vector3.Distance(transform.position, evt.Position);
            if (distance > detectionRadius)
            {
                return;
            }

            _investigating = true;
            _investigateTarget = evt.Position;
            _investigateTarget.y = transform.position.y;
            _investigateTimer = investigationDuration;
            SetAlertState(true);
        }

        private void SetAlertState(bool alert)
        {
            if (_renderers == null)
            {
                return;
            }

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i].sharedMaterial == null)
                {
                    continue;
                }

                _renderers[i].sharedMaterial.SetColor("_EmissionColor", alert ? _alertColor * 1.5f : _originalColor * 0.1f);
                _renderers[i].sharedMaterial.color = alert ? _alertColor : _originalColor;
                _renderers[i].sharedMaterial.EnableKeyword("_EMISSION");
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

        public void SetColors(Color alertColor, Color defaultColor)
        {
            _alertColor = alertColor;
            _originalColor = defaultColor;
            SetAlertState(_investigating);
        }
    }
}
