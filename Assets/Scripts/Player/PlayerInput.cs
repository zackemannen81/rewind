
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public bool JumpInput { get; private set; }

        private PlayerInputActions _playerInputActions;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Move.performed += OnMove;
            _playerInputActions.Player.Move.canceled += OnMove;
            _playerInputActions.Player.Jump.performed += OnJump;
            _playerInputActions.Player.Jump.canceled += OnJump;
        }

        private void OnDisable()
        {
            _playerInputActions.Player.Disable();
            _playerInputActions.Player.Move.performed -= OnMove;
            _playerInputActions.Player.Move.canceled -= OnMove;
            _playerInputActions.Player.Jump.performed -= OnJump;
            _playerInputActions.Player.Jump.canceled -= OnJump;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            JumpInput = context.ReadValueAsButton();
        }
    }
}
