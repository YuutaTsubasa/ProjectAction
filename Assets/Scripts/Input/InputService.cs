using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectAction.Input
{
    public sealed class InputService
    {
        private readonly InputState _state = new();
        private readonly VirtualInputBridge _virtualInput;
        private readonly InputActionAsset _actions;
        private readonly InputAction _moveAction;
        private readonly InputAction _lookAction;
        private readonly InputAction _jumpAction;
        private readonly InputAction _sprintAction;

        public InputService(InputActionAsset inputActions, VirtualInputBridge virtualInput)
        {
            _virtualInput = virtualInput;
            if (inputActions == null)
            {
                Debug.LogError("Input actions asset is missing. Input will be disabled.");
                return;
            }

            _actions = Object.Instantiate(inputActions);
            _moveAction = _actions.FindAction("Player/Move", true);
            _lookAction = _actions.FindAction("Player/Look", true);
            _jumpAction = _actions.FindAction("Player/Jump", true);
            _sprintAction = _actions.FindAction("Player/Sprint", true);
            _actions.Enable();
        }

        public InputState State => _state;

        public void Update()
        {
            var move = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
            var look = _lookAction != null ? _lookAction.ReadValue<Vector2>() : Vector2.zero;

            if (_virtualInput != null)
            {
                move += _virtualInput.Move;
                look += _virtualInput.Look;
            }

            move = Vector2.ClampMagnitude(move, 1f);

            var jumpPressed =
                (_jumpAction != null && _jumpAction.WasPressedThisFrame()) ||
                (_virtualInput != null && _virtualInput.JumpPressed);

            var sprintHeld =
                (_sprintAction != null && _sprintAction.IsPressed()) ||
                (_virtualInput != null && _virtualInput.SprintHeld);

            _state.Move.Value = move;
            _state.Look.Value = look;
            _state.JumpPressed.Value = jumpPressed;
            _state.SprintHeld.Value = sprintHeld;

            _virtualInput?.ClearJump();
        }

        public void Dispose()
        {
            _actions?.Disable();
            _state.Dispose();
        }
    }
}
