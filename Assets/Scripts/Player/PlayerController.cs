
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;
        [SerializeField]
        private float runSpeed = 10f;
        [SerializeField]
        private float jumpSpeed = 5f;
        [SerializeField]
        private float gravity = -9.81f;

        private CharacterController _characterController;
        private PlayerInput _playerInput;
        private Vector3 _velocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            // Grounded check
            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            // Movement
            var moveInput = _playerInput.MoveInput;
            var moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            var speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
            _characterController.Move(moveDirection * speed * Time.deltaTime);

            // Jumping
            if (_playerInput.JumpInput && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            }

            // Gravity
            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}
