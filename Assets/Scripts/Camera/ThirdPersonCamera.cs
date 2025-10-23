using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraSystem
{
    /// <summary>
    /// Orbit-style third-person camera that follows a target with smooth damping,
    /// supports mouse/gamepad look and zoom, and keeps the player aligned with the camera yaw.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Rig References")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform rotationTarget;
        [SerializeField] private Transform pivot;
        [SerializeField] private Transform cameraTransform;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 followOffset = new(0f, 1.6f, 0f);
        [SerializeField, Min(0f)] private float followLerp = 12f;

        [Header("Orbit Settings")]
        [SerializeField] private Vector2 lookSensitivity = new(180f, 140f);
        [SerializeField] private Vector2 pitchLimits = new(-65f, 70f);
        [SerializeField, Min(0f)] private float rotationLerp = 16f;
        [SerializeField] private bool lockCursor = true;

        [Header("Zoom Settings")]
        [SerializeField] private float distance = 4.5f;
        [SerializeField] private Vector2 distanceLimits = new(2.2f, 6.5f);
        [SerializeField] private float zoomSensitivity = 2f;
        [SerializeField] private LayerMask collisionMask = -1;
        [SerializeField] private float collisionRadius = 0.2f;

        private InputAction _lookAction;
        private InputAction _zoomTriggerAction;
        private InputAction _zoomScrollAction;
        private float _yaw;
        private float _pitch;
        private float _desiredDistance;
        private Vector3 _currentPivotPosition;
        private bool _cursorLocked;

        private void Awake()
        {
            if (pivot == null)
            {
                pivot = transform;
            }

            if (cameraTransform == null && TryGetComponent(out Camera ownCamera))
            {
                cameraTransform = ownCamera.transform;
            }

            if (cameraTransform == null)
            {
                cameraTransform = GetComponentInChildren<Camera>()?.transform;
            }

            if (cameraTransform == null)
            {
                throw new InvalidOperationException("ThirdPersonCamera requires a Camera transform reference.");
            }

            var forward = pivot.forward;
            _yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            _pitch = pivot.eulerAngles.x;
            _desiredDistance = Mathf.Clamp(distance, distanceLimits.x, distanceLimits.y);
            distance = _desiredDistance;
            _currentPivotPosition = pivot.position;

            SetupActions();
        }

        private void SetupActions()
        {
            _lookAction = new InputAction("Look", InputActionType.Value);
            _lookAction.AddBinding("<Mouse>/delta");
            _lookAction.AddBinding("<Gamepad>/rightStick");

            _zoomTriggerAction = new InputAction("ZoomTriggers", InputActionType.Value);
            _zoomTriggerAction.AddCompositeBinding("1DAxis")
                .With("positive", "<Gamepad>/rightTrigger")
                .With("negative", "<Gamepad>/leftTrigger");

            _zoomScrollAction = new InputAction("ZoomScroll", InputActionType.Value);
            _zoomScrollAction.AddBinding("<Mouse>/scroll/y");
        }

        private void OnEnable()
        {
            _lookAction?.Enable();
            _zoomTriggerAction?.Enable();
            _zoomScrollAction?.Enable();

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _cursorLocked = true;
            }
        }

        private void OnDisable()
        {
            _lookAction?.Disable();
            _zoomTriggerAction?.Disable();
            _zoomScrollAction?.Disable();

            if (lockCursor && _cursorLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _cursorLocked = false;
            }
        }

        private void OnDestroy()
        {
            _lookAction?.Dispose();
            _zoomTriggerAction?.Dispose();
            _zoomScrollAction?.Dispose();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!lockCursor)
            {
                return;
            }

            if (hasFocus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _cursorLocked = true;
            }
            else if (_cursorLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _cursorLocked = false;
            }
        }

        private void LateUpdate()
        {
            if (followTarget == null || cameraTransform == null)
            {
                return;
            }

            var dt = Time.deltaTime;
            var lookInput = _lookAction.ReadValue<Vector2>();
            _yaw += lookInput.x * lookSensitivity.x * dt;
            _pitch -= lookInput.y * lookSensitivity.y * dt;
            _pitch = Mathf.Clamp(_pitch, pitchLimits.x, pitchLimits.y);

            var triggerZoom = _zoomTriggerAction.ReadValue<float>();
            if (!Mathf.Approximately(triggerZoom, 0f))
            {
                _desiredDistance = Mathf.Clamp(_desiredDistance - triggerZoom * zoomSensitivity * dt, distanceLimits.x, distanceLimits.y);
            }

            var scrollZoom = _zoomScrollAction.ReadValue<float>();
            if (!Mathf.Approximately(scrollZoom, 0f))
            {
                const float scrollScale = 0.02f;
                _desiredDistance = Mathf.Clamp(_desiredDistance - scrollZoom * zoomSensitivity * scrollScale, distanceLimits.x, distanceLimits.y);
            }

            distance = Mathf.Lerp(distance, _desiredDistance, 1f - Mathf.Exp(-followLerp * dt));

            var desiredPosition = followTarget.position + followOffset;
            _currentPivotPosition = Vector3.Lerp(_currentPivotPosition, desiredPosition, 1f - Mathf.Exp(-followLerp * dt));
            pivot.position = _currentPivotPosition;

            var targetRotation = Quaternion.Euler(_pitch, _yaw, 0f);
            pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, 1f - Mathf.Exp(-rotationLerp * dt));

            var desiredCameraLocalPosition = new Vector3(0f, 0f, -distance);
            cameraTransform.localPosition = desiredCameraLocalPosition;
            cameraTransform.localRotation = Quaternion.identity;

            ResolveCollisions();
            AlignTargetRotation(dt);
        }

        private void ResolveCollisions()
        {
            if (collisionRadius <= 0f)
            {
                return;
            }

            var desiredWorldPosition = cameraTransform.position;
            var origin = pivot.position;
            var direction = (desiredWorldPosition - origin).normalized;
            var distanceToCamera = Vector3.Distance(origin, desiredWorldPosition);

            if (Physics.SphereCast(origin, collisionRadius, direction, out var hit, distanceToCamera, collisionMask, QueryTriggerInteraction.Ignore))
            {
                var adjustedPosition = origin + direction * Mathf.Max(0.05f, hit.distance - 0.05f);
                cameraTransform.position = adjustedPosition;
            }
        }

        private void AlignTargetRotation(float deltaTime)
        {
            if (rotationTarget == null)
            {
                return;
            }

            var yawOnly = Quaternion.Euler(0f, _yaw, 0f);
            rotationTarget.rotation = Quaternion.Slerp(rotationTarget.rotation, yawOnly, 1f - Mathf.Exp(-rotationLerp * deltaTime));
        }

        public void SetTargets(Transform follow, Transform rotation)
        {
            followTarget = follow;
            rotationTarget = rotation;
            _currentPivotPosition = followTarget != null ? followTarget.position + followOffset : Vector3.zero;
            _desiredDistance = Mathf.Clamp(distance, distanceLimits.x, distanceLimits.y);
        }
    }
}
