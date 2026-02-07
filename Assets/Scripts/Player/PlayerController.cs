using ProjectAction.AutoAttributes;
using ProjectAction.Core;
using UnityEngine;

namespace ProjectAction.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : ProjectBehaviour
    {
        private static readonly int SPEED_ID = Animator.StringToHash("Speed");
        private static readonly int IS_GROUNDED_ID = Animator.StringToHash("IsGrounded");
        private static readonly int VERTICAL_VELOCITY_ID = Animator.StringToHash("VerticalVelocity");
        private static readonly int JUMP_TRIGGER_ID = Animator.StringToHash("JumpTrigger");
        private static readonly int LAND_TRIGGER_ID = Animator.StringToHash("LandTrigger");
        private static readonly int IS_SPRINTING_ID = Animator.StringToHash("IsSprinting");

        [Header("References")]
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _moveReference;
        [SerializeField, GetComponent] private Animator _animator;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _sprintMultiplier = 1.6f;
        [SerializeField] private float _rotationSpeed = 12f;
        [SerializeField] private float _maxMoveSpeedForAnimator = 0f;

        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private int _maxJumpCount = 2;

        private float _verticalVelocity;
        private int _jumpCount;
        private bool _loggedMissingController;
        private bool _wasGrounded;

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
                _animator?.SetTrigger(JUMP_TRIGGER_ID);
            }

            var moveInput = input.Move.Value;
            var reference = _moveReference != null ? _moveReference : transform;
            var forward = reference != transform
                ? Vector3.Scale(transform.position - reference.position, new Vector3(1f, 0f, 1f)).normalized
                : Vector3.Scale(reference.forward, new Vector3(1f, 0f, 1f)).normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
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

            UpdateAnimator(horizontalVelocity.magnitude, isGrounded, input.SprintHeld.Value);
            _wasGrounded = isGrounded;
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
            _wasGrounded = _controller.isGrounded;
        }

        private void UpdateAnimator(float horizontalSpeed, bool isGrounded, bool isSprinting)
        {
            if (_animator == null)
            {
                return;
            }

            var maxSpeed = _maxMoveSpeedForAnimator > 0f
                ? _maxMoveSpeedForAnimator
                : _moveSpeed * _sprintMultiplier;
            var normalizedSpeed = maxSpeed > 0f ? Mathf.Clamp01(horizontalSpeed / maxSpeed) : 0f;

            _animator.SetFloat(SPEED_ID, normalizedSpeed);
            _animator.SetBool(IS_GROUNDED_ID, isGrounded);
            _animator.SetFloat(VERTICAL_VELOCITY_ID, _verticalVelocity);
            _animator.SetBool(IS_SPRINTING_ID, isSprinting);

            if (isGrounded && !_wasGrounded)
            {
                _animator.SetTrigger(LAND_TRIGGER_ID);
            }
        }
    }
}
