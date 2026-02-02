using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectAction.Input
{
    public sealed class InputService
    {
        private readonly InputState _state = new();
        private readonly VirtualInputBridge _virtualInput;

        public InputService(VirtualInputBridge virtualInput)
        {
            _virtualInput = virtualInput;
        }

        public InputState State => _state;

        public void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            var gamepad = Gamepad.current;

            var move = Vector2.zero;
            if (keyboard != null)
            {
                move += new Vector2(
                    keyboard.aKey.isPressed ? -1f : keyboard.dKey.isPressed ? 1f : 0f,
                    keyboard.sKey.isPressed ? -1f : keyboard.wKey.isPressed ? 1f : 0f);
            }

            if (gamepad != null)
            {
                move += gamepad.leftStick.ReadValue();
            }

            move = Vector2.ClampMagnitude(move, 1f);

            var look = Vector2.zero;
            if (mouse != null)
            {
                look += mouse.delta.ReadValue();
            }

            if (gamepad != null)
            {
                look += gamepad.rightStick.ReadValue() * 20f;
            }

            if (_virtualInput != null)
            {
                move += _virtualInput.Move;
                look += _virtualInput.Look;
            }

            var jumpPressed =
                (keyboard != null && keyboard.spaceKey.wasPressedThisFrame) ||
                (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame) ||
                (_virtualInput != null && _virtualInput.JumpPressed);

            var sprintHeld =
                (keyboard != null && keyboard.leftShiftKey.isPressed) ||
                (gamepad != null && gamepad.rightTrigger.ReadValue() > 0.2f) ||
                (_virtualInput != null && _virtualInput.SprintHeld);

            _state.Move.Value = move;
            _state.Look.Value = look;
            _state.JumpPressed.Value = jumpPressed;
            _state.SprintHeld.Value = sprintHeld;

            _virtualInput?.ClearJump();
        }

        public void Dispose()
        {
            _state.Dispose();
        }
    }
}
