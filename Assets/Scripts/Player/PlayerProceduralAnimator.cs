using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerProceduralAnimator : MonoBehaviour
    {
        [Header("Rig Transforms")]
        [SerializeField]
        private Transform rootPivot;
        [SerializeField]
        private Transform hips;
        [SerializeField]
        private Transform torso;
        [SerializeField]
        private Transform head;
        [SerializeField]
        private Transform leftUpperArm;
        [SerializeField]
        private Transform leftLowerArm;
        [SerializeField]
        private Transform rightUpperArm;
        [SerializeField]
        private Transform rightLowerArm;
        [SerializeField]
        private Transform leftUpperLeg;
        [SerializeField]
        private Transform leftLowerLeg;
        [SerializeField]
        private Transform rightUpperLeg;
        [SerializeField]
        private Transform rightLowerLeg;
        [SerializeField]
        private Transform leftFoot;
        [SerializeField]
        private Transform rightFoot;
        [SerializeField]
        private Transform accentNode;

        [Header("Locomotion Tuning")]
        [SerializeField]
        private float idleBobAmplitude = 0.015f;
        [SerializeField]
        private float idleBobFrequency = 1.15f;
        [SerializeField]
        private float walkCycleRate = 3.5f;
        [SerializeField]
        private float runCycleRate = 6.2f;
        [SerializeField]
        private float armSwing = 28f;
        [SerializeField]
        private float legSwing = 36f;
        [SerializeField]
        private float crouchSwingMultiplier = 0.45f;
        [SerializeField]
        private float leanAngle = 18f;
        [SerializeField]
        private float leanDamping = 12f;
        [SerializeField]
        private float poseDamping = 14f;
        [SerializeField]
        private float crouchHeightOffset = 0.42f;

        private struct PoseData
        {
            public Vector3 Position;
            public Quaternion Rotation;

            public PoseData(Transform transform)
            {
                Position = transform.localPosition;
                Rotation = transform.localRotation;
            }
        }

        private readonly Dictionary<Transform, PoseData> _defaultPoses = new();
        private float _locomotionPhase;
        private float _idlePhase;
        private float _targetLean;
        private float _currentLean;
        private float _currentSpeed;
        private float _normalizedSpeed;
        private float _verticalVelocity;
        private bool _isGrounded;
        private bool _isCrouching;
        private bool _isSneaking;
        private bool _isRunning;
        private bool _isVaulting;
        private bool _isClimbing;
        private float _vaultBlend;
        private float _climbBlend;

        private static readonly Vector3 AccentForwardOffset = new(0f, 0f, 0.02f);

        private void Awake()
        {
            CachePose(rootPivot);
            CachePose(hips);
            CachePose(torso);
            CachePose(head);
            CachePose(leftUpperArm);
            CachePose(leftLowerArm);
            CachePose(rightUpperArm);
            CachePose(rightLowerArm);
            CachePose(leftUpperLeg);
            CachePose(leftLowerLeg);
            CachePose(rightUpperLeg);
            CachePose(rightLowerLeg);
            CachePose(leftFoot);
            CachePose(rightFoot);
            CachePose(accentNode);
        }

        private void CachePose(Transform target)
        {
            if (target == null || _defaultPoses.ContainsKey(target))
            {
                return;
            }

            _defaultPoses[target] = new PoseData(target);
        }

        public void SetTraversalState(bool isVaulting, bool isClimbing)
        {
            _isVaulting = isVaulting;
            _isClimbing = isClimbing;
        }

        public void UpdateLean(float leanInput)
        {
            _targetLean = Mathf.Clamp(leanInput, -1f, 1f);
        }

        public void UpdateLocomotion(float speed, float normalizedSpeed, bool isGrounded, bool isCrouching, bool isSneaking, bool isRunning, float verticalVelocity)
        {
            _currentSpeed = speed;
            _normalizedSpeed = normalizedSpeed;
            _isGrounded = isGrounded;
            _isCrouching = isCrouching;
            _isSneaking = isSneaking;
            _isRunning = isRunning;
            _verticalVelocity = verticalVelocity;
        }

        private void LateUpdate()
        {
            var dt = Time.deltaTime;
            if (dt <= 0f)
            {
                return;
            }

            float smoothing = 1f - Mathf.Exp(-poseDamping * dt);
            float leanSmoothing = 1f - Mathf.Exp(-leanDamping * dt);

            _currentLean = Mathf.Lerp(_currentLean, _targetLean, leanSmoothing);
            _vaultBlend = Mathf.MoveTowards(_vaultBlend, _isVaulting ? 1f : 0f, dt * 4.5f);
            _climbBlend = Mathf.MoveTowards(_climbBlend, _isClimbing ? 1f : 0f, dt * 4.5f);

            float locomotionInfluence = Mathf.Clamp01(_normalizedSpeed);
            if (_isSneaking)
            {
                locomotionInfluence *= 0.6f;
            }

            float cycleRate = Mathf.Lerp(walkCycleRate, runCycleRate, locomotionInfluence);
            float cycleBlend = Mathf.Clamp01(_currentSpeed * 0.35f);
            if (_isCrouching)
            {
                cycleBlend *= crouchSwingMultiplier;
            }

            if (_vaultBlend > 0.1f || _climbBlend > 0.1f)
            {
                cycleBlend = Mathf.Lerp(cycleBlend, 0f, Mathf.Max(_vaultBlend, _climbBlend));
            }

            _locomotionPhase = (_locomotionPhase + dt * cycleRate * Mathf.Sign(_currentSpeed)) % (Mathf.PI * 2f);
            _idlePhase = (_idlePhase + dt * idleBobFrequency) % (Mathf.PI * 2f);

            float swingSin = Mathf.Sin(_locomotionPhase);
            float swingCos = Mathf.Cos(_locomotionPhase);

            float idleBob = Mathf.Sin(_idlePhase) * idleBobAmplitude * (1f - cycleBlend);
            float crouchOffset = _isCrouching ? -crouchHeightOffset : 0f;

            ApplyPosition(hips, new Vector3(0f, crouchOffset + idleBob, 0f), smoothing);
            ApplyTorsoPose(smoothing);
            ApplyHeadPose(smoothing);
            ApplyAccent(smoothing);

            if (_vaultBlend > 0.01f || _climbBlend > 0.01f)
            {
                ApplyTraversalPose(smoothing);
                return;
            }

            ApplyLegs(swingSin, swingCos, cycleBlend, smoothing);
            ApplyArms(swingSin, swingCos, cycleBlend, smoothing);
            ApplyFeet(swingSin, swingCos, cycleBlend, smoothing);
        }

        private void ApplyTorsoPose(float smoothing)
        {
            if (!TryGetPose(torso, out var pose))
            {
                return;
            }

            float lean = _currentLean * leanAngle;
            float pitch = Mathf.Clamp(-_verticalVelocity * 3.5f, -15f, 12f);
            var target = pose.Rotation * Quaternion.Euler(pitch, 0f, -lean);
            SetRotation(torso, target, smoothing);
        }

        private void ApplyHeadPose(float smoothing)
        {
            if (!TryGetPose(head, out var pose))
            {
                return;
            }

            float lean = _currentLean * leanAngle * 0.5f;
            float nod = Mathf.Clamp(_verticalVelocity * 2.2f, -10f, 10f);
            var target = pose.Rotation * Quaternion.Euler(nod, 0f, -lean);
            SetRotation(head, target, smoothing * 0.8f);
        }

        private void ApplyAccent(float smoothing)
        {
            if (!TryGetPose(accentNode, out var pose))
            {
                return;
            }

            Vector3 offset = AccentForwardOffset;
            if (!_isGrounded)
            {
                offset += new Vector3(0f, Mathf.Sign(_verticalVelocity) * 0.02f, 0f);
            }

            SetPosition(accentNode, pose.Position + offset, smoothing);
        }

        private void ApplyLegs(float swingSin, float swingCos, float cycleBlend, float smoothing)
        {
            float legAmplitude = legSwing * cycleBlend;
            float kneeFold = Mathf.Clamp(-swingCos * legAmplitude * 0.5f, -22f, 36f);

            ApplyLimb(leftUpperLeg, new Vector3(legAmplitude * swingSin, 0f, 0f), smoothing);
            ApplyLimb(rightUpperLeg, new Vector3(-legAmplitude * swingSin, 0f, 0f), smoothing);

            ApplyLimb(leftLowerLeg, new Vector3(-kneeFold, 0f, 0f), smoothing);
            ApplyLimb(rightLowerLeg, new Vector3(kneeFold, 0f, 0f), smoothing);
        }

        private void ApplyArms(float swingSin, float swingCos, float cycleBlend, float smoothing)
        {
            float armAmplitude = armSwing * cycleBlend;
            ApplyLimb(leftUpperArm, new Vector3(-armAmplitude * swingSin, 0f, armAmplitude * 0.2f * swingCos), smoothing);
            ApplyLimb(rightUpperArm, new Vector3(armAmplitude * swingSin, 0f, -armAmplitude * 0.2f * swingCos), smoothing);

            ApplyLimb(leftLowerArm, new Vector3(Mathf.Clamp(-armAmplitude * swingCos, -30f, 20f), 0f, 0f), smoothing);
            ApplyLimb(rightLowerArm, new Vector3(Mathf.Clamp(armAmplitude * swingCos, -30f, 20f), 0f, 0f), smoothing);
        }

        private void ApplyFeet(float swingSin, float swingCos, float cycleBlend, float smoothing)
        {
            float footLift = Mathf.Clamp01(cycleBlend) * 0.04f;
            SetPosition(leftFoot, OffsetPose(leftFoot, new Vector3(0f, Mathf.Max(0f, -swingSin) * footLift, 0f)), smoothing);
            SetPosition(rightFoot, OffsetPose(rightFoot, new Vector3(0f, Mathf.Max(0f, swingSin) * footLift, 0f)), smoothing);
        }

        private void ApplyTraversalPose(float smoothing)
        {
            float blend = Mathf.Max(_vaultBlend, _climbBlend);
            if (blend <= 0f)
            {
                return;
            }

            float armPitch = Mathf.Lerp(0f, -70f, _vaultBlend) + Mathf.Lerp(0f, 60f, _climbBlend);
            float legFold = Mathf.Lerp(0f, 22f, _vaultBlend) + Mathf.Lerp(0f, -18f, _climbBlend);

            ApplyLimb(leftUpperArm, new Vector3(armPitch, 0f, -20f * _vaultBlend), smoothing);
            ApplyLimb(rightUpperArm, new Vector3(armPitch, 0f, 20f * _vaultBlend), smoothing);
            ApplyLimb(leftLowerArm, new Vector3(-armPitch * 0.5f, 0f, 0f), smoothing);
            ApplyLimb(rightLowerArm, new Vector3(-armPitch * 0.5f, 0f, 0f), smoothing);

            ApplyLimb(leftUpperLeg, new Vector3(legFold, 0f, 0f), smoothing);
            ApplyLimb(rightUpperLeg, new Vector3(-legFold, 0f, 0f), smoothing);
            ApplyLimb(leftLowerLeg, new Vector3(Mathf.Lerp(0f, -35f, _climbBlend), 0f, 0f), smoothing);
            ApplyLimb(rightLowerLeg, new Vector3(Mathf.Lerp(0f, 35f, _climbBlend), 0f, 0f), smoothing);

            if (TryGetPose(hips, out var hipsPose))
            {
                Vector3 offset = new(0f, -crouchHeightOffset * 0.25f, Mathf.Lerp(0f, 0.08f, _vaultBlend));
                SetPosition(hips, hipsPose.Position + offset, smoothing);
            }
        }

        private void ApplyLimb(Transform target, Vector3 eulerOffset, float smoothing)
        {
            if (!TryGetPose(target, out var pose))
            {
                return;
            }

            var targetRotation = pose.Rotation * Quaternion.Euler(eulerOffset);
            SetRotation(target, targetRotation, smoothing);
        }

        private void ApplyPosition(Transform target, Vector3 offset, float smoothing)
        {
            if (!TryGetPose(target, out var pose))
            {
                return;
            }

            SetPosition(target, pose.Position + offset, smoothing);
        }

        private void SetRotation(Transform target, Quaternion rotation, float smoothing)
        {
            if (target == null)
            {
                return;
            }

            target.localRotation = Quaternion.Slerp(target.localRotation, rotation, smoothing);
        }

        private void SetPosition(Transform target, Vector3 position, float smoothing)
        {
            if (target == null)
            {
                return;
            }

            target.localPosition = Vector3.Lerp(target.localPosition, position, smoothing);
        }

        private Vector3 OffsetPose(Transform target, Vector3 offset)
        {
            return TryGetPose(target, out var pose) ? pose.Position + offset : target.localPosition + offset;
        }

        private bool TryGetPose(Transform target, out PoseData pose)
        {
            if (target != null && _defaultPoses.TryGetValue(target, out pose))
            {
                return true;
            }

            pose = default;
            return false;
        }
    }
}
