using UnityEngine;

namespace ProjectAction.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _moveReference;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _sprintMultiplier = 1.6f;
        [SerializeField] private float _rotationSpeed = 12f;

        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private int _maxJumpCount = 2;

        private float _verticalVelocity;
        private int _jumpCount;
        private bool _loggedMissingController;

        public Transform MoveReference
        {
            get => _moveReference;
            set => _moveReference = value;
        }

        public void Tick(ProjectAction.Input.InputState input, float deltaTime)
        {
            if (_controller == null)
            {
                if (!_loggedMissingController)
                {
                    Debug.LogError("PlayerController requires a CharacterController reference.");
                    _loggedMissingController = true;
                }

                return;
            }

            var isGrounded = _controller.isGrounded;
            if (isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
                _jumpCount = 0;
            }

            if (input.JumpPressed.Value && _jumpCount < _maxJumpCount)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _jumpCount++;
            }

            var moveInput = input.Move.Value;
            var reference = _moveReference != null ? _moveReference : transform;
            var forward = Vector3.Scale(reference.forward, new Vector3(1f, 0f, 1f)).normalized;
            var right = Vector3.Scale(reference.right, new Vector3(1f, 0f, 1f)).normalized;
            var moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            var speed = _moveSpeed * (input.SprintHeld.Value ? _sprintMultiplier : 1f);
            var horizontalVelocity = moveDirection * speed;

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                var targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * deltaTime);
            }

            _verticalVelocity += _gravity * deltaTime;
            var velocity = horizontalVelocity + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * deltaTime);
        }

        public void ResetForRespawn(Vector3 position, Quaternion rotation)
        {
            if (_controller == null)
            {
                return;
            }

            _controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            _controller.enabled = true;
            _verticalVelocity = 0f;
            _jumpCount = 0;
        }
    }
}
