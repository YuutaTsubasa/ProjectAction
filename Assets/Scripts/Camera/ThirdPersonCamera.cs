using ProjectAction.Core;
using UnityEngine;

namespace ProjectAction.Camera
{
    public sealed class ThirdPersonCamera : ProjectBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _target;

        [Header("Orbit")]
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _height = 2f;
        [SerializeField] private float _minPitch = -20f;
        [SerializeField] private float _maxPitch = 70f;
        [SerializeField] private float _lookSensitivity = 0.2f;
        [SerializeField] private float _positionSmooth = 12f;

        private float _yaw;
        private float _pitch;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        public void Tick(ProjectAction.Input.InputState input, float deltaTime)
        {
            if (_target == null)
            {
                return;
            }

            var look = input.Look.Value * _lookSensitivity;
            _yaw += look.x;
            _pitch = Mathf.Clamp(_pitch - look.y, _minPitch, _maxPitch);

            var rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            var desiredPosition = _target.position + rotation * new Vector3(0f, _height, -_distance);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, _positionSmooth * deltaTime);
            transform.rotation = rotation;
        }

        public void SnapToTarget()
        {
            if (_target == null)
            {
                return;
            }

            var rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            transform.position = _target.position + rotation * new Vector3(0f, _height, -_distance);
            transform.rotation = rotation;
        }
    }
}
