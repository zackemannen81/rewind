using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public bool JumpHeld { get; private set; }
        public float LeanInput { get; private set; }
        public bool RunHeld => _runAction != null && _runAction.IsPressed();
        public bool SneakHeld => _sneakAction != null && _sneakAction.IsPressed();

        private PlayerInputActions _playerInputActions;
        private InputAction _runAction;
        private InputAction _crouchAction;
        private InputAction _sneakAction;
        private InputAction _leanAction;
        private InputAction _climbAction;

        private bool _jumpPressedThisFrame;
        private bool _crouchToggleRequested;
        private bool _climbRequested;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            SetupAuxiliaryActions();
        }

        private void OnEnable()
        {
            var playerMap = _playerInputActions.Player;
            playerMap.Enable();
            playerMap.Move.performed += OnMove;
            playerMap.Move.canceled += OnMove;
            playerMap.Jump.performed += OnJumpPerformed;
            playerMap.Jump.canceled += OnJumpCanceled;

            _runAction?.Enable();
            _crouchAction?.Enable();
            _sneakAction?.Enable();
            _leanAction?.Enable();
            _climbAction?.Enable();
        }

        private void OnDisable()
        {
            var playerMap = _playerInputActions.Player;
            playerMap.Move.performed -= OnMove;
            playerMap.Move.canceled -= OnMove;
            playerMap.Jump.performed -= OnJumpPerformed;
            playerMap.Jump.canceled -= OnJumpCanceled;
            playerMap.Disable();

            _runAction?.Disable();
            _crouchAction?.Disable();
            _sneakAction?.Disable();
            _leanAction?.Disable();
            _climbAction?.Disable();
        }

        private void OnDestroy()
        {
            _playerInputActions?.Dispose();
            _runAction?.Dispose();
            _crouchAction?.Dispose();
            _sneakAction?.Dispose();
            _leanAction?.Dispose();
            _climbAction?.Dispose();
        }

        public bool ConsumeJumpPressed()
        {
            if (!_jumpPressedThisFrame)
            {
                return false;
            }

            _jumpPressedThisFrame = false;
            return true;
        }

        public bool ConsumeCrouchToggle()
        {
            if (!_crouchToggleRequested)
            {
                return false;
            }

            _crouchToggleRequested = false;
            return true;
        }

        public bool ConsumeClimbRequest()
        {
            if (!_climbRequested)
            {
                return false;
            }

            _climbRequested = false;
            return true;
        }

        private void SetupAuxiliaryActions()
        {
            _runAction = new InputAction("Run", InputActionType.Button);
            _runAction.AddBinding("<Keyboard>/leftShift");
            _runAction.AddBinding("<Gamepad>/leftStickPress");

            _crouchAction = new InputAction("Crouch", InputActionType.Button);
            _crouchAction.AddBinding("<Keyboard>/c");
            _crouchAction.AddBinding("<Gamepad>/b");
            _crouchAction.performed += context =>
            {
                if (context.ReadValueAsButton())
                {
                    _crouchToggleRequested = true;
                }
            };

            _sneakAction = new InputAction("Sneak", InputActionType.Button);
            _sneakAction.AddBinding("<Keyboard>/leftCtrl");
            _sneakAction.AddBinding("<Gamepad>/leftTrigger");

            _leanAction = new InputAction("Lean", InputActionType.Value);
            _leanAction.AddCompositeBinding("1DAxis")
                .With("negative", "<Keyboard>/q")
                .With("positive", "<Keyboard>/e");
            _leanAction.performed += OnLean;
            _leanAction.canceled += OnLean;

            _climbAction = new InputAction("Climb", InputActionType.Button);
            _climbAction.AddBinding("<Keyboard>/f");
            _climbAction.AddBinding("<Gamepad>/y");
            _climbAction.performed += context =>
            {
                if (context.ReadValueAsButton())
                {
                    _climbRequested = true;
                }
            };
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpHeld = true;
            _jumpPressedThisFrame = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            JumpHeld = false;
        }

        private void OnLean(InputAction.CallbackContext context)
        {
            LeanInput = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
        }
    }
}
