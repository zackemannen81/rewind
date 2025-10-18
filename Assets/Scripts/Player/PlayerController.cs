using System.Collections;
using Core;
using Events;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float walkSpeed = 5f;
        [SerializeField]
        private float runSpeed = 9f;
        [SerializeField]
        private float crouchSpeed = 2.5f;
        [SerializeField]
        private float sneakSpeed = 1.5f;
        [SerializeField]
        private float jumpSpeed = 5f;
        [SerializeField]
        private float gravity = -9.81f;

        [Header("Crouch")]
        [SerializeField]
        private float standingHeight = 1.8f;
        [SerializeField]
        private float crouchingHeight = 1.2f;
        [SerializeField]
        private float headClearanceBuffer = 0.15f;

        [Header("Traversal")]
        [SerializeField]
        private LayerMask collisionMask = ~0;
        [SerializeField]
        private LayerMask traversalMask = ~0;
        [SerializeField]
        private LayerMask groundMask = ~0;
        [SerializeField]
        private float vaultCheckDistance = 1.1f;
        [SerializeField]
        private float maxVaultHeight = 1.2f;
        [SerializeField]
        private float vaultDuration = 0.35f;
        [SerializeField]
        private float climbCheckDistance = 0.9f;
        [SerializeField]
        private float maxClimbHeight = 2.8f;
        [SerializeField]
        private float climbDurationPerMeter = 0.55f;
        [SerializeField]
        private float ledgeClearance = 0.5f;

        [Header("Lean")]
        [SerializeField]
        private Transform leanPivot;
        [SerializeField]
        private float leanAngle = 15f;
        [SerializeField]
        private float leanOffset = 0.25f;
        [SerializeField]
        private float leanSmoothing = 12f;

        [Header("Noise Output")]
        [SerializeField]
        private float idleNoise = 0.05f;
        [SerializeField]
        private float walkNoise = 0.35f;
        [SerializeField]
        private float runNoise = 0.8f;
        [SerializeField]
        private float crouchNoise = 0.18f;
        [SerializeField]
        private float sneakNoise = 0.08f;
        [SerializeField]
        private float noiseBroadcastInterval = 0.25f;

        private CharacterController _characterController;
        private PlayerInput _playerInput;

        private Vector3 _velocity;
        private float _currentSpeed;
        private bool _isRunning;
        private bool _isCrouching;
        private bool _isVaulting;
        private bool _isClimbing;

        private Coroutine _traversalRoutine;
        private float _noiseTimer;
        private Vector3 _defaultControllerCenter;
        private Vector3 _leanDefaultPosition;
        private Quaternion _leanDefaultRotation;
        private float _currentLeanAngle;
        private Vector3 _currentLeanOffset;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _defaultControllerCenter = _characterController.center;

            if (leanPivot != null)
            {
                _leanDefaultPosition = leanPivot.localPosition;
                _leanDefaultRotation = leanPivot.localRotation;
                _currentLeanOffset = _leanDefaultPosition;
            }
        }

        private void Start()
        {
            standingHeight = Mathf.Max(standingHeight, _characterController.height);
            crouchingHeight = Mathf.Clamp(crouchingHeight, 0.6f, standingHeight - 0.1f);
            SetControllerHeight(standingHeight);
        }

        private void Update()
        {
            if (_isVaulting || _isClimbing)
            {
                UpdateLean();
                BroadcastNoise();
                return;
            }

            HandleCrouchToggle();
            HandleMovement();
            HandleJumpOrVault();
            HandleClimbRequest();
            ApplyGravity();
            UpdateLean();
            BroadcastNoise();
        }

        private void HandleMovement()
        {
            var input = _playerInput.MoveInput;
            var moveDirection = transform.right * input.x + transform.forward * input.y;
            var hasInput = moveDirection.sqrMagnitude > 0.0001f;
            if (hasInput)
            {
                moveDirection.Normalize();
            }

            var speed = DetermineSpeed(hasInput);
            var horizontalVelocity = moveDirection * speed;
            _characterController.Move(horizontalVelocity * Time.deltaTime);
            _currentSpeed = horizontalVelocity.magnitude;
        }

        private float DetermineSpeed(bool hasInput)
        {
            _isRunning = false;

            if (!hasInput)
            {
                return 0f;
            }

            if (_isCrouching)
            {
                return crouchSpeed;
            }

            if (_playerInput.SneakHeld)
            {
                return sneakSpeed;
            }

            if (_playerInput.RunHeld)
            {
                _isRunning = true;
                return runSpeed;
            }

            return walkSpeed;
        }

        private void HandleJumpOrVault()
        {
            if (!_playerInput.ConsumeJumpPressed())
            {
                return;
            }

            if (TryStartVault())
            {
                return;
            }

            if (_characterController.isGrounded)
            {
                if (_isCrouching && !TryStandUp())
                {
                    return;
                }

                _velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            }
        }

        private void HandleClimbRequest()
        {
            if (!_playerInput.ConsumeClimbRequest())
            {
                return;
            }

            TryStartClimb();
        }

        private void HandleCrouchToggle()
        {
            if (_playerInput.ConsumeCrouchToggle())
            {
                if (_isCrouching)
                {
                    TryStandUp();
                }
                else
                {
                    EnterCrouch();
                }
            }

            if (_isCrouching && _playerInput.RunHeld)
            {
                TryStandUp();
            }
        }

        private void EnterCrouch()
        {
            _isCrouching = true;
            SetControllerHeight(crouchingHeight);
        }

        private bool TryStandUp()
        {
            if (!HasClearanceToStand())
            {
                return false;
            }

            _isCrouching = false;
            SetControllerHeight(standingHeight);
            return true;
        }

        private bool HasClearanceToStand()
        {
            var radius = Mathf.Max(0.05f, _characterController.radius - 0.02f);
            var bounds = _characterController.bounds;
            var castOrigin = bounds.center;
            var castDistance = standingHeight - _characterController.height;
            if (castDistance <= 0f)
            {
                return true;
            }

            return !Physics.SphereCast(castOrigin, radius, Vector3.up, out _, castDistance + headClearanceBuffer, collisionMask, QueryTriggerInteraction.Ignore);
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private bool TryStartVault()
        {
            if (!_characterController.isGrounded)
            {
                return false;
            }

            var origin = transform.position + Vector3.up * (_characterController.height * 0.5f);
            if (!Physics.Raycast(origin, transform.forward, out var hit, vaultCheckDistance, traversalMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            var obstacleTop = hit.collider.bounds.max.y;
            var obstacleHeight = obstacleTop - transform.position.y;
            if (obstacleHeight <= 0f || obstacleHeight > maxVaultHeight)
            {
                return false;
            }

            var forwardOffset = transform.forward * (ledgeClearance + _characterController.radius);
            var landingProbeOrigin = new Vector3(hit.point.x, obstacleTop + ledgeClearance, hit.point.z) + forwardOffset + Vector3.up;
            if (!Physics.Raycast(landingProbeOrigin, Vector3.down, out var landingHit, maxVaultHeight + 2f, groundMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            var targetPosition = landingHit.point;
            targetPosition.y += _characterController.stepOffset;

            BeginTraversalRoutine(VaultRoutine(targetPosition));
            return true;
        }

        private bool TryStartClimb()
        {
            var origin = transform.position + Vector3.up * (_characterController.height * 0.5f);
            if (!Physics.Raycast(origin, transform.forward, out var hit, climbCheckDistance, traversalMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            var obstacleTop = hit.collider.bounds.max.y;
            var climbHeight = obstacleTop - transform.position.y;
            if (climbHeight < maxVaultHeight || climbHeight > maxClimbHeight)
            {
                return false;
            }

            var ledgeProbeOrigin = new Vector3(hit.point.x, obstacleTop + ledgeClearance, hit.point.z) + transform.forward * ledgeClearance;
            if (!Physics.Raycast(ledgeProbeOrigin, Vector3.down, out var landingHit, climbHeight + 2f, groundMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            var targetPosition = landingHit.point;
            targetPosition.y += _characterController.stepOffset;

            BeginTraversalRoutine(ClimbRoutine(targetPosition));
            return true;
        }

        private void BeginTraversalRoutine(IEnumerator routine)
        {
            if (_traversalRoutine != null)
            {
                StopCoroutine(_traversalRoutine);
                ResetTraversalState();
            }

            _traversalRoutine = StartCoroutine(routine);
        }

        private IEnumerator VaultRoutine(Vector3 targetPosition)
        {
            _isVaulting = true;
            _characterController.enabled = false;
            var startPosition = transform.position;
            var elapsed = 0f;

            while (elapsed < vaultDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / vaultDuration);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            transform.position = targetPosition;
            ResetTraversalState();
        }

        private IEnumerator ClimbRoutine(Vector3 targetPosition)
        {
            _isClimbing = true;
            _characterController.enabled = false;

            var startPosition = transform.position;
            var distance = Vector3.Distance(startPosition, targetPosition);
            var duration = Mathf.Max(0.35f, distance * climbDurationPerMeter);
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            transform.position = targetPosition;
            ResetTraversalState();
        }

        private void ResetTraversalState()
        {
            _characterController.enabled = true;
            _velocity = Vector3.zero;
            _isVaulting = false;
            _isClimbing = false;
            _traversalRoutine = null;
        }

        private void UpdateLean()
        {
            if (leanPivot == null)
            {
                return;
            }

            var targetAngle = _playerInput.LeanInput * leanAngle;
            _currentLeanAngle = Mathf.Lerp(_currentLeanAngle, targetAngle, Time.deltaTime * leanSmoothing);

            var targetOffset = _leanDefaultPosition + Vector3.right * (leanOffset * _playerInput.LeanInput);
            _currentLeanOffset = Vector3.Lerp(_currentLeanOffset, targetOffset, Time.deltaTime * leanSmoothing);

            leanPivot.localPosition = _currentLeanOffset;
            var targetRotation = _leanDefaultRotation * Quaternion.Euler(0f, 0f, -_currentLeanAngle);
            leanPivot.localRotation = Quaternion.Slerp(leanPivot.localRotation, targetRotation, Time.deltaTime * leanSmoothing);
        }

        private void BroadcastNoise()
        {
            _noiseTimer += Time.deltaTime;
            if (_noiseTimer < noiseBroadcastInterval)
            {
                return;
            }

            _noiseTimer = 0f;
            var isMoving = _isVaulting || _isClimbing || _currentSpeed > 0.1f;
            var noiseLevel = idleNoise;

            if (isMoving)
            {
                if (_isRunning)
                {
                    noiseLevel = runNoise;
                }
                else if (_isCrouching)
                {
                    noiseLevel = crouchNoise;
                }
                else if (_playerInput.SneakHeld)
                {
                    noiseLevel = sneakNoise;
                }
                else
                {
                    noiseLevel = walkNoise;
                }
            }
            else
            {
                if (_isCrouching)
                {
                    noiseLevel = crouchNoise * 0.5f;
                }
                else if (_playerInput.SneakHeld)
                {
                    noiseLevel = sneakNoise * 0.5f;
                }
            }

            EventBus.Publish(new PlayerNoiseEvent
            {
                NoiseLevel = noiseLevel,
                IsMoving = isMoving,
                Position = transform.position
            });
        }

        private void SetControllerHeight(float height)
        {
            var clampedHeight = Mathf.Max(0.5f, height);
            var previousHeight = _characterController.height;
            var heightDelta = clampedHeight - previousHeight;

            var center = _defaultControllerCenter;
            center.y = clampedHeight * 0.5f;

            _characterController.height = clampedHeight;
            _characterController.center = center;
            transform.position += new Vector3(0f, heightDelta * 0.5f, 0f);
        }
    }
}
