using ProjectAction.Core;
using UnityEngine;

namespace ProjectAction.Input
{
    public sealed class VirtualInputBridge : ProjectBehaviour
    {
        [Header("Virtual Input State")]
        [SerializeField] private Vector2 _move;
        [SerializeField] private Vector2 _look;
        [SerializeField] private bool _jumpPressed;
        [SerializeField] private bool _sprintHeld;

        public Vector2 Move => _move;
        public Vector2 Look => _look;
        public bool JumpPressed => _jumpPressed;
        public bool SprintHeld => _sprintHeld;

        public void ClearJump()
        {
            _jumpPressed = false;
        }
    }
}
